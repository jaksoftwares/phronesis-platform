# Phronesis Implementation Plan

**From audit to a stable, professional learning platform.**

| Item | Detail |
|---|---|
| Version | 1.0 — 20 November 2026 |
| Inputs | [Audit](../audit/2026-11-20-platform-audit.md) · [Target architecture](target-architecture.md) · [Decisions & business rules](decisions-and-business-rules.md) · product blueprint (`system-modules.md`) · DB and API specs |
| Repositories | `phronesis-platform` (backend, owns this plan) · `phronesis-web` (frontend) |
| Status | Approved for execution, phase by phase |

This plan supersedes every earlier implementation plan in both repositories (`ORDER OF IMPLEMENTATITON OF THE MODULES.md`, `initial.md` §69, `phronesis-web/docs/ORDER-OF-IMPLEMENTATION-PHASES-INITIAL`, `first-module-plan`, `learner_portal_implementation_plan.md`, `learner_workspace_enhancement.md`). They move to `docs/archive/` in Phase 0.

---

## 1. How to read this plan

- Phases run **in order**. Each one ends with an **exit review**: every exit criterion is demonstrated on staging before the next phase starts.
- Each phase delivers **complete vertical slices**: domain → application → API → authorization → audit → notifications → frontend → tests → docs. Nothing is "done" on the backend alone.
- Work items are checkboxes so progress can be tracked in this file (or mirrored to GitHub Issues/Projects with the same IDs, e.g. `P5.3`).
- **Gates** reference open decisions in the decision log (`O-xx`). If a gate is not decided in time, the documented default is used and recorded as a decision.
- Durations assume **one full-time senior developer working with AI assistance**. With two or three developers, Phases 5–10 can partly overlap (backend and frontend streams), bringing the total from about 44 to about 28 weeks.
- Every work item has a permanent ID (`P5.1`, `P11.2.3`, `P3.F2`). IDs are never renumbered; new items take the next free number. Progress is tracked as described in [progress tracking](progress-tracking.md) and summarised in [PROGRESS.md](PROGRESS.md).

## 2. Phase overview

| # | Phase | Modules | Weeks | Cumulative | Outcome |
|---|---|---|---|---|---|
| 0 | Reset & reorganisation | — | 1.5 | 1.5 | Clean repos, one source of truth, legacy frozen |
| 1 | Platform foundation, DevOps & design system | M00–M02, M32–M33 base | 5 | 6.5 | Clone → run → test → deploy to staging; new brand system live |
| 2 | Identity, access & staff | M03–M05 | 3.5 | 10 | Secure login, MFA, roles and permissions, staff invitations, audit |
| 3 | Learners, guardians, teachers & verification | M06–M08 | 4 | 14 | Accounts for every persona; complete teacher verification lifecycle |
| 4 | Curriculum & academic configuration | M09–M10 | 2 | 16 | CBC taxonomy configurable and imported |
| 5 | Content, review, secure media & discovery | M11–M14 | 5 | 21 | Content authored, reviewed, published, protected and found |
| 6 | Learning workspace, assessments & progress | M15–M17 | 4 | 25 | Learners study, practise and see progress; guardians see it too |
| 7 | Plans, entitlements, manual payments & **exam papers store** | M18–M20, M37 | 6 | 31 | Paybill → finance confirmation → entitlements; exam papers sold online; marking schemes by request |
| 8 | Virtual classes, booking & attendance | M21–M24 | 4 | 35 | Classes booked, linked to Meet, attended and recorded |
| 9 | Communication, support & collaboration | M25–M27 | 3 | 38 | Reminders, announcements, helpdesk, teacher contributions |
| 10 | Administration, reporting, audit & safeguarding | M28–M31 | 3 | 41 | Operations console, national reports, compliance, safeguarding |
| 11 | Public website, hardening & launch | M32–M34 (launch scope) | 3 | 44 | Production launch with monitoring, backups and hypercare |
| 12 | Post-launch roadmap | M34–M36, deferred items | — | — | PWA, Daraja, SMS, Calendar API, analytics, personalisation |

**M37 Exam Papers Store & Marking Schemes** is a module added to the blueprint on 2026-11-20 (D-014, D-015): Phronesis sells exam papers online to learners and the public, and handles marking-scheme requests end to end.

```
Wk  0    5    10   15   20   25   30   35   40   44
P0  ▌
P1   ████
P2       ███
P3          ████
P4              ██
P5                ████▌
P6                     ████
P7                         ██████
P8                               ████
P9                                   ███
P10                                     ███
P11                                        ███  ► launch
R1 (optional early store release)        ▲ end of P7 + store hardening
```

Milestones: **M1** foundation on staging (end P1) · **M2** "first verified teacher" (end P3) · **M3** "first published lesson" (end P5) · **M4** "first paid learner" and **M4b** "first exam paper sold" (end P7) · **M5** "first live class" (end P8) · **M6** operational readiness (end P10) · **M7** public launch (end P11).

**Optional early release R1 — Exam Papers Store (O-19).** After Phase 7, identity, content, media, payments and the store are complete. The store, accounts and payments can go live on production as an early revenue release while classes are still being built. This requires the Phase 11 hardening checklist for those surfaces only: security review, load test, legal pages, store SEO pages and backups verified. The public site at R1 contains only finished pages: home, exam store, about, contact and legal. Live classes and learning features are absent from it until they ship, with no "coming soon" references (§3.5).

---

## 3. Ways of working (applies to every phase)

### 3.1 Repository and delivery workflow

