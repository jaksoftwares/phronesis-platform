# Phronesis Platform Audit — 20 November 2026

| Item | Detail |
|---|---|
| Scope | `phronesis-platform` (ASP.NET Core backend) `main @ 3a143f2`, `phronesis-web` (Next.js 16) `main @ fc6ffd9` |
| Yardstick | `problem-definition-and-discovery.md`, `system-modules.md`, `initial.md` (backend foundation spec), `databasearchitecturespecification.md`, `api-endpoint-specification.md` |
| Method | Read all docs in both repos; built and tested the backend; **ran the API locally and probed it over HTTP**; type-checked, linted and dependency-audited the frontend; cross-checked all 92 frontend API calls against backend routes; static review of controllers, services, domain, auth, storage. Two passes. |
| Interactive version | https://claude.ai/artifact/5nZuU1dDCdXmNMcHNwkkXc |
| Follow-up | [Implementation plan](../plan/implementation-plan.md) |

Findings marked **(verified live)** were reproduced against the running API. Mutating probes used non-existent IDs, so no data was changed.

---

## 1. Verdict

The platform is not close to its objectives, and parts of it are unsafe. A usable skeleton exists: a layered .NET solution, roughly 70 domain entities covering most of the conceptual model, and a Next.js app with a page for most portal screens. The foundation that every document calls non-negotiable was never built: PostgreSQL, Docker, CI, observability, background jobs, object storage, a real payment flow and tests. **Over half of the API endpoints are placeholders, and several core actions are open to anyone on the internet.**

No end-to-end workflow works. Content cannot be created by any user. Assessments crash. Payments are simulated and subscriptions can be self-granted. Teacher approval needs no login. The frontend does not compile. The module "walkthrough" reports in `docs/` describe modules as complete; the code does not support those claims.

**Key figures**

| Measure | Value |
|---|---|
| Endpoint actions returning `501 Not Implemented` | 227 of 425 |
| Controllers with no `[Authorize]` and no fallback policy | ~20 |
| Call sites reading a claim that never exists (`FindFirst("sub")`) | 41 |
| Routes defined twice (crash with HTTP 500) | 6 |
| Business actions that write an audit record | 0 |
| Backend tests / of which empty placeholders | 8 / 4 |
| Frontend tests | 0 |
| Frontend compile errors / ESLint errors | 2 / 156 |

---

## 2. Critical findings

| ID | Finding | Evidence | Impact |
|---|---|---|---|
| C-01 | **Admin endpoints open to anonymous callers** (verified live) | No global auth policy; ~20 controllers lack `[Authorize]`. Without a token: `POST /admin/teacher-verifications/{id}/approve` reaches the handler; `GET /admin/teacher-verifications/pending`, `GET /content/pending-reviews`, `POST /grading/submissions/{id}/grade`, `POST /payments/guardian/checkout` return 200; content archive, taxonomy writes and assessment authoring reachable. | Anyone can approve teachers, archive content, change curriculum, plant exams. |
| C-02 | **User identity read from a non-existent claim** (verified live) | 41 sites use `User.FindFirst("sub")`; ASP.NET maps `sub` → `ClaimTypes.NameIdentifier`. Four identity-reading patterns coexist. Admin with valid token: `POST /content` → 401. | Content authoring, review, bookmarks, assessment attempts and most "me" actions fail for everyone. |
| C-03 | **Free subscriptions; client-set payment amounts** | `POST /commerce/subscriptions/mock-provision` open to any logged-in user and called by the learner UI. `PaymentRequest.Amount` is client-supplied. `MockPaymentProvider` always succeeds. Guardian checkout anonymous. | Commercial model bypassable. |
| C-04 | **Premium content public** (verified live) | `GET /catalog/{id}` anonymous, no entitlement check, returns raw file URIs under `wwwroot/uploads`. | Breaks premium-content protection (M13, business rule §19). |
| C-05 | **Secrets and documents in Git** | JWT key in `appsettings.Development.json`; admin `Admin@123!` seeded in every environment; 13 uploaded teacher files committed. *Update: files confirmed as test files; untracked and ignored on 20 Nov; history left as is.* | Known credentials; token forgery if key reused. |
| C-06 | **Plaintext passwords in logs** (verified live) | `HttpLoggingFields.All` logs request bodies; login wrote the password to the log. | Credential leakage. |
| C-07 | **Token design undermined** | Refresh token returned in JSON body as well as cookie; stored plaintext; access token persisted to `localStorage`; MFA simulated. | XSS protection absent. |
| C-08 | **Frontend does not compile** | `tsc` fails on `guardian/billing/page.tsx`, `LinkedLearnersWidget.tsx`. | No deployable build. |
| C-09 | **Nothing is audited** | `IAuditService` only used by the audit read endpoint. | Fails FR-17 / M30. |
| C-10 | **Vulnerable backend packages** | NU1903: `System.Security.Cryptography.Pkcs 9.0.11`, `Microsoft.OpenApi 2.0.0`. | Known exploitable dependencies. |
| C-11 | **Assessments crash** (verified live) | Same routes in `AssessmentController` and `LearningController`; `/roles` in two controllers. `GET /assessments`, `GET /assessments/{id}`, `POST /assessments/{id}/attempts`, `POST /roles` → 500 `AmbiguousMatchException`. | Learner assessment flow unusable. |
| C-12 | **Role policies undefined; roles never get permissions** (verified live) | `RequireAdmin/RequireTeacher/RequireLearner` not registered → treated as permission names; no `RolePermission` rows seeded. Admin gets 403 on settings, reconciliation, support queue. | Permission model not wired; protected endpoints lock out their own users. |

