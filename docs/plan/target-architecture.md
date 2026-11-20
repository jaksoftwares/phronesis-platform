# Target Architecture — the System Phronesis Needs

This document describes the system we are building: what it does, how it is structured, how it runs on the Hostinger KVM 2, and the engineering standards every module follows. The [implementation plan](implementation-plan.md) builds it phase by phase. Product rules live in [decisions and business rules](decisions-and-business-rules.md).

---

## 1. What the system is

Phronesis is a **national online homeschool**: a digital operating platform for Kenyan CBC learners (Grades 7–12), their guardians, verified teachers, exam-paper buyers and Phronesis staff. It is not a content website: it runs the organisation's core processes end to end, for users in every county, on low-cost phones and constrained networks.

### 1.1 Core value chain (MVP)

```
Verified teachers ─┐
                   ├─► Structured CBC content ─► Learner study & assessments ─► Progress visible to guardians
Phronesis staff ───┘            │
                                └─► Paid access (plans → invoice → Paybill → finance confirmation → entitlements)
                                └─► Exam papers store (buy papers → watermarked download → marking-scheme request)
                                └─► Live classes (availability → booking → Meet link → attendance → feedback)
Everything ─► audited · notified · supported · safeguarded · reported
```

### 1.2 Surfaces (web applications)

| Surface | Audience | Route prefix | Purpose |
|---|---|---|---|
| Public website | Everyone | `/` | Brand, subjects and resource previews, pricing, how it works, apply to teach, legal pages, contact |
| Exam papers store | Learners, guardians, teachers, schools, the public | `/exams` | Browse and preview papers, cart, checkout (Paybill), "My purchases" (`/account/purchases`), marking-scheme requests |
| Learner portal | Learners | `/learn` | Dashboard, subjects → topics → resources, assessments, classes, progress, plan & payments, support |
| Guardian portal | Parents/guardians | `/family` | Linked learners, consent, progress, attendance, payments, messages from school |
| Teacher Arena | Verified teachers (and applicants) | `/teach` | Application & onboarding, profile, availability, classes, content contributions, grading, earnings |
| Staff Console | Internal roles | `/staff` | Permission-driven sections: Admin, Finance, Academic review, Content, Support, Safeguarding, Reports, Settings |
| Account | All signed-in users | `/account` | Profile, security (password, MFA, sessions), notifications, agreements |

One login (`/login`) routes each user to their surface by role (D-010). A future mobile app consumes the same API.

### 1.3 Domain modules

| Module | Responsibility | Blueprint |
|---|---|---|
| Identity | Users, credentials, sessions, verification, MFA, security events | M03 |
| Access | Roles, permissions, policies, resource-level authorization | M04 |
| Organization | Staff profiles, departments, invitations | M05 |
| People | Learner and guardian profiles, links, consent | M06 |
| Teachers | Teacher profiles, competences, availability, status | M07 |
| TeacherCompliance | Applications, documents, reviews, interviews, agreements, suspension | M08 |
| Academic | Curricula, versions, levels, grades, subjects, strands, sub-strands, objectives, prerequisites | M09–M10 |
| Content | Resources, versions, classification, access rules, licensing, review workflow, publishing | M11–M12 |
| Media | File assets, upload validation, private storage, signed access, watermarking, video references | M13 |
| Discovery | Full-text search, filters, featured/recent/popular | M14 |
| Learning | Enrolments, learner subjects, activity, progress, bookmarks | M15, M17 |
| Assessment | Question banks, questions, assessments, attempts, marking, results | M16 |
| Commerce | Plans, entitlements, subscriptions, orders, invoices, receipts, refunds | M18, M20 |
| ExamStore | Store products and bundles over exam-paper resources, cart, buyer library, personalised watermarked fulfilment, marking-scheme requests, institutional licences | M37 |
| Payments | Payment submissions, provider abstraction (Manual Paybill → Daraja), reconciliation | M19 |
| Classes | Class offerings, sessions, teacher assignment, materials, meeting links | M21, M23 |
| Booking | Availability rules, bookings, credits, attendance, notes, feedback | M22, M24 |
| Collaboration | Teacher contributions, comments, class Q&A | M25 |
| Communication | Notifications (in-app, email, SMS later), templates, preferences, announcements | M26 |
| Support | Tickets, messages, assignment, SLA | M27 |
| Safeguarding | Concern reports, restrictions, moderation, incidents | M31 |
| Administration | Settings, feature flags, staff actions | M28 |
| Reporting | Operational and management reports, exports | M29 |
| Audit | Audit log, data-subject requests, retention | M30 |