- **Branch model: `dev` → `test` → `main`** (both repositories).
  - Work happens on **local** branches named by work item (e.g. `p2/P2.4-mfa`). These are never pushed.
  - The repository owner reviews and merges work into `dev`, promotes `dev` → `test` for staging verification, and `test` → `main` for production. **Only the owner merges and pushes `dev`, `test` and `main`.**
  - `dev` = integrated work, CI must be green. `test` = deployed to staging for demos, exit reviews and UAT. `main` = production; every merge is tagged (`v0.x` before launch, `v1.0.0` at launch).
  - Fixes found in `test` go back through a local branch → `dev` → `test`, so the branches never diverge.
- Conventional Commits (`feat(payments): …`); PR template with checklist (tests, auth, audit, docs, screenshots).
- Every PR: CI green (build, tests, lint, type-check, architecture tests, vulnerability scan), self-review against the Definition of Done, and review (human or AI-assisted, then human approval).
- Releases: semantic version tags (`v0.x` until launch, `v1.0.0` at launch) and a `CHANGELOG.md`.
- Architecture decisions recorded as ADRs in `docs/adr/`.

### 3.2 Definition of Ready (before a module starts)

Business rules for the module are written in the decision log or module spec; open decisions resolved or defaulted; screens listed; API slice identified in the API spec; permissions identified in the matrix.

### 3.3 Definition of Done (every work item)

1. Domain model and invariants implemented with unit tests.
2. Use cases in the Application layer; no business logic in endpoints.
3. Endpoints documented in OpenAPI; generated client updated.
4. Authorization policy declared and **authorization matrix tests** pass.
5. Audit records for administrative, financial, verification, publishing and permission actions.
6. Notifications wired through the outbox where the flow needs them.
7. Frontend screens in the design system, with loading, empty, error and no-permission states; responsive to 360 px; keyboard accessible.
8. Integration tests against PostgreSQL; E2E test for the module's critical path.
9. Logging without PII; metrics for key business events.
10. Docs updated (module spec, API spec status, runbook if operational).
11. Deployed to staging and demonstrated.
12. Release discipline (§3.5): no placeholder, mock data or unfinished UI reachable; the feature stays flagged off until its whole workflow is complete.

### 3.4 Module rebuild protocol

For each legacy module being replaced:

