#!/usr/bin/env node
// Mirrors docs/plan/implementation-plan.md to GitHub: one label set, one milestone per phase,
// one issue per work item (title "[P5.3] …"). Safe to run repeatedly (idempotent).
//
// Requires the GitHub CLI (`gh`) authenticated with repo scope.
//
// Usage:
//   node scripts/plan-sync-github.mjs                 dry run: prints what would change (default)
//   node scripts/plan-sync-github.mjs --apply         create/update labels, milestones, issues
//   node scripts/plan-sync-github.mjs --repo owner/name   target repository (default: jaksoftwares/phronesis-platform)
//
// Rules:
//   - Missing labels, milestones and issues are created.
//   - Plan item done + issue open   → issue is closed with a comment.
//   - Issue closed + plan item open → reported as a mismatch (the plan is the record; fix it by hand).
//   - Issues are never deleted.

import { execFileSync } from 'node:child_process';
import fs from 'node:fs';
import path from 'node:path';
import { fileURLToPath } from 'node:url';

const root = path.resolve(path.dirname(fileURLToPath(import.meta.url)), '..');
const apply = process.argv.includes('--apply');
const repoArg = process.argv.indexOf('--repo');
const repo = repoArg > -1 ? process.argv[repoArg + 1] : 'jaksoftwares/phronesis-platform';
const planUrl = `https://github.com/${repo}/blob/main/docs/plan/implementation-plan.md`;

// --- parse plan --------------------------------------------------------------
const phases = [];
let current = null;
for (const line of fs.readFileSync(path.join(root, 'docs/plan/implementation-plan.md'), 'utf8').split(/\r?\n/)) {
  const ph = line.match(/^### Phase (\d+) — (.+?)(?: \((\d+(?:\.\d+)?) weeks?\))?(?: ·.*)?$/);
  if (ph) { current = { number: Number(ph[1]), title: ph[2].trim(), items: [] }; phases.push(current); continue; }
  if (/^## /.test(line)) { current = null; continue; }
  const it = current && line.match(/^- \[( |x|X)\] (P\d+\.[A-Z0-9.]*[A-Z0-9]) (.*)$/);
  if (it) current.items.push({ id: it[2], done: it[1].toLowerCase() === 'x', text: it[3].replace(/<!--.*?-->/g, '').trim() });
}

const area = id => (/\.F\d+$/.test(id) ? 'frontend' : 'backend');
const milestoneTitle = p => `P${p.number} — ${p.title}`;
const issueTitle = i => {
  const plain = i.text.replace(/\*\*/g, '').replace(/`/g, '');
  return `[${i.id}] ${plain.length > 90 ? plain.slice(0, 89) + '…' : plain}`;
};
const issueBody = (p, i) => [
  `**Work item ${i.id}** · Phase ${p.number} — ${p.title}`,
  '',
  i.text,
  '',
  `Source of truth: [implementation plan](${planUrl}). Tick the item there when this issue is done.`,
  '',
  '### Definition of Done',
  '- [ ] Domain and application logic with tests',
  '- [ ] Endpoint documented in OpenAPI; client regenerated',
  '- [ ] Authorization policy + matrix tests',
  '- [ ] Audit and notifications where required',
  '- [ ] Frontend screens with loading/empty/error/no-permission states',
  '- [ ] Integration and E2E tests',
  '- [ ] Docs updated',
  '- [ ] Demonstrated on staging',
].join('\n');

// --- gh helpers ----------------------------------------------------------------
const gh = (args, input) => execFileSync('gh', args, { encoding: 'utf8', input, stdio: ['pipe', 'pipe', 'pipe'] });
const ghJson = args => JSON.parse(gh(args) || '[]');
const plan = [];
const act = (desc, fn) => { plan.push(desc); if (apply) fn(); };

if (apply) {
  try { gh(['--version']); } catch { console.error('GitHub CLI (gh) not found. Install it and run `gh auth login`.'); process.exit(1); }
}
const existing = apply || hasGh()
  ? {
      labels: new Set(ghJson(['label', 'list', '--repo', repo, '--limit', '500', '--json', 'name']).map(l => l.name)),
      milestones: new Map(ghJson(['api', `repos/${repo}/milestones?state=all&per_page=100`]).map(m => [m.title, m.number])),
      issues: ghJson(['issue', 'list', '--repo', repo, '--state', 'all', '--limit', '2000', '--json', 'number,title,state']),
    }
  : { labels: new Set(), milestones: new Map(), issues: [] };

function hasGh() { try { execFileSync('gh', ['auth', 'status'], { stdio: 'ignore' }); return true; } catch { return false; } }

// --- labels --------------------------------------------------------------------
const labels = [
  ...phases.map(p => ({ name: `phase:P${p.number}`, color: 'C01B22' })),
  { name: 'area:backend', color: '1F1E1C' }, { name: 'area:frontend', color: 'BC8D3F' },
  { name: 'plan-item', color: 'F6EEDC' }, { name: 'blocked', color: 'A3131A' },
];
for (const l of labels) if (!existing.labels.has(l.name)) act(`create label ${l.name}`, () => gh(['label', 'create', l.name, '--repo', repo, '--color', l.color]));

// --- milestones ----------------------------------------------------------------
for (const p of phases) {
  const t = milestoneTitle(p);
  if (!existing.milestones.has(t)) act(`create milestone "${t}"`, () => gh(['api', `repos/${repo}/milestones`, '-f', `title=${t}`]));
}

// --- issues --------------------------------------------------------------------
const byId = new Map(existing.issues.map(i => [i.title.match(/^\[(P[^\]]+)\]/)?.[1], i]).filter(([k]) => k));
const mismatches = [];
let creates = 0, closes = 0;
for (const p of phases) for (const i of p.items) {
  const issue = byId.get(i.id);
  if (!issue) {
    if (i.done) continue; // do not create issues for work finished before syncing started
    creates++;
    act(`create issue ${issueTitle(i)}`, () => gh(['issue', 'create', '--repo', repo, '--title', issueTitle(i),
      '--body-file', '-', '--label', `plan-item,phase:P${p.number},area:${area(i.id)}`, '--milestone', milestoneTitle(p)], issueBody(p, i)));
  } else if (i.done && issue.state === 'OPEN') {
    closes++;
    act(`close #${issue.number} ${i.id} (done in plan)`, () => gh(['issue', 'close', String(issue.number), '--repo', repo,
      '--comment', `Marked done in the implementation plan (${i.id}).`]));
  } else if (!i.done && issue.state === 'CLOSED') {
    mismatches.push(`#${issue.number} ${i.id} is closed on GitHub but not ticked in the plan`);
  }
}

console.log(`${apply ? 'Applied' : 'Dry run'} for ${repo}: ${plan.length} change(s) — ${creates} issue(s) to create, ${closes} to close.`);
for (const p of plan.slice(0, 40)) console.log('  ' + p);
if (plan.length > 40) console.log(`  … and ${plan.length - 40} more`);
if (mismatches.length) { console.log('Mismatches (fix the plan or reopen the issue):'); mismatches.forEach(m => console.log('  ' + m)); }
if (!apply) console.log('Nothing was changed. Re-run with --apply to make these changes.');