Modules talk to each other through **application-level contracts or domain events**, never by reaching into another module's tables or internal classes. Architecture tests enforce this.

---

## 2. Technology choices

| Concern | Choice | Notes |
|---|---|---|
| Backend runtime | .NET 10, ASP.NET Core | Existing choice; LTS |
| Database | PostgreSQL 17 | D-001; one database, one schema per module |
| ORM | EF Core 10 + Npgsql | Snake-case naming convention; migrations in Git |
| Background jobs | Hangfire with PostgreSQL storage | D-013; runs inside the API process at launch |
| Async integration | Transactional outbox table → Hangfire dispatcher | RabbitMQ later if needed |
| Search | PostgreSQL full-text + `pg_trgm` | D-012 |
| File storage | `IFileStorage` abstraction: private disk volume (outside web root) at launch; S3-compatible adapter (Cloudflare R2 / Backblaze B2) when volume grows | Files are never served directly |
| Video | External streaming service with signed playback (O-07) | Not on the VPS |
| Email | Transactional provider over SMTP/API (O-11) | Mailpit in development |
| Validation | FluentValidation | |
| Mapping | Explicit mapping or Mapperly | D-011 |
| Command/query handling | Thin in-house handlers + decorators (validation, transaction, audit, logging) | D-011 |
| API docs | Built-in OpenAPI + Scalar UI | OpenAPI is the contract |
| Logging/telemetry | Serilog (JSON, redaction) + OpenTelemetry (traces, metrics) exported off-box | Grafana Cloud free tier or similar; nothing heavy on the VPS |
| Error tracking | Sentry-compatible SDK (Sentry or GlitchTip) | Backend and frontend |
| Frontend | Next.js 16 (App Router), React 19, TypeScript strict | `proxy.ts` (not `middleware.ts`) |
| Styling/UI | Tailwind CSS v4 + Radix primitives (shadcn/ui pattern, owned components) | Brand tokens from the design system doc |
| Data fetching | TanStack Query + **generated typed client** from OpenAPI (orval or openapi-typescript) | No hand-written fetch calls in pages |
| Forms | React Hook Form + Zod | Zod schemas generated or shared from the contract |
| Testing (backend) | xUnit, Testcontainers (PostgreSQL), Respawn, NetArchTest, Verify for snapshots | |
| Testing (frontend) | Vitest + Testing Library, MSW (tests only), Playwright E2E, axe for accessibility | |
| CI/CD | GitHub Actions → images on GitHub Container Registry → SSH deploy with Docker Compose | |
| Reverse proxy/TLS | Caddy (automatic HTTPS) | |

---

## 3. Deployment on Hostinger KVM 2

**Server:** 2 shared vCPU, 8 GB RAM, 100 GB NVMe, 8 TB/month bandwidth (D-007).

```
                 Internet
                    │ 443 (HTTPS, HTTP/3)
              ┌─────▼─────┐
              │   Caddy   │  TLS, security headers, compression, rate limit backstop
              └──┬─────┬──┘
     phronesishomeschool.com   api.phronesishomeschool.com
          ┌──────▼───┐     ┌──────▼────────────┐
          │ web      │     │ api               │  ASP.NET Core + Hangfire server
          │ Next.js  │────►│ (stateless)       │
          └──────────┘     └──┬─────────┬──────┘
                              │         │
                     ┌────────▼──┐  ┌───▼──────────────┐
                     │PostgreSQL │  │ /data/files      │ private volume (uploads)
                     └───────────┘  └──────────────────┘
   Off-box: nightly encrypted backups (DB + files) → object storage · telemetry → hosted Grafana/Sentry · uptime → external monitor
```