1. **Review** the legacy code and screens in full (closing the audit's "sampled" gaps) and list anything worth keeping.
2. **Specify**: module spec in `docs/specs/modules/<module>.md`: rules, states, permissions, endpoints, screens, events, acceptance criteria.
3. **Build** the slice in the new structure.
4. **Replace**: point frontend screens at the new endpoints; delete the legacy controller, service and page code in the same PR.
5. **Verify**: tests, E2E, staging demo.

Legacy code is never patched for production use; nothing legacy is ever deployed.

### 3.5 Release discipline: the app only shows finished work

Delivery tracking is internal. It lives in the repositories (this plan, `PROGRESS.md`, status reports) and in the GitHub Project, never in the product. The application that learners, guardians, teachers, buyers and staff use contains **only complete, working features**.

| Never in the app | Instead |
|---|---|
| "Coming soon", "under construction", "beta" badges, disabled or greyed-out menu items for unbuilt features | Unbuilt modules have no routes and no navigation entries. They appear only when they ship. |
| Mock, sample or hard-coded data (fake students, fake invoices, placeholder numbers) | Real data from the API. When there is none, a designed empty state that explains what will appear and how to start (e.g. "No papers purchased yet — browse the store"). |
| Placeholder pages (a heading and nothing else) | The page is not routed until its content is complete. |
| API endpoints returning `501 Not Implemented`, mock providers in production | Endpoints exist only when implemented. Mock/fake providers are registered in tests and local development only. |
| Progress bars, roadmaps or "what's next" for the build | Internal documents only. |
| Dev and demo routes (`/design-system`, `/brand-audit`) | Component catalogue runs as a separate Storybook, outside the app. |

How it is enforced:
- **Feature flags for unreleased modules.** A module merged to `main` but not yet released is switched off by a server-side flag. Its routes return 404 and its navigation is absent, never "coming soon". Release = flag on, after the phase exit review.
- **Staging = what production will be**, plus flagged modules switched on for review behind staging basic-auth.
- **CI guards** (P1.4.4) fail the build on placeholder copy, mock-data imports outside tests, 501 responses, and dev-only routes in production builds.
- **Each production release contains whole workflows.** A partly built workflow (e.g. checkout without finance confirmation) stays flagged off until the whole workflow passes its E2E test.

---

## 4. Phases in detail

### Phase 0 — Reset & reorganisation (1.5 weeks)

**Goal:** stop the drift. One source of truth, legacy code frozen, repos ready for the rebuild.

**P0.1 Freeze and tag legacy**
- [ ] P0.1.1 Tag both repos `legacy-v0` (backend `3a143f2`, frontend `fc6ffd9`) and note in README that legacy code is reference only.
- [ ] P0.1.2 Commit the staged untracking of `wwwroot/uploads` and the `.gitignore` change.
- [ ] P0.1.3 Move legacy backend code out of the build: `legacy/` folder (excluded from the solution) containing old controllers/services for reference during module reviews. The API is left with only real endpoints (health); all 501 stubs and mock providers are removed from the running app.
- [ ] P0.1.4 Remove `phronesis.db`, dev HTML landing page and markdown spec route from `Program.cs`.

**P0.2 Documentation reorganisation (backend repo)**
- [ ] P0.2.1 Create the structure in [target architecture §9](target-architecture.md#9-documentation-structure-target): `product/`, `specs/`, `plan/`, `adr/`, `audit/`, `runbooks/`, `archive/`.
- [ ] P0.2.2 Move and rename: `problem-definition-and-discovery.md` → `product/`, `system-modules.md` → `product/`, DB and API specs → `specs/`; give extension-less files `.md` extensions; fix misspelt names.
- [ ] P0.2.3 Move walkthrough reports (identity, staff, teacher, M11–M14, media, search) and superseded plans to `archive/` with a header "superseded by the implementation plan of 2026-11-20".
- [ ] P0.2.4 Delete empty `problemstatement.md`.
- [ ] P0.2.5 Write `docs/README.md`: index + "source of truth" table.
- [ ] P0.2.6 Write ADRs 0001–0008 from decisions D-001, D-007, D-011, D-012, D-013 and the token design.

**P0.3 Documentation reorganisation (web repo)**
- [ ] P0.3.1 `docs/README.md` linking to this plan; keep only frontend-specific docs.
- [ ] P0.3.2 Replace the three old brand documents with `docs/design-system/brand-and-design-system.md`; move old ones to `docs/archive/`.
- [ ] P0.3.3 Move `learner_portal_implementation_plan.md` and old plans to `docs/archive/`.
- [ ] P0.3.4 Rename `public/new-logos-phronesis/` → `public/brand/source/` (raw files) and retire the old blue/teal brand PDFs to the archive.

**P0.4 Frontend triage**
- [ ] P0.4.1 Move all legacy pages and their components out of `src/app` into `legacy/` (outside the build, kept for reference during each module's rebuild). The routed app starts clean, with no mock-data screens, so `next build` passes without patching broken legacy code.
- [ ] P0.4.2 Remove dev routes (`/brand-audit`, `/identity`) and the dead `lib/api.ts`.
- [ ] P0.4.3 Upgrade vulnerable dependencies (`next`, `sharp`, `zod`, transitive) and re-run `npm audit`.

**P0.5 Decisions**
- [ ] P0.5.1 Walk through open decisions O-01…O-14 with Phronesis; record outcomes or confirm defaults; set gates.
- [ ] P0.5.2 Request vector logo files (O-12).

**Exit criteria:** both repos tagged; docs reorganised with one index; frontend builds; no high/critical dependency alerts; decision log updated.

---

### Phase 1 — Platform foundation, DevOps & design system (5 weeks)

**Goal:** the foundation the specs demanded: anyone can clone, run, test and deploy; the new brand exists as a working design system.

**P1.1 Backend solution and conventions**
- [ ] P1.1.1 `Phronesis.slnx` lists all projects; `Directory.Build.props` (nullable, warnings as errors, analyzers, deterministic builds); `Directory.Packages.props` (central versions).
- [ ] P1.1.2 Layer and module folder structure per target architecture §4.1; `.editorconfig`.
- [ ] P1.1.3 Building blocks: `Entity`, `AggregateRoot`, `ValueObject`, `DomainEvent`, `Result`/error model, `Money`, `IClock`, `ICurrentUser`.
- [ ] P1.1.4 Command/query handler abstractions + decorators (validation, transaction, audit, logging) registered with Scrutor.
- [ ] P1.1.5 Error handling: ProblemDetails with stable codes; global exception handler; consistent success envelope helpers.
- [ ] P1.1.6 API versioning (`/api/v1`), OpenAPI + Scalar, OpenAPI JSON exported in CI to `docs/api/openapi.v1.json`.
- [ ] P1.1.7 Authorization foundation: **fallback policy = authenticated + explicit policy required**; permission policy provider (no undefined-policy fallthrough); architecture test "every endpoint declares a policy".

**P1.2 Data foundation**
- [ ] P1.2.1 PostgreSQL 17 with Npgsql; snake-case naming; schema-per-module configuration; UUID v7 key generation.
- [ ] P1.2.2 Interceptors: timestamps, audit trail, outbox capture of domain events.
- [ ] P1.2.3 New migration baseline (legacy SQL Server migrations archived); migrations run as a separate deploy step, never at app start in production.
- [ ] P1.2.4 Environment-aware seeding framework (reference data everywhere; demo data local/staging only; no passwords in code: first Super Admin created by a one-time CLI command).
- [ ] P1.2.5 Hangfire on PostgreSQL + outbox dispatcher job; dashboard behind `admin.jobs` permission.
- [ ] P1.2.6 `IFileStorage` (private disk) with validation pipeline (allow-lists, magic bytes, size, server-generated keys).
- [ ] P1.2.7 Email abstraction with templating (Razor or MJML-based) + Mailpit in development.

**P1.3 Cross-cutting runtime**
- [ ] P1.3.1 Serilog JSON logging with PII/secret redaction (no request bodies); correlation IDs across API and web.
- [ ] P1.3.2 OpenTelemetry traces/metrics exporter (configurable endpoint); Sentry-compatible error tracking.
- [ ] P1.3.3 Health checks: live/ready (PostgreSQL, storage, Hangfire).
- [ ] P1.3.4 Rate limiting policies; security headers; strict CORS from configuration.
- [ ] P1.3.5 Options validation at startup (fail fast on missing secrets); `.env.example` complete.

**P1.4 Testing foundation**
- [ ] P1.4.1 Test projects per target architecture; Testcontainers PostgreSQL fixture; Respawn reset; test data builders.
- [ ] P1.4.2 Architecture tests: layer rules, module boundaries, no `DbContext` in Api, endpoint-policy rule.
- [ ] P1.4.3 Authorization matrix test harness (role × endpoint → expected status) ready for every later phase.
- [ ] P1.4.4 Release-hygiene guards in CI (both repos): fail on placeholder copy ("coming soon", "lorem ipsum", "TBD", "under construction"), mock/fixture imports outside test folders, endpoints returning 501, mock providers registered outside Development/Test, dev-only routes in production builds, `console.log` and `alert()` in shipped code.
- [ ] P1.4.5 Server-side feature flags for module release (routes 404 and navigation hidden while off), managed by Super Admin, audited; frontend navigation built only from enabled modules.

**P1.5 DevOps and infrastructure**
- [ ] P1.5.1 `deploy/docker-compose.dev.yml` (PostgreSQL, Mailpit), `.staging.yml`, `.prod.yml`, Caddyfile, Postgres tuning for KVM 2, resource limits per [memory budget](target-architecture.md#31-memory-budget-production--small-staging-on-the-same-vps).
- [ ] P1.5.2 Dockerfiles: API (chiselled/non-root), web (Next standalone, non-root).
- [ ] P1.5.3 GitHub Actions: backend and frontend CI on pushes to `dev`, `test` and `main`; image build → GHCR; deploy to staging on push to `test`; deploy to production on push to `main` (tagged) with manual approval; branch protection on all three.
- [ ] P1.5.4 Provision the Hostinger KVM 2: hardening (SSH keys, ufw, fail2ban, unattended upgrades), Docker, DNS (O-10), Caddy TLS, staging basic-auth.
- [ ] P1.5.5 Backups: nightly DB + files to off-box encrypted storage; restore script; first restore drill.
- [ ] P1.5.6 External uptime monitoring; alert channel.
- [ ] P1.5.7 Runbooks: local setup, deploy, rollback, backup/restore, secrets rotation.

**P1.6 Frontend foundation and design system** (see `phronesis-web/docs/design-system/brand-and-design-system.md`)
- [ ] P1.6.1 New folder structure (target architecture §5.1); `middleware.ts` → `proxy.ts`; strict TypeScript; ESLint zero-error baseline; Prettier; Husky + lint-staged.
- [ ] P1.6.2 Generated API client pipeline (OpenAPI → typed client + TanStack Query hooks); query client defaults; error normalisation from ProblemDetails.
- [ ] P1.6.3 Session handling: in-memory access token, refresh via cookie, `/auth/me`, permission helpers, surface guards (wired fully in Phase 2).
- [ ] P1.6.4 Brand tokens (colour, type, spacing, radius, elevation, motion) as CSS variables + Tailwind v4 theme; self-hosted fonts via `next/font`.
- [ ] P1.6.5 Core components: Button, IconButton, Link, Input, Textarea, Select, Combobox, Checkbox, Radio, Switch, DatePicker, FileUpload, Form field, Dialog, Drawer, Dropdown, Tabs, Table (sortable, paginated), Card, Badge/Status pill, Avatar, Toast, Alert, Skeleton, EmptyState, ErrorState, Pagination, Breadcrumbs, Stepper, ProgressBar, Tooltip.
- [ ] P1.6.6 Layout shells: Public header/footer, Portal shell (learner/family/teach), Staff Console shell (permission-driven nav), Account shell.
- [ ] P1.6.7 Component catalogue in Storybook (separate from the app, never deployed with it) with accessibility checks.
- [ ] P1.6.8 Vitest + Testing Library, Playwright setup with a smoke test, axe checks.

**Exit criteria (Milestone M1):** a new developer runs `docker compose up` + `dotnet test` + `npm test` successfully from the README in under 30 minutes; CI is green and blocks on failures; staging is live over HTTPS with `/health/ready` reporting real dependency status; a deliberately unprotected endpoint fails the architecture test; backups run and one restore has been rehearsed; the design-system catalogue shows the new brand in all components.

---

### Phase 2 — Identity, access & staff (3.5 weeks) · M03–M05

**Goal:** secure accounts and governance for every later module.

**Backend**
- [ ] P2.1 Users, credentials, email verification (hashed single-use tokens), login with lockout and rate limits, logout.
- [ ] P2.2 Sessions: rotating hashed refresh tokens, reuse detection, device list, revoke one/all; revoke all on password reset/suspension.
- [ ] P2.3 Password reset and change; password policy; breached-password check.
- [ ] P2.4 MFA (TOTP + recovery codes), mandatory for staff roles.
- [ ] P2.5 Roles and permissions seeded from code (role model in decisions §9); permission matrix document generated from code; role assignment with separation-of-duties rules; changes audited.
- [ ] P2.6 Staff: departments, staff profiles, **invitation flow** (email link to set password + MFA — no temporary passwords in responses), suspend/reactivate.
- [ ] P2.7 Account status (pending/active/suspended/deactivated) separate from profile statuses.
- [ ] P2.8 Agreements framework: versioned legal documents and acceptance records (terms, privacy; teacher agreement later).
- [ ] P2.9 Security events log (login success/failure, MFA changes, role changes) and audit for all admin actions.
- [ ] P2.10 Transactional emails: verify email, reset password, staff invitation, security alerts.

**Frontend**
- [ ] P2.F1 Single `/login` with role-based landing; register (learner/guardian entry points), verify email, forgot/reset password, MFA setup and challenge.
- [ ] P2.F2 `/account`: profile, password, MFA, sessions, agreements.
- [ ] P2.F3 Staff Console: Users (search, filter, status actions), Roles & permissions (view; edit for Super Admin), Staff & invitations, Security events.
- [ ] P2.F4 Surface guards and 403/404 pages.

**Exit criteria:** authorization matrix covers 100 % of Phase 2 endpoints; refresh token never readable by JavaScript (verified); staff cannot reach the console without MFA; every role change appears in the audit log; E2E: register → verify → login → MFA (staff) → logout.

---

### Phase 3 — Learners, guardians, teachers & verification (4 weeks) · M06–M08

**Gate:** O-02 (minors and guardians).

**Backend**
- [ ] P3.1 Learner profile (admission number, date of birth, curriculum, current grade, **county**, status); one canonical learner ID used everywhere (fixes H-02). County is also captured for guardians, teachers and store buyers to support national operations reporting (P10.7).
- [ ] P3.2 Guardian profile; guardian–learner links with relationship, primary payer flag, consent record and visibility permissions; link invitations in both directions with expiry.
- [ ] P3.3 Minor rules per O-02 (consent gate on purchases and live classes).
- [ ] P3.4 Teacher profile (bio, qualifications, experience, competences per grade/subject, status: Applicant → Verified → Onboarding → Active → Suspended/Revoked).
- [ ] P3.5 Teacher application state machine: Draft → Submitted → Under review → (Interview scheduled) → Changes requested → Approved / Rejected; resubmission rules; history.
- [ ] P3.6 Documents: typed (ID, certificates, TSC registration, etc.), validated uploads to private storage, per-document verification status, reviewer-only access with logged views.
- [ ] P3.7 Reviewer workflow: queue, assignment, completeness checklist, notes, interview scheduling with manual Meet link, decision with reason, notifications to applicant.
- [ ] P3.8 Teacher agreement acceptance (conduct, safeguarding, content licence); onboarding fee step modelled as a pending requirement (activated in Phase 7).
- [ ] P3.9 Suspension/revocation with reason, audit, and effect on permissions.

**Frontend**
- [ ] P3.F1 Learner onboarding (grade, subjects); guardian onboarding and linking flows; consent screens.
- [ ] P3.F2 Teacher Arena: application wizard (profile → competences → documents → review & submit), status tracker, change requests.
- [ ] P3.F3 Staff Console → Academic review: applications queue, application detail with document viewer, checklist, interview, decision.
- [ ] P3.F4 Family portal shell: linked learners list and link management.

**Exit criteria (Milestone M2):** E2E: teacher applies → reviewer requests changes → teacher resubmits → interview → approval → teacher sees "complete onboarding"; guardian links a learner with consent; no document reachable without authorization; all decisions audited.

---

### Phase 4 — Curriculum & academic configuration (2 weeks) · M09–M10

**Gate:** O-01 (official taxonomy and launch grades).

- [ ] P4.1 Curriculum and curriculum versions; education levels; grades; subjects; grade–subject mapping; strands; sub-strands (topics); learning objectives; ordering; prerequisites. Nothing hard-coded (data-driven per `initial.md` §10).
- [ ] P4.2 Spreadsheet import (validate → preview diff → apply) and export; audit of taxonomy changes; archive instead of delete.
- [ ] P4.3 Teacher competence mapping uses the taxonomy (migrate Phase 3 free-text if any).
- [ ] P4.4 Cached public read endpoints (grades → subjects → topics).
- [ ] P4.5 Frontend: Staff Console → Curriculum manager (tree editor, reorder, import wizard); public/learner subject browser components.
- [ ] P4.6 Import the approved CBC taxonomy for the launch grades.

**Exit criteria:** content manager imports and edits the taxonomy without developer help; learners browse grade → subject → topic; taxonomy changes audited.

---

### Phase 5 — Content, review, secure media & discovery (5 weeks) · M11–M14

**Gates:** O-06 (free vs premium, downloads, ownership), O-07 (video hosting).

**Backend**
- [ ] P5.1 Resource aggregate with versions; resource types (notes, past papers, topical questions, exercises, exams, marking schemes, videos, animations, class materials, teacher resources); multi-classification (grades, subjects, topics, objectives); tags; licensing and ownership metadata.
- [ ] P5.2 Lifecycle: Draft → Submitted → Under review → Changes requested → Approved → Scheduled → Published → Archived; Rejected. Author ≠ reviewer; checklist; threaded comments; publish scheduling; new version of a published resource goes through review while the old version stays live.
- [ ] P5.3 Access rules per resource: free, premium (entitlement key), staff-only; preview segments.
- [ ] P5.4 Media: upload sessions, validation, private storage, processing status; signed short-lived access endpoints streaming files; PDF viewing with per-user watermark; download policy; access logging; video via external host with signed playback (O-07).
- [ ] P5.5 Discovery: PostgreSQL full-text search with weights, trigram fuzzy matching, filters (grade, subject, topic, type, free/premium), sorting, facets; featured, recent and popular lists.
- [ ] P5.6 Events: `ResourcePublished` → notifications/projections via outbox.
- [ ] P5.7 Exam-paper metadata on resources: exam type (past paper, mock, end-of-term, opener, county/zonal, Phronesis original), year, term, paper number, duration, total marks, grade(s), subject, and a **linked marking scheme** resource (kept as a separate, restricted resource). Rights record per paper (owner, licence, evidence; O-18). Prepares the Phase 7 store.

**Frontend**
- [ ] P5.F1 Content studio (Staff Console → Content, and Teacher Arena → My contributions): create, classify, upload, preview, submit, respond to review.
- [ ] P5.F2 Review queue and review screen (side-by-side preview, checklist, comments, decision).
- [ ] P5.F3 Learner library: subject → topic pages, resource cards, search with filters, resource viewer (PDF viewer with watermark, video player, worksheet download), premium lock states with upgrade prompt.

**Exit criteria (Milestone M3):** E2E: teacher drafts → reviewer requests changes → teacher revises → reviewer approves → scheduled publish → learner finds it via search and views it; premium file unreachable without entitlement (tested at API level with direct URLs); every lifecycle action audited.

---

### Phase 6 — Learning workspace, assessments & progress (4 weeks) · M15–M17

- [ ] P6.1 Learner workspace: dashboard (continue learning, upcoming classes, assigned work, recent activity), my subjects, bookmarks, recently viewed, learning history.
- [ ] P6.2 Activity tracking (resource opened/completed, time on task — privacy-aware) and topic progress.
- [ ] P6.3 Question bank: questions with types (multiple choice, multiple select, true/false, short answer, long answer, matching, ordering), options, marking schemes, difficulty, taxonomy tags; versioned.
- [ ] P6.4 Assessments: topical exercises, timed tests, mock exams; assembly from the bank; publishing via the content review workflow; availability windows and attempt limits.
- [ ] P6.5 Attempts: server-side timer enforcement, autosave, submission, automatic marking for objective types, **manual grading queue** for subjective answers, results with feedback and answer review (when policy allows).
- [ ] P6.6 Progress analytics: per subject/topic mastery, assessment performance, timeline; role-controlled visibility (learner, linked guardian, assigned teacher, staff).
- [ ] P6.7 Frontend: learner dashboard, assessment player (mobile-first, resilient to connection drops), results; Teacher Arena grading inbox and grading screen; Family portal progress views.

**Exit criteria:** E2E: learner takes a timed assessment on a phone-sized viewport → objective items auto-marked → teacher grades the essay → learner and guardian see results; no mock data anywhere in learning views.

---

### Phase 7 — Plans, entitlements, manual payments & exam papers store (6 weeks) · M18–M20, M37

**Gates:** O-03, O-04, O-09, O-15, O-16, O-17, O-18. Specifications: [manual Paybill payments](decisions-and-business-rules.md#4-manual-paybill-payments--functional-specification) · [exam papers store](decisions-and-business-rules.md#6-exam-papers-store--functional-specification) · [marking-scheme requests](decisions-and-business-rules.md#7-marking-scheme-requests--functional-specification).

**Part A — Commerce core (weeks 1–4)**

- [ ] P7.1 Plans as data (audience, ISO-8601 duration, price in KES, entitlement set); plan management in Staff Console; plan comparison page data.
- [ ] P7.2 Entitlement engine: grant, consume (credits), expire, revoke; `HasEntitlement` checks used by content (P5.3) and booking (P8).
- [ ] P7.3 Orders and invoices with references (`PHX-…`); invoice PDF; expiry of unpaid invoices.
- [ ] P7.4 Payment submissions (code, amount, phone, time, optional proof) with idempotency and global uniqueness of M-PESA codes; **finance notification on every submission**.
- [ ] P7.5 Finance queue: confirm / reject / partial, separation of duties, SLA reminders; confirmation activates subscription and entitlements in one transaction; receipts.
- [ ] P7.6 Subscriptions: activation, expiry job, renewal reminders, cancellation, suspension.
- [ ] P7.7 Teacher onboarding fee and licence through the same flow; activates Teacher status `Active` (completes Phase 3's pending step).
- [ ] P7.8 Refunds (recorded, audited, entitlement effects) and reconciliation report against Paybill statements; exports (CSV).
- [ ] P7.9 `IPaymentProvider` abstraction with `ManualPaybillProvider`; contract tests ready for Daraja later.
- [ ] P7.10 Frontend: pricing/plan selection, checkout with Paybill instructions, payment confirmation form, payment status and history, receipts; guardian paying for linked learners; Staff Console → Finance (queue, payment detail, invoices, refunds, reconciliation, reports).

**Part B — Exam Papers Store & marking schemes (weeks 5–6) · M37**
- [ ] P7.11 Store catalogue: store products over exam-paper resources (single paper, subject bundle, grade/term bundle, "full set" bundle); price in KES per product (O-15); publish/unpublish; featured and new; product pages with a **free preview** (cover page and instructions only, watermarked).
- [ ] P7.12 Store search and filters: grade, subject, exam type, year, term; sort by newest/popular; SEO-friendly public pages (server-rendered, structured data).
- [ ] P7.13 Buyers: learners, guardians, teachers and members of the public. Checkout requires a verified **buyer account** (lightweight: name, email, phone, county), per O-17. Cart with several papers and bundles; one invoice per order.
- [ ] P7.14 Payment through the Part A manual Paybill flow. On confirmation, an item-level entitlement (`store.paper:{id}`) is granted to the buyer and receipt sent.
- [ ] P7.15 Fulfilment: "My purchases" library; download generates a **personalised, watermarked PDF** on the server (buyer name, email/phone, order number, date on every page) from the private master; download limit and expiring links per policy; in-browser viewing; every download logged.
- [ ] P7.16 Marking-scheme requests: request from a purchased paper (or bundle); policy engine per O-16 (included / priced / approval required / release date / role-based, e.g. verified teachers and guardians); staff review queue (Academic Reviewer or Content Manager) with approve/decline and reason; payment step when priced; fulfilment as watermarked PDF; requester notified at each step; full audit.
- [ ] P7.17 Store operations: Staff Console → Store (products, bundles, pricing, featured, sales, downloads, marking-scheme queue); refunds for store orders through Part A refunds; abuse controls (download anomalies, shared-link detection, rate limits).
- [ ] P7.18 Institutional buyers (schools, tuition centres, homeschool groups): organisation account, bulk order with quantity-based licence and invoice, multiple named downloaders. Ships if O-15 confirms institutional pricing; otherwise moved to Phase 12.
- [ ] P7.19 Store reports: sales by paper, subject, grade, county and period; conversion from preview to purchase; marking-scheme request volumes and turnaround.
- [ ] P7.20 Frontend: public store (`/exams`, product pages, cart, checkout), buyer library (`/account/purchases`), marking-scheme request flow and status; Staff Console store and request queue.

**Exit criteria (Milestones M4 and M4b):**
- E2E: guardian buys a plan for a learner → submits M-PESA code → finance admin is notified and confirms → learner immediately accesses premium content.
- E2E: a member of the public finds a Grade 8 Mathematics end-of-term paper → previews → buys it with a bundle → finance confirms → buyer downloads a watermarked PDF → requests the marking scheme → reviewer approves (and payment confirmed if priced) → buyer downloads the watermarked marking scheme.
- A purchased file cannot be downloaded by another account or after its link expires; duplicate M-PESA code is rejected; finance cannot confirm their own payment; expiry job removes plan access; every financial and fulfilment action audited and reconcilable.

---

### Phase 8 — Virtual classes, booking & attendance (4 weeks) · M21–M24

**Gate:** O-05. Specification: [Google Meet manual links](decisions-and-business-rules.md#5-google-meet-manual-links--functional-specification).

- [ ] P8.1 Class offerings (1:1 and group), capacity, duration, subject/topic, teacher assignment, materials (linked resources), status.
- [ ] P8.2 Teacher availability: weekly rules, exceptions, blocked periods, Africa/Nairobi time handling; conflict detection.
- [ ] P8.3 Booking state machine (Requested → Confirmed → Scheduled → Completed / Cancelled / No-show) with **entitlement or credit check at confirmation**, transactional capacity control (row lock / constraint) so the last seat can't be double-booked, and conflict checks against availability **and** pending bookings.
- [ ] P8.4 Rescheduling and cancellation rules (O-05) with credit effects.
- [ ] P8.5 Sessions: manual Meet link entry and validation; link visible only to entitled participants; reminders (24 h, 2 h, 15 min) and "link missing" reminders.
- [ ] P8.6 Join through the platform (join events), attendance marking by teacher, session notes, learner feedback/ratings (if approved), no-show handling, incident flag to safeguarding.
- [ ] P8.7 Teacher earnings ledger entries per completed session (payouts manual per O-03).
- [ ] P8.8 Frontend: class discovery and booking (learner/guardian), my schedule (calendar + list), session page with join button; Teacher Arena availability editor, class management, attendance and notes; Staff Console → Classes (schedule, assignments, interventions).

**Exit criteria (Milestone M5):** E2E: learner with credits books a 1:1 → teacher adds Meet link → reminders sent → learner joins via platform → teacher marks attendance → learner leaves feedback → credit consumed and earnings recorded; concurrent last-seat booking test passes.

---

### Phase 9 — Communication, support & collaboration (3 weeks) · M25–M27

**Gate:** O-08.

- [ ] P9.1 Notification centre: in-app inbox, email channel, templates per event, user preferences, digest options; SMS channel interface (disabled until provider chosen).
- [ ] P9.2 Announcements: targeted by role, grade, class; scheduled; read tracking.
- [ ] P9.3 Support desk: tickets with categories, priority, SLA, assignment, internal notes, escalation, attachments (validated), satisfaction rating; contextual "Get help" from any screen.
- [ ] P9.4 Teacher collaboration: contribution workspace on shared drafts, comments, attribution and contribution history; class-scoped Q&A visible to staff (no free-form learner DMs at launch).
- [ ] P9.5 Frontend: notification bell and centre, preferences; Support (all users) and Staff Console → Support queue; collaboration views in Teacher Arena.

**Exit criteria:** every event listed in the notification matrix is delivered in-app and by email in staging; support ticket lifecycle E2E; preferences respected.

---

### Phase 10 — Administration, reporting, audit & safeguarding (3 weeks) · M28–M31

- [ ] P10.1 Administration: system settings (typed, validated, audited), feature flags, maintenance mode, organisation profile, notification templates editor.
- [ ] P10.2 Reporting: management dashboard (learners, teachers, content, subscriptions, revenue, exam paper sales, classes, attendance, assessments), report pages with filters and CSV export, export jobs for large reports; read models where queries are heavy.
- [ ] P10.7 National operations view: learners, guardians, teachers, buyers, sales and class activity **by county** (from profile county), teacher coverage per subject and county, service-level indicators (payment confirmation time, review turnaround, support response) so Phronesis can run and grow operations across Kenya.
- [ ] P10.3 Audit explorer: search by actor, entity, action, date; entity history views (teacher, resource, payment); retention policy job.
- [ ] P10.4 Data-subject rights (Kenya Data Protection Act): access/export and deletion requests workflow with approval, anonymisation strategy that preserves financial/audit records.
- [ ] P10.5 Safeguarding: "Report a concern" on learner/teacher surfaces, concern triage by Safeguarding Lead, user restrictions and blocks, content takedown, incident records, escalation rules; security incident workflow runbook.
- [ ] P10.6 Frontend: Staff Console dashboard and reports, audit explorer, safeguarding workspace, settings.

**Exit criteria (Milestone M6):** management can answer "how many active paying learners, revenue this month, classes delivered, content published, open concerns" from the console without developer help; a concern can be raised, triaged and actioned with full audit.

---

### Phase 11 — Public website, hardening & launch (3 weeks)

**Gates:** O-13 (legal texts), O-14.

**P11.1 Public website**
- [ ] P11.1.1 Home, About (mission, values, Christ-centred education), How it works (learners, guardians, teachers), Subjects & grades (from taxonomy, with resource previews), **Exam papers store** entry points, Pricing (from plans), Live classes, Apply to teach, FAQ, Contact, Legal (terms, privacy, refund, safeguarding, exam-paper licence terms, cookie notice).
- [ ] P11.1.2 SEO: metadata, Open Graph images in the new brand, sitemap, robots, structured data (EducationalOrganization).
- [ ] P11.1.3 Performance budgets met on simulated 4G.

**P11.2 Hardening**
- [ ] P11.2.1 Security review against OWASP ASVS L2 checklist; OWASP ZAP baseline scan; fix all high/medium findings.
- [ ] P11.2.2 Load test (k6) at 3× expected launch traffic on staging-sized resources; tune PostgreSQL and caching.
- [ ] P11.2.3 Accessibility audit (WCAG 2.2 AA) of the five core journeys.
- [ ] P11.2.4 Backup restore drill from production-like data; disaster recovery runbook with RPO 24 h / RTO 4 h.
- [ ] P11.2.5 Alert routing tested end to end.

**P11.3 Launch readiness**
- [ ] P11.3.1 Production data: Super Admin and staff accounts via invitation; plans, taxonomy, initial content loaded and reviewed.
- [ ] P11.3.2 User acceptance testing with Phronesis staff, a few teachers, guardians and learners; sign-off per journey.
- [ ] P11.3.3 Staff training: finance confirmations, reviews, support, safeguarding (short guides in `docs/runbooks/`).
- [ ] P11.3.4 Release `v1.0.0`; go-live checklist; two weeks of **hypercare** with daily checks of errors, payment queue and support tickets.

**Exit criteria (Milestone M7):** all five core journeys pass E2E on production; monitoring, alerts and backups verified in production; UAT signed off; launch announced.

---

### Phase 12 — Post-launch roadmap (prioritised after launch data)

| Item | Module | Trigger |
|---|---|---|
| PWA (installable, offline shell, push notifications) | M34 | After launch; mobile usage data |
| M-PESA Daraja STK Push replacing manual confirmation | M19 | Payment volume makes manual confirmation slow |
| SMS notifications (reminders, payment confirmations) | M26 | Provider chosen |
| Google Calendar API for automatic Meet links and attendance | M23 | Google Workspace available |
| Teacher payouts automation | M20 | O-03 business model settled |
| Advanced learner analytics, weak-topic detection, recommendations | M35 | Enough activity data |
| OpenSearch, RabbitMQ, separate worker, larger VPS | Platform | Measured load |
| Native mobile apps | M34 | PWA limits reached |
| Additional curricula, currencies, languages | M36 | International expansion |

---

## 5. Cross-cutting tracks (run through all phases)

| Track | Ongoing responsibilities |
|---|---|
| Security | Threat model updated per module; dependency updates weekly; secrets rotation; permission matrix review each phase |
| Quality | Coverage gates; E2E suite grows per milestone; flaky tests fixed within a day |
| Design & UX | Each module's screens designed in the design system before build; usability check with a real user per milestone |
| Content readiness | Phronesis prepares taxonomy (before P4), launch content (during P5–P11), plans (before P7) |
| Documentation | Module specs, API spec status, runbooks, CHANGELOG kept current |
| Operations | Weekly staging demo; monthly restore drill; capacity review (CPU, RAM, disk) |

## 6. Risk register

| Risk | Likelihood | Impact | Mitigation |
|---|---|---|---|
| Open business decisions delay phases | High | High | Gates with documented defaults; decision review at every exit review |
| Single VPS capacity or failure | Medium | High | Memory budget, load tests, off-box backups, tested restore, documented scale-up path |
| Manual payment confirmation becomes a bottleneck | Medium | Medium | SLA reminders, queue metrics, Daraja in Phase 12 behind the same interface |
| Premium content leakage | Medium | High | Signed URLs, watermarking, access logs, rate limits, legal terms |
| Safeguarding incident involving a minor | Low | Very high | Verification, no learner DMs, report-a-concern, safeguarding lead, audit |
| Scope creep | High | High | Phase exit criteria; new ideas go to Phase 12 backlog unless they unblock MVP |
| Video costs/bandwidth | Medium | Medium | External streaming (O-07); launch possible with documents and assessments only |
| Exam paper copyright (e.g. KNEC past papers) | Medium | High | Rights record per paper (O-18); sell only papers Phronesis owns or is licensed to sell; takedown process |
| Purchased papers shared freely | High | Medium | Per-buyer watermarking, download limits, expiring links, anomaly detection, licence terms |
| Marking schemes reaching learners before they attempt papers | Medium | Medium | Request policy with approval and release rules (O-16); audit of every release |
| Raster-only logo hurts brand quality | Medium | Low | Vector master before Phase 11 (O-12) |
| Single developer availability | Medium | High | Documentation, runbooks, tests and ADRs so another developer can continue |

## 7. Tracking and governance

Progress is tracked as described in [progress tracking](progress-tracking.md):
- Work items are ticked here only when they meet the Definition of Done and have been demonstrated on staging.
- `node scripts/plan-progress.mjs` regenerates [PROGRESS.md](PROGRESS.md) (overall %, per-phase %, milestones, gates due); CI fails if it is stale.
- Each work item mirrors to a GitHub issue in the "Phronesis Delivery" project (`scripts/plan-sync-github.mjs`).
- A weekly status report goes in `docs/plan/status/`, and an exit review per phase in `docs/plan/reviews/`.
- Any change in scope or order is recorded as a decision (D-xxx) before it happens.