## 3. High and medium findings

| ID | Finding |
|---|---|
| H-01 | Assessment authoring (create, add questions with correct answers, publish) is anonymous. Scoring itself is sound: answers never sent to learners; marking server-side. |
| H-02 | "Learner ID" means two things: engagement/attempt records store the user ID; guardian queries use the learner-profile ID → guardian progress always empty. |
| H-03 | EF model has drifted from migrations (`has-pending-model-changes`). |
| H-04 | `npm audit` (production): 1 critical (`next`), 4 high (`sharp`, `zod`, `follow-redirects`, `source-map-js`). |
| H-05 | Booking: requested times not checked against availability; pending-request clashes undetected; `Price = 0`, no payment/entitlement check; naive `DateTime`, no Africa/Nairobi handling. |
| H-06 | Business logic in controllers: 37 of 49 controllers query the DbContext directly; Application layer is ~800 lines of mostly interfaces. |
| H-07 | Competing implementations of the same capability (teacher approval ×2, content review ×3, analytics ×4, classes ×4). |
| H-08 | Mock data in production paths: grading inbox, teacher earnings/schedule, guardian invoices (in **USD $149**), guardian GPA "3.8", messages, "TotalPoints = 120", video password `mock-password-123`. |
| H-09 | Domain model cannot express the business: `BillingInterval` only Monthly/Annual (no hour/day/week plans); no `Entitlement`; 4 content states vs 8 in spec (approve = publish); no resource versions, media assets, consent or safeguarding entities. |
| H-10 | Uploads stored in `wwwroot` with no extension/MIME/signature/size validation; client file names trusted. |
| M-01 | Settings-by-key and feature flags readable anonymously (toggling does require auth). |
| M-02 | Reset/verification tokens plaintext; password reset doesn't revoke sessions; roles created at runtime by registration controllers. |
| M-03 | Seven overlapping implementation plans across both repos; brand guidelines in three documents; misspelled/extension-less doc filenames; empty `problemstatement.md`. |
| M-04 | Three response envelope styles; no RFC 9457 error codes; no correlation IDs. |
| M-05 | 60+ nullable warnings; money precision unset; seeders run on every startup in every environment. |

## 4. Foundation against the specification (`initial.md` §66–68)

| Specified | Actual |
|---|---|
| PostgreSQL | SQL Server LocalDB, 31 SQL Server migrations |
| Redis, RabbitMQ, Hangfire, MinIO/S3, OpenSearch | None |
| `docker-compose.yml`, `.env.example` | Both 0 bytes |
| Solution file, central package management | `Phronesis.slnx` empty (root build builds nothing); `Directory.*.props` empty |
| CI/CD | `.github/workflows` empty in both repos |
| Health checks verify dependencies | `/health`, `/health/ready` return hard-coded "healthy" |
| OpenTelemetry, structured logs, correlation IDs | None |
| Validation, rate limiting, security headers | DataAnnotations only; none |
| Domain events, outbox | None |
| Architecture tests | Empty test |

## 5. Module scorecard

Legend: **Missing** not started · **Stub/Mock** placeholder · **Partial** real but incomplete · **Broken/Unsafe** fails or exploitable. No module meets the blueprint's Definition of Done (§12).