### 3.1 Memory budget (production + small staging on the same VPS)

| Container | Limit | Notes |
|---|---|---|
| Caddy | 128 MB | |
| web (prod) | 768 MB | Next.js standalone output |
| api (prod) | 1 GB | Includes Hangfire workers (2) |
| PostgreSQL (shared by prod + staging DBs) | 2.5 GB | `shared_buffers` ≈ 1 GB, tuned with pgtune |
| web + api (staging) | 1 GB total | Scaled down; can be stopped when idle |
| OS, Docker, backups, headroom | ~2.5 GB | |

ClamAV (antivirus scanning of uploads) needs ~1–1.5 GB; enable it if memory allows after load testing, otherwise scan via an external service. Scale-up path: move PostgreSQL to a second VPS or managed instance → KVM 4 → separate worker container.

### 3.2 Environments

| Environment | Where | Data | Deploys |
|---|---|---|---|
| Local | Developer machine, `docker compose up` (Postgres, Mailpit) | Seeded fake data | — |
| CI | GitHub Actions | Testcontainers | Every push / PR |
| Staging | Same VPS, separate compose project and database, `staging.` subdomains, basic-auth protected | Seeded demo data, never production copies | Automatically when the owner pushes `test` |
| Production | VPS | Real | When the owner pushes a tagged `main`, with manual approval |

Branches: local work branches → `dev` (integration, CI only) → `test` (staging) → `main` (production). Only the repository owner merges and pushes `dev`, `test` and `main`.

### 3.3 Operations baseline

- Server hardening: SSH keys only, no root login, `ufw` (22/80/443), fail2ban, unattended security upgrades, Docker rootless or non-root containers.
- Secrets: `.env` files on the server (permissions 600) created from `.env.example`; never in Git; rotated on staff changes.
- Backups: nightly `pg_dump` (custom format) + files rsync → encrypted off-box object storage; 7 daily, 4 weekly, 6 monthly; **monthly restore drill** into staging, recorded in the runbook.
- Health: `/health/live` (process) and `/health/ready` (PostgreSQL, storage, Hangfire); external uptime monitor alerts by email/WhatsApp-capable channel.
- Deploy: zero-downtime best effort (pull images → run migrations job → restart api/web); rollback = previous image tag + migration rollback script reviewed in the PR.

---

## 4. Backend architecture

### 4.1 Solution layout

```
phronesis-platform/
├── src/
│   ├── Phronesis.Api/                 # HTTP only: endpoints, auth wiring, OpenAPI, middleware
│   │   └── Endpoints/<Module>/        # thin endpoint groups, one file per resource
│   ├── Phronesis.Application/         # use cases
│   │   ├── Abstractions/              # ICommandHandler, IQueryHandler, ICurrentUser, IClock, IFileStorage…
│   │   ├── Behaviours/                # validation, transaction, audit, logging decorators
│   │   └── <Module>/<UseCase>/        # Command|Query + Handler + Validator + Response
│   ├── Phronesis.Domain/
│   │   ├── Common/                    # Entity, AggregateRoot, ValueObject, DomainEvent, Result, Money
│   │   └── <Module>/                  # aggregates, value objects, domain events, invariants
│   ├── Phronesis.Infrastructure/
│   │   ├── Persistence/               # DbContext, interceptors (audit, outbox, timestamps), configurations/<Module>
│   │   ├── Jobs/                      # Hangfire jobs, outbox dispatcher
│   │   ├── Storage/ Email/ Payments/ Meetings/ Search/
│   │   └── DependencyInjection.cs
│   └── Phronesis.Shared/              # error codes, contracts shared with clients (minimal)
├── tests/
│   ├── Phronesis.Domain.Tests/
│   ├── Phronesis.Application.Tests/
│   ├── Phronesis.Api.Tests/           # integration: Testcontainers PostgreSQL + WebApplicationFactory
│   └── Phronesis.Architecture.Tests/
├── deploy/                            # docker-compose.{dev,staging,prod}.yml, Caddyfile, postgres config, backup scripts
├── docs/                              # see §9
├── .github/workflows/
├── Directory.Build.props              # nullable, warnings as errors, analyzers
├── Directory.Packages.props           # central package versions
└── Phronesis.slnx
```

