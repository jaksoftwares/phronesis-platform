# Progress Tracking

How Phronesis tracks delivery of the [implementation plan](implementation-plan.md): what counts as progress, where it is recorded, how it is reported, and how often.

## 1. Principles

- **Tracking is internal.** Progress lives in the repositories and the GitHub Project only. Nothing about build progress appears in the application (decision D-018).
- **One record of scope:** the implementation plan. Every work item has a permanent ID (`P7.15`, `P2.F1`). Nothing is built that isn't in the plan or the decision log.
- **Progress means working software.** An item is ticked `[x]` only when it meets the Definition of Done (plan §3.3) **and** has been demonstrated on staging. Code merged but not demonstrated is "in progress".
- **Phases close with evidence.** A phase is finished only after its exit review, which records the demo of every exit criterion.
- **Numbers are generated, never typed.** Percentages come from `scripts/plan-progress.mjs`, so the report can't drift from the plan.

## 2. The tracking stack

| Layer | Tool | Holds | Updated |
|---|---|---|---|
| Scope and status | `docs/plan/implementation-plan.md` | Work items, status markers, phase structure | When an item starts, blocks or finishes |
| Progress report | `docs/plan/PROGRESS.md` + `progress.json` | Overall % (weighted by phase weeks), per-phase %, milestones, blocked/in-progress lists, decisions due | Generated: `node scripts/plan-progress.mjs`; CI fails if stale |
| Day-to-day work | GitHub Project **"Phronesis Delivery"** + issues in `phronesis-platform` | One issue per work item, plus bugs and tasks; board, roadmap and per-phase views | Continuously; synced from the plan with `node scripts/plan-sync-github.mjs --apply` |
| Weekly status | `docs/plan/status/YYYY-Www.md` | RAG status, done/next, risks, decisions needed | Every Friday |
| Phase exit | `docs/plan/reviews/phase-NN.md` | Exit criteria evidence, demo links, deviations, decisions | End of each phase |
| Decisions | `docs/plan/decisions-and-business-rules.md` | Decision log and open decisions with gates | When a decision is taken |

## 3. Status markers in the plan

```markdown
- [ ] P5.3 Access rules per resource …                         not started
- [ ] P5.3 Access rules per resource … <!-- wip -->           in progress
- [ ] P5.3 Access rules per resource … <!-- blocked: O-06 -->  blocked (reason shown in PROGRESS.md)
- [x] P5.3 Access rules per resource …                         done and demonstrated on staging
```

New items take the next free number in their section and are never renumbered. Dropped items are struck through (`~~…~~`) with a decision reference, not deleted.

## 4. How the percentage is calculated

- **Phase %** = done items ÷ items in the phase.
- **Overall %** = Σ(phase weeks × phase %) ÷ total planned weeks. A five-week phase counts more than a two-week phase.
- **Milestones** are reached when the phase that delivers them has an exit review file.
- Item count is a proxy; the weekly status adds judgement (RAG) where items differ greatly in size.

## 5. GitHub Project setup (one-time)

1. Install the GitHub CLI and run `gh auth login` (scopes: `repo`, `project`).
2. Dry run: `node scripts/plan-sync-github.mjs`. Review, then `node scripts/plan-sync-github.mjs --apply`. This creates labels (`phase:P0…P11`, `area:backend|frontend`, `plan-item`, `blocked`), one milestone per phase and one issue per open work item.
3. Create a GitHub Project (Projects → New → "Phronesis Delivery") owned by the account, and enable the **auto-add workflow** for issues labelled `plan-item` from both repositories.
4. Project fields: Status (Backlog, Ready, In progress, In review, On staging, Done), Phase, Area, Size (S/M/L), Target week.
5. Views:
   - **Board** by Status for the current phase.
   - **Roadmap** by milestone (phase) dates.
   - **Table** grouped by Phase with progress counts.
   - **Blocked** filtered on the `blocked` label.
6. Pull requests reference their item: `feat(payments): finance confirmation queue (P7.5)` and `Closes #123`.

Frontend issues live in the same tracker (label `area:frontend`), so the whole platform is planned in one place. The frontend PRs are in `phronesis-web` and link to the issue.

## 6. Cadence

| When | What | Output |
|---|---|---|
| Daily | Move issues across the board; mark plan items `wip`/`blocked` | Board current |
| Each merged item | Tick the plan item after the staging demo; regenerate `PROGRESS.md`; run the sync script | Report and issues match the plan |
| Weekly (Friday) | Status report from the template; review blocked items and decisions due | `docs/plan/status/YYYY-Www.md` |
| End of phase | Exit review meeting with Phronesis: demo every exit criterion on staging | `docs/plan/reviews/phase-NN.md`; milestone reached |
| Monthly | Capacity review of the VPS, restore drill, risk register review | Notes in the weekly status |

## 7. RAG status (weekly)

| Status | Meaning |
|---|---|
| Green | On plan: current phase on track for its planned weeks; no blocker older than 2 working days |
| Amber | Up to 1 week behind, or a blocker / overdue decision with a known path |
| Red | More than 1 week behind, a blocked exit criterion, or a decision gate missed with no default accepted |

## 8. Delivery health metrics (in the weekly status)

Overall and current-phase % · items done this week · blocked items and age · decisions due/overdue · CI pass rate on `main` · open bugs by severity · escaped defects (bugs found on staging after an item was ticked) · staging availability.