| Module | State | Evidence |
|---|---|---|
| M00 Architecture | Partial | Layering exists; logic in controllers |
| M01 Infra & DevOps | Missing | Empty compose/env/solution; no CI |
| M02 Database | Partial | ~70 entities; SQL Server; no entitlements/versions/concurrency |
| M03 Identity | Unsafe | Works, but passwords logged, refresh token leaked, MFA simulated |
| M04 RBAC | Broken | Policies undefined; permissions never granted; ~20 anonymous controllers |
| M05 Staff | Partial | Temp password returned in API response; staff mgmt 501 |
| M06 Learners/guardians | Partial | Linking works; progress mocked; ID mismatch; no consent flow |
| M07 Teacher profiles | Mock | Schedule and earnings hard-coded |
| M08 Teacher verification | Unsafe | Two flows, one anonymous; no fee/policy/suspension |
| M09–M10 Curriculum | Partial | Hierarchy modelled and seeded; writes anonymous; 19 stubs |
| M11 Content | Broken | Create fails for all users |
| M12 Review & publish | Broken | Approve publishes instantly; archive anonymous |
| M13 Media | Unsafe | Local `wwwroot`; raw URIs public |
| M14 Search | Partial | SQL `LIKE`; search/discover 501 |
| M15 Workspace | Partial | Placeholder values |
| M16 Assessments | Broken | Route collisions → 500; authoring anonymous |
| M17 Progress | Mock | Hard-coded numbers |
| M18 Subscriptions | Broken | Can't express plans; free self-provisioning |
| M19 Payments | Missing | Mock provider only |
| M20 Orders/invoices | Stub | Refund TODO; invoices hard-coded |
| M21 Classes | Partial | 30 stub endpoints |
| M22 Booking | Partial | See H-05 |
| M23 Video | Missing | Mock provider |
| M24 Attendance | Partial | Entities only |
| M25 Collaboration | Partial | Discussions/resources service |
| M26 Notifications | Partial | In-app only; SMTP localhost; synchronous |
| M27 Support | Partial | Tickets; admin queue 403 |
| M28 Administration | Partial | 39 stubs beside real controllers |
| M29 Reporting | Stub | 20 stubs, four overlapping controllers |
| M30 Audit | Broken | Nothing writes audit records |
| M31 Safeguarding | Missing | — |
| M32 QA | Missing | 8 tests (4 empty); 0 frontend |
| M33 Observability | Missing | Hard-coded health |
| M34 Mobile/PWA | Missing | — |
| M35–M36 | Future | Correctly deferred |

## 6. Frontend

| Area | Finding |
|---|---|
| Build | 2 compile errors; 156 ESLint errors, 65 warnings; 54 `any`; 49 `console`; `alert()` in 18 files |
| Auth | Access token in `localStorage`; route guard checks only cookie presence; no role checks (learner can open `/admin/*`); `middleware.ts` is deprecated in Next 16 (renamed `proxy.ts`) |
| API layer | Two competing axios clients (one dead, wrong port); data fetching inline in ~45 pages; only 4 React Query hooks; no shared types or generated client |
| Fakes | Subscription page calls `mock-provision`; billing, grading, earnings, messages render hard-coded data |
| Missing surfaces | Academic Reviewer, Content Manager, Finance, Support, Management (blueprint §6) |
| Public site | Pricing, About, Contact, Subjects contain only a heading; no legal pages |
| Hygiene | Duplicate header/footer sets; dev pages (`/brand-audit`, `/identity`) in production routes; no tests; no CI |
| Contract | Almost every frontend call has a backend route; the gap is behaviour (auth, mocks, claim bug), not URLs |

## 7. End-to-end journeys

| Journey | Status |
|---|---|
| Learner | Register/login works → email verification fragile (SMTP localhost) → catalog empty (no content can be created) → subscribe/pay faked → no entitlements → premium content unprotected → assessments crash → booking partial → live session not built |
| Teacher | Application works → documents unvalidated → verification anonymous → no fee/policy → content contribution 401 → grading/earnings fake |
| Content | Create 401 → submit 401 → no reviewer assignment → approve auto-publishes → no access rules, versioning or retirement |
| Class booking | Availability partial → booking partial → no payment check → mock video → no reminders/cancellation rules |

## 8. What is worth keeping

- Domain entities (~70) with private setters and some invariants — starting point, to be aligned with the DB spec.
- CBC taxonomy model (Curriculum → Level → Grade → Subject → Strand → Sub-strand → Objective).
- Auth building blocks: BCrypt, lockout, multi-device sessions, permission handler skeleton.
- Correct patterns: server-side assessment marking without leaking answers; guardian link checks; login requires verified email; no account enumeration on reset; real slot computation.
- Frontend stack choices (Next 16, React Query, Zod, RHF, Zustand) and the page inventory (61 pages).
- The strategy specs (problem definition, blueprint, DB spec, API spec).

## 9. Coverage and limits

| Depth | Examined |
|---|---|
| Full | All planning docs; build/tests/config; authorization on every controller; all stubs/mocks/TODOs; route collisions; frontend compile/lint/deps/contract; live probes of auth, content, catalog, commerce, assessments, role policies; payment, subscription, booking, storage and auth services |
| Sampled | ~20 of 49 controllers read in full; domain entities via search; notification, support, collaboration, accounting, tuition services in part |
| Not covered | Visual UI, responsiveness, accessibility; performance/load; page-by-page frontend logic; column-by-column EF vs DB spec; email content |

The sampled code sits in modules being rebuilt from the specs. Each module's rebuild starts with a full review of its legacy code and screens (see the implementation plan, "Module rebuild protocol").

## 10. Decisions taken after the audit

See [decision log](../plan/decisions-and-business-rules.md). Summary: PostgreSQL; rebuild module by module; uploads untracked; manual Paybill payments confirmed by finance admins (Daraja later); Google Meet with manually entered links; Hostinger KVM 2 hosting; new crimson-and-gold brand.