The existing four-project split is kept; the change is that **every layer is organised by module**, and the API layer contains no business logic.

### 4.2 Request flow and standards

```
HTTP → correlation ID → security headers → rate limit → authentication → endpoint
     → authorization policy (fallback = deny) → command/query
     → [validation → transaction → audit → logging] decorators → handler → domain → EF → outbox
     → response (envelope or ProblemDetails)
```

- **Every endpoint declares its policy**; an architecture test fails the build if one does not. `AllowAnonymous` is an explicit, reviewed exception.
- **One `ICurrentUser`** (user ID, roles, permissions, learner/teacher/guardian profile IDs) — the only way code learns who is calling. Fixes C-02 and H-02 by design.
- **Resource-level authorization** in handlers (e.g. guardian ↔ learner link, author ≠ reviewer, booking owner).
- **Errors**: RFC 9457 ProblemDetails with a stable `code` (e.g. `content.not_found`, `payments.duplicate_transaction_code`) and `traceId`. Validation errors list field errors.
- **Success responses**: `{ "data": …, "meta": { pagination } }` for collections; resource body for single items. One convention, enforced by shared helpers.
- **IDs**: UUID v7, never sequential IDs in URLs.
- **Time**: `timestamptz` in UTC; Africa/Nairobi conversions in presentation and scheduling rules.
- **Money**: `numeric(19,4)` + ISO currency in a `Money` value object.
- **Concurrency**: PostgreSQL `xmin` row version on aggregates edited concurrently; `If-Match` for edit endpoints where users collide (content, schedules).
- **Idempotency**: `Idempotency-Key` header on payment submissions, bookings and other retry-sensitive commands.
- **Auditing**: every command marked `[Audited]` writes actor, action, entity, before/after (redacted), IP, correlation ID — in the same transaction.
- **Domain events + outbox**: state changes raise events (e.g. `PaymentConfirmed`, `ResourcePublished`); the outbox guarantees notifications and projections happen exactly once after commit.
- **Seeding**: reference data (roles, permissions, plans, CBC taxonomy) via idempotent seeders per environment; demo data only in local/staging; no hard-coded passwords anywhere.

### 4.3 Data architecture

- One PostgreSQL database; schemas: `identity`, `access`, `org`, `people`, `teachers`, `compliance`, `academic`, `content`, `media`, `learning`, `assessment`, `commerce`, `payments`, `classes`, `booking`, `store`, `collab`, `comms`, `support`, `safeguarding`, `admin`, `audit`, `jobs` (Hangfire), `outbox`.
- `snake_case`, plural tables; FK naming per the DB spec; check constraints for statuses; unique indexes for business keys (email, admission number, M-PESA transaction code, invoice number).
- Historical records are archived, not deleted (content versions, payments, verification, audit).
- Personal data inventory maintained (`docs/specs/data-inventory.md`) for the Data Protection Act: purpose, retention, access roles.

### 4.4 Security architecture

| Area | Design |
|---|---|
| Passwords | BCrypt (work factor tuned to ~250 ms on the VPS); breached/weak password checks; lockout with backoff |
| Tokens | Access JWT (10 min) kept **in memory** in the browser. Refresh token: random 256-bit, **hashed** in DB, rotated on every use with reuse detection (reuse revokes the session family), sent only as `HttpOnly; Secure; SameSite=Lax` cookie on the API domain. Never in a response body. |
| Sessions | Listed and revocable by the user; all revoked on password reset or suspension |
| MFA | TOTP mandatory for all staff roles; optional for teachers/guardians; recovery codes |
| Verification/reset tokens | Random, hashed, single-use, short expiry |
| Authorization | Permission codes per role, fallback deny, resource checks in handlers; permission matrix tests |
| Transport | HTTPS only, HSTS, TLS via Caddy |
| Headers | CSP (nonce-based on web), X-Content-Type-Options, frame-ancestors none, Referrer-Policy, Permissions-Policy |
| Rate limiting | ASP.NET rate limiter: login, register, reset, payment submission, uploads, search; Caddy backstop |
| Uploads | Allow-list by type per use (documents, images, payment proofs); magic-byte sniffing; size limits; server-generated names; private storage; optional AV scan; images re-encoded to strip metadata |
| Content protection | Short-lived signed URLs streamed via API; per-user watermark overlay on premium PDFs; download only where policy allows; access logged |
| Sold exam papers | Server-side personalised PDF generated per buyer from a private master (buyer name, contact, order number stamped on every page) using an MIT/Apache-licensed PDF library (e.g. PDFsharp), never an AGPL one; generated copies cached briefly then deleted; download counts and anomaly alerts |
| Logging | Request bodies never logged; PII and secrets redacted by Serilog enrichers |
| Minors | Guardian consent gate (O-02), no free-form learner DMs at launch, report-a-concern, teacher conduct agreement, safeguarding audit trail |
| Supply chain | Dependabot/Renovate, `dotnet list package --vulnerable` and `npm audit` gates in CI (no high/critical) |

### 4.5 Integrations (all behind interfaces)

| Interface | Launch implementation | Later |
|---|---|---|
| `IPaymentProvider` | `ManualPaybillProvider` (submission + finance confirmation) | `DarajaStkPushProvider` |
| `IMeetingProvider` | `ManualMeetLinkProvider` (validated link) | `GoogleCalendarMeetProvider` |
| `IEmailSender` | SMTP/API provider; Mailpit in dev | — |
| `ISmsSender` | Not enabled | Africa's Talking or similar |
| `IFileStorage` | Private disk volume | S3-compatible (R2/B2) |
| `IVideoHost` | External streaming service with signed URLs (O-07) | — |
| `ISearchIndex` | PostgreSQL FTS | OpenSearch |

### 4.6 Scheduled jobs

Outbox dispatch (every few seconds) · subscription and entitlement expiry (hourly) · class reminders (24 h / 2 h / 15 min) · missing Meet link reminders · unconfirmed payment SLA reminders · invoice expiry · stale upload cleanup · audit/log retention · daily ops digest to admins · backup verification check.

---

## 5. Frontend architecture

### 5.1 Structure

```
phronesis-web/src/
├── app/
│   ├── (public)/            # marketing, server-rendered, SEO
│   ├── (auth)/              # login, register, apply, verify, reset
│   ├── learn/  family/  teach/  staff/  account/   # authenticated surfaces, each with layout + guard
│   ├── api/                 # only if a backend-for-frontend route is truly needed
│   └── layout.tsx, error.tsx, not-found.tsx
├── features/<module>/       # components, hooks, schemas, pages' building blocks per business feature
├── components/ui/           # design-system primitives (Button, Input, Dialog, Table, Badge, Toast…)
├── components/layout/       # shells: PublicHeader, PortalShell, StaffShell, navigation
├── lib/api/                 # generated client (do not edit), query client, auth/session handling
├── lib/                     # utils, formatting (KES, dates in Africa/Nairobi), permissions helpers
├── styles/                  # tokens.css (from design system), globals
└── proxy.ts                 # optimistic redirect only; real checks are server-side
```

### 5.2 Standards

- **Only finished features are reachable.** Navigation and routes are built from the modules enabled by server-side feature flags; an unreleased module returns 404 and has no menu entry. No "coming soon" UI, placeholder pages or mock data in shipped code (implementation plan §3.5, CI-enforced).
- **Server components** for public and read-mostly pages; client components only where interaction needs them.
- **Generated API client + TanStack Query**: one query key factory per feature; no `axios` calls in pages.
- **Session**: on load the app calls `/auth/refresh` (cookie) to obtain an in-memory access token, then `/auth/me` for user, roles and permissions. `proxy.ts` only redirects obviously signed-out users; each surface layout checks role/permission and renders a 403 page otherwise. The API remains the authority.
- **Permission-aware UI**: navigation and actions rendered from the permission list; hidden ≠ secure, so the API also enforces.
- **States**: every data view has designed loading (skeletons), empty, error (with retry) and no-permission states. No `alert()`; toasts and inline messages from the design system.
- **Forms**: RHF + Zod, server errors mapped to fields from ProblemDetails.
- **Accessibility**: WCAG 2.2 AA: keyboard paths, focus management, labels, contrast checked against the new palette; axe checks in CI.
- **Performance budgets** (constrained networks): public pages LCP < 2.5 s on 4G, JS < 170 KB gzip per route, images via `next/image` with AVIF/WebP, fonts self-hosted with `next/font`.
- **Formatting**: KES currency, `en-KE` locale, Africa/Nairobi time zone everywhere.
- **Quality gates**: `tsc --noEmit`, ESLint (zero errors), Prettier, Vitest, Playwright smoke on every PR.

---

## 6. API conventions (summary)

`/api/v1` · plural nouns for resources · explicit command endpoints for state transitions (`POST /resources/{id}/submit-review`) · `page`/`pageSize` + `sortBy`/`sortDir` · filters as query params · ProblemDetails errors with codes · `Idempotency-Key` for retry-sensitive commands · `ETag`/`If-Match` on collaborative edits · OpenAPI published per build and versioned in Git (`docs/api/openapi.v1.json`); breaking changes need a new version or a deprecation window.

The endpoint inventory in `docs/specs/api-endpoint-specification.md` is the target; each phase implements its slice and marks it done there.

---

## 7. Quality strategy

| Level | What | Gate |
|---|---|---|
| Domain unit tests | Invariants and state machines (teacher application, content workflow, booking, payment submission) | ≥ 90 % line coverage of Domain |
| Application tests | Handlers with real PostgreSQL (Testcontainers) | ≥ 80 % of Application |
| API tests | Every endpoint: happy path, validation, **authorization matrix** (each role × endpoint expected status) | 100 % of endpoints have an authorization test |
| Architecture tests | Layer and module boundaries; every endpoint has a policy; no `DbContext` in Api | Must pass |
| Frontend unit/component | Features, forms, permission rendering | Critical features covered |
| E2E (Playwright) | The five core journeys (§1.1) on staging | Must pass before release |
| Non-functional | Load test (k6) on staging for catalog, login, payment submission; security scan (OWASP ZAP baseline); accessibility audit | Before launch and each major release |

---

## 8. Observability

Structured logs with correlation IDs (API and web) · OpenTelemetry traces and metrics exported to a hosted backend · error tracking for API and web · business metrics: registrations, payment submissions awaiting confirmation, confirmation time, bookings, attendance, content published · alerts: API error rate, readiness failing, backup failure, disk > 80 %, payment queue older than SLA.

---

## 9. Documentation structure (target)

```
docs/
├── README.md                 # index and rules: which document is the source of truth for what
├── product/                  # problem definition, system modules blueprint
├── specs/                    # database spec, API spec, permission matrix, data inventory, module specs
├── plan/                     # implementation plan, target architecture, decisions & business rules
├── adr/                      # architecture decision records (0001-postgresql.md …)
├── audit/                    # audits and reviews (dated)
├── runbooks/                 # deploy, rollback, backup/restore, incident, onboarding a developer
└── archive/                  # superseded documents (legacy walkthrough reports, old plans)
```

The web repository keeps only frontend-specific documents (design system, frontend conventions) and links here for everything else.
