# Decisions and Business Rules

This is the single record of product and engineering decisions for Phronesis. Code follows this document; if code and this document disagree, one of them is a bug. Each open decision has a **gate**: the phase that cannot start (or finish) until it is decided.

Related: [Implementation plan](implementation-plan.md) · [Target architecture](target-architecture.md) · [Audit](../audit/2026-11-20-platform-audit.md)

---

## 1. Decision log

| ID | Date | Decision | Rationale / consequence |
|---|---|---|---|
| D-001 | 2026-11-20 | **PostgreSQL** is the system of record; SQL Server is retired. | Matches all specs. No production data exists, so the switch is cheapest now. New migration baseline; legacy migrations archived. |
| D-002 | 2026-11-20 | **Rebuild the backend one module at a time** from the specs, keeping domain entities only as a starting point. | Patching would preserve logic-in-controllers, duplicated controllers and broken auth. |
| D-003 | 2026-11-20 | Uploaded files are **not tracked in Git**; history is not rewritten. | Files were test files. User uploads live in private storage, never in the repo. |
| D-004 | 2026-11-20 | **Payments are manual at launch**: payer sends money to the Phronesis M-PESA Paybill, uploads the confirmation, a **finance admin** verifies. Finance admins are **notified on every submission**. Daraja STK Push comes later. | No Daraja onboarding required for launch. A provider abstraction keeps the later switch cheap. Spec in §4. |
| D-005 | 2026-11-20 | **Live classes use Google Meet**, with links **entered manually** by the teacher or staff. | No Google API integration needed for launch. Spec in §5. |
| D-006 | 2026-11-20 | Plans and pricing are decided **jointly by engineering and business** using the options in §3. | Prices in the problem definition are provisional and internally inconsistent (§3.1). |
| D-007 | 2026-11-20 | Production runs on **one Hostinger KVM 2 VPS** (2 shared vCPU, 8 GB RAM, 100 GB NVMe, 8 TB bandwidth). | Drives a lean infrastructure: no OpenSearch or RabbitMQ at launch. See target architecture §3. |
| D-008 | 2026-11-20 | **New brand**: crimson and antique-gold seal ("tree of open books" on a stack of three books, radiant burst, classical Roman capitals). The blue/teal identity is retired. | Spec in `phronesis-web/docs/design-system/brand-and-design-system.md`. |
| D-009 | 2026-11-20 | One **Staff Console** for all internal roles (admin, finance, reviewer, content, support), with navigation driven by permissions, instead of separate portals per role. | Less duplicated UI; separation of duties enforced by permissions, not by separate apps. |
| D-010 | 2026-11-20 | **One login page** with role-based landing, replacing four per-portal login pages. Public "Apply to teach" and "Register" entry points remain separate. | Simpler, safer, one auth flow to test. |
| D-011 | 2026-11-20 | Avoid commercially licensed libraries in the core: **no MediatR, no AutoMapper**. Use thin in-house command/query handlers and explicit mapping (or Mapperly, source-generated). | MediatR and AutoMapper moved to commercial licences in 2025. |
| D-012 | 2026-11-20 | Search uses **PostgreSQL full-text search** (tsvector + trigram) at launch; OpenSearch only when measured need appears. | KVM 2 memory budget; one less system to operate. |
| D-013 | 2026-11-20 | Background work uses **Hangfire on PostgreSQL** plus a **transactional outbox** table; RabbitMQ deferred. | Reliable async without an extra broker on a single VPS. |

| D-014 | 2026-11-20 | Phronesis **sells exam papers online** to learners and the public (new module **M37 Exam Papers Store**), as a core MVP revenue product. | Built in Phase 7 on top of content, media and payments. Spec in §6. |
| D-015 | 2026-11-20 | Buyers can **request marking schemes**, handled end to end: request → policy/approval → payment if priced → watermarked delivery → audit. | Spec in §7. |
| D-016 | 2026-11-20 | Phronesis is a **national online homeschool**: every profile and buyer carries a county; operations are reported by county; the platform is designed for low-bandwidth use anywhere in Kenya. | Drives P3.1, P10.7 and frontend performance budgets. |
| D-017 | 2026-11-20 | Delivery progress is tracked from the plan's work-item IDs, a generated `PROGRESS.md`, and a GitHub Project mirroring every work item. | See `progress-tracking.md`. |

| D-018 | 2026-11-20 | **The app only shows finished work.** No "coming soon", placeholder pages, mock data, disabled menu items for unbuilt features, or build-progress information in the product. Delivery tracking stays internal (repo docs, GitHub Project). Unreleased modules are hidden by server-side feature flags. | Enforced by CI guards (P1.4.4) and feature flags (P1.4.5); see implementation plan §3.5. |

| D-019 | 2026-11-20 | **Branch model `dev` → `test` → `main`**: work branches stay local; only the repository owner merges and pushes the primary branches. `test` deploys to staging, `main` to production. Applies from Phase 0. | Owner controls every promotion; staging and production map to branches. |

New decisions are appended with the next ID. Superseded decisions are marked, never deleted.

---

## 2. Open decisions

| ID | Question | Proposed default (used if not decided by the gate) | Gate |
|---|---|---|---|
| O-01 | Official CBC taxonomy for launch grades (subjects, strands, sub-strands per grade) and which grades launch first. | Grades 7–9 (Junior School) first; 10–12 in a later content release. Taxonomy imported from a spreadsheet Phronesis supplies. | Phase 4 |
| O-02 | Minors: must learners under 18 be linked to a guardian? Who creates the account and who pays? What may guardians see? | Under-18 learners need a linked, consenting guardian before buying anything or joining live classes. Guardians see progress, attendance and payments. Either may pay. | Phase 3 |
| O-03 | Teacher money flow: onboarding fee amount and refundability, licence fee, earnings and payouts in MVP? | Onboarding fee + monthly licence collected via manual Paybill. Earnings recorded as a ledger in MVP; payouts made manually outside the platform and recorded by finance. | Phase 7 |
| O-04 | Plans and entitlements (see §3). | Option C (hybrid). | Phase 7 |
| O-05 | Live classes: 1:1, group or both; max group size; who assigns teachers; recording policy; cancellation, no-show and refund rules. | Both. Group max 25. Learner picks a verified teacher for 1:1; staff schedule group classes. No recording at launch. Cancel ≥24 h before = credit returned; learner no-show = credit consumed; teacher no-show = credit returned + incident logged. | Phase 8 |
| O-06 | Content: which resource types are free; view-only vs downloadable; ownership of teacher uploads. | Notes previews and one sample per topic free; everything else premium. Premium documents view-only with watermark; worksheets downloadable. Teachers grant Phronesis a licence on submission (agreement text by Phronesis legal). | Phase 5 |
| O-07 | Video hosting: VPS disk and bandwidth can't serve lesson video at scale. | External video service with signed playback (e.g. Bunny Stream or Cloudflare Stream). Launch with documents/assessments if budget not approved. | Phase 5 |
| O-08 | Learner–teacher messaging outside live classes; safeguarding reporting process and designated safeguarding lead. | No free-form direct messaging for learners at launch. Class-scoped Q&A visible to staff. "Report a concern" on every learner/teacher surface routed to a safeguarding lead. | Phase 9 |
| O-09 | Paybill number, account-reference format, and target confirmation time (SLA). | Account reference = invoice number (e.g. `PHX-24117`). SLA: confirmed within 4 working hours. | Phase 7 |
| O-10 | Domains: production and staging hostnames, email sending domain. | `phronesishomeschool.com` (web), `api.` and `staging.` subdomains; transactional email from `noreply@` with SPF/DKIM/DMARC. | Phase 1 |
| O-11 | Transactional email provider. | A provider with good deliverability and a free/low tier (e.g. Brevo, Resend or Amazon SES). | Phase 1 |
| O-12 | Vector logo files (SVG/PDF) from the logo designer. Current files are WhatsApp JPEGs. | Commission or redraw a vector master; no launch with raster-only logos. | Phase 1 (brand tokens can start from the JPEG colours) |
| O-13 | Legal texts: terms, privacy notice (Kenya Data Protection Act 2019), teacher agreement, safeguarding policy, refund policy. | Drafted by Phronesis with legal advice; the platform stores versioned agreements and records acceptance. | Phase 11 (acceptance mechanism built in Phase 2–3) |
| O-14 | Launch date and must-have list. | MVP = Phases 0–11 of the plan. | Phase 0 |
| O-15 | Exam paper pricing: per paper, bundle prices, discounts, institutional (school) pricing. | Single paper KES 50–150 by type; subject and term bundles at a discount; institutional pricing deferred unless confirmed. | Phase 7 |
| O-16 | Marking-scheme policy: included with the paper, sold separately, or released on approved request? Who may receive one (learners, guardians, teachers, schools)? Release delay? | Released on request to the paper's buyer; guardians, verified teachers and institutions approved automatically; learners approved by staff after a 24-hour delay; priced separately per O-15 unless bundled. | Phase 7 |
| O-17 | Can people buy without an account (guest checkout)? | No guest checkout: a lightweight verified buyer account (name, email, phone, county) is required so purchases can be re-downloaded, watermarked and audited. | Phase 7 |
| O-18 | Rights to sell each paper (KNEC past papers, county/zonal papers, Phronesis originals). | Sell only Phronesis-authored papers and papers with written licence; a rights record is mandatory before a paper can be listed. | Phase 5 (metadata), Phase 7 (listing) |
| O-19 | Release the exam papers store early (release R1 after Phase 7) before live classes are ready? | Yes, if store hardening passes. Earlier revenue and real usage data. | End of Phase 7 |

---

## 3. Plans and entitlements — options for the joint decision

### 3.1 What the problem definition says, and why it needs a decision

| Arena | Stated plan | Price (KES) | Issue |
|---|---|---|---|
| Learners | 1 Hour | 250 | Is this one live session, or one hour of content access? |
| Learners | 1 Day ("say 3 sessions") | 300 | Three sessions for 300 vs one for 250 makes the 1-hour plan poor value. |
| Learners | 1 Week ("3 sessions a day") | 2,000 | Up to 21 live sessions for 2,000 is cheaper per session than the 1-hour plan. Teacher cost not covered? |
| Learners | 1 Month / 1 Year | 8,000 / 100,000 | A year costs more than 12 months (96,000). Usually the opposite. |
| Teachers | 1 Week / 1 Month / 1 Year | 300 / 500 / 1,000 | A year is cheaper than two months. Licence vs onboarding fee unclear. |

Whatever the business picks, the engineering model below supports it: prices, durations and what each plan grants are **data**, not code.

### 3.2 The engineering model (fixed regardless of option)

- A **Plan** has a code, audience (learner/teacher), price + currency, duration (any ISO-8601 duration: `PT1H`, `P1D`, `P7D`, `P1M`, `P1Y`), and a list of **plan entitlements**.
- An **entitlement** is a typed grant: `content.premium` (with scope, e.g. grades 7–9), `class.session_credits` (quantity), `class.group_access`, `assessment.mock_exams`, `teacher.arena_access`, etc.
- Buying a plan creates an **order** → **invoice** → (manual) **payment** → on confirmation, a **subscription** with start/end and the concrete **entitlements** (with usage counters for credits).
- Access checks ask "does this user hold entitlement X now?" — never "is this user paid?".
- Expiry, cancellation, refund and suspension change entitlements through scheduled jobs and audited actions.

### 3.3 Options

| Option | How it works | Pros | Cons |
|---|---|---|---|
| A. Time passes | Every plan = all-premium-content access for its duration + N live-session credits. | Simple to explain. | Mixes two costs (content vs teacher time); hard to price fairly. |
| B. Separate products | Content subscription (week/month/year) **and** live sessions sold per session or in bundles. | Each price reflects its cost; flexible. | Two things to buy; more UI. |
| C. **Hybrid (recommended)** | Content subscriptions (week/month/term/year) include a small number of session credits; extra sessions bought singly (the "1 Hour" product) or as bundles. | Clear value ladder; teacher time priced honestly; matches "1 hour = one live session". | Needs clear plan comparison page. |

Teacher side (any option): one-time **onboarding fee** + recurring **licence** (week/month/year) granting `teacher.arena_access`; earnings ledger per delivered session (platform fee deducted) — payouts manual at launch (O-03).

---

## 4. Manual Paybill payments — functional specification

**Actors:** payer (learner, guardian or teacher), finance admin, system.

1. Payer chooses a plan/product → system creates an **order** and an **invoice** with a unique reference (e.g. `PHX-24117`), amount in KES, and Paybill instructions (Paybill number, account = invoice reference).
2. Payer pays on M-PESA, then submits a **payment confirmation**: M-PESA transaction code (required, format-validated, e.g. `SKL4XXXXXX`), amount paid, phone number used, payment date/time, and an optional screenshot (image/PDF, validated, stored privately).
3. System records a **payment submission** in state `Submitted` and immediately **notifies all finance admins** (in-app + email; SMS later). The payer sees "Awaiting confirmation" with the SLA (O-09).
4. Finance admin opens the **Payments queue** (Staff Console → Finance), checks the code against the Paybill statement, and either:
   - **Confirms** → submission `Confirmed`, invoice `Paid`, subscription/entitlements activated in the same database transaction, receipt generated, payer notified.
   - **Rejects** with a reason (e.g. code not found, amount mismatch) → payer notified and may resubmit.
   - **Marks partial** → invoice remains open for the balance.
5. Rules:
   - A transaction code can be **confirmed only once** across the whole system (unique index); duplicates are flagged automatically.
   - Amount must match the invoice unless a finance admin records an approved difference with a reason.
   - Only users with `payments.confirm` may confirm; a finance admin cannot confirm a payment for their own account (separation of duties).
   - Every state change is **audited** (who, when, before/after, reason).
   - Unconfirmed submissions older than the SLA raise a reminder to finance.
6. Reconciliation: finance uploads or enters the Paybill statement period; the report lists confirmed submissions not on the statement and statement lines with no submission.
7. Refunds: recorded by finance (reason, amount, M-PESA reversal reference); entitlements adjusted per refund policy; audited.
8. Future: a `DarajaStkPushProvider` implements the same `IPaymentProvider` contract; confirmed callbacks create the same `Confirmed` transition automatically.

## 5. Google Meet (manual links) — functional specification

1. A class session has an optional **meeting link** field, validated to `https://meet.google.com/xxx-xxxx-xxx`.
2. The assigned teacher (or staff with `classes.manage`) pastes the link when scheduling; sessions without a link show "Link pending" and trigger a reminder to the teacher 24 h and 2 h before start.
3. The link is visible **only** to the assigned teacher, staff, and learners with a confirmed booking (entitlement checked server-side). It is never included in public pages or list endpoints.
4. "Join" goes through the platform (`POST /sessions/{id}/join`), which records a **join event** (attendance evidence) and then redirects to Meet.
5. The teacher marks attendance after the session (present/late/absent/excused), adds session notes; learners give feedback. Disputes go to support.
6. Safeguarding guidance shown to teachers: use the Phronesis Google account/calendar for sessions, admit only listed learners, no recordings unless policy allows.
7. Future: Google Calendar API creates links automatically and attendance reports are imported (needs Google Workspace).

## 6. Exam papers store — functional specification

**Actors:** buyer (learner, guardian, teacher, institution or member of the public), content manager, academic reviewer, finance admin, system.

**Catalogue**
1. Exam papers are content resources (Phase 5) with exam metadata: exam type (past paper, mock, end-of-term, opener, county/zonal, Phronesis original), year, term, paper number, duration, total marks, grade(s), subject, and a linked marking-scheme resource. Each paper passes the normal content review before it can be sold.
2. A **store product** wraps one paper or a **bundle** (e.g. "Grade 8 Term 2 — all subjects", "Mathematics Grade 7–9 set"). A product has a title, description, price in KES, status (draft, listed, unlisted, retired) and optional sale price with dates.
3. A paper can be listed only when its **rights record** is complete (O-18).
4. Product pages are public and searchable, with filters for grade, subject, exam type, year and term. Each shows a **watermarked preview** (cover and instructions only), never the full paper.

**Purchase**
5. Buyer adds products to a cart and checks out with a verified buyer account (O-17). Checkout creates an order and an invoice and shows Paybill instructions.
6. Payment follows the manual Paybill flow (§4). Finance admins are notified and confirm. On confirmation, the buyer receives an item entitlement for each paper in the order, a receipt, and a "Your papers are ready" notification.
7. Guardians can buy for linked learners; the papers appear in both libraries.

**Fulfilment**
8. "My purchases" lists owned papers. Viewing and downloading generate a **personalised PDF on the server** from the private master. Every page carries the buyer's name, contact and order number, plus a faint diagonal watermark. Files are never served from a public URL.
9. Download policy is configurable per product (default: 5 downloads, links valid 15 minutes). Every view and download is logged. Unusual patterns (many downloads, many IPs) raise an alert to staff.
10. A revised paper creates a new version. Previous buyers get the update and are notified.

**Operations**
11. Staff Console → Store: products and bundles, pricing, featured lists, orders, downloads, refunds (via the payments refund flow), sales reports by paper, subject, grade, county and period.
12. Institutional buyers (if O-15 confirms): organisation account, bulk licence quantity, invoice, several named downloaders, each copy watermarked with the institution and user.

## 7. Marking-scheme requests — functional specification

1. On any purchased paper, the buyer can **request the marking scheme**. A guardian can also request one for a linked learner's paper.
2. The request records requester, paper, role, reason (optional) and requested date, in state `Requested`.
3. A **policy** decides the path (O-16):
   - **Auto-approve**, e.g. verified teachers, guardians, institutions, or when the scheme is bundled with the product.
   - **Review required**, e.g. learners: goes to the Academic Reviewer / Content Manager queue, optionally only after a release delay so learners attempt the paper first.
   - **Priced**: an invoice is created and the request waits in `Awaiting payment` until finance confirms.
4. Reviewer approves or declines with a reason. Declines tell the requester what to do next.
5. On approval (and payment where required) the request becomes `Fulfilled`: the marking scheme appears in the requester's library as a watermarked PDF under the same download policy as papers.
6. States: Requested → (Awaiting payment) → (Under review) → Approved → Fulfilled · Declined · Cancelled. Every transition notifies the requester and is audited (who released which scheme to whom, when).
7. Reports: request volumes, approval rate, turnaround time, revenue from priced schemes.

## 8. Core business rules (from the problem definition §19, made testable)

| Rule | Enforced by |
|---|---|
| A learner may access premium resources only with an active matching entitlement. | `ResourceAccessPolicy` server-side; signed, short-lived file URLs. |
| A teacher cannot teach or publish until verification **and** onboarding (fee + agreement) are complete. | `TeacherStatus = Active` required by `teacher.*` permissions. |
| Only reviewers (not the author) may approve content. | `content.review` permission + author ≠ reviewer check. |
| Premium resources are never visible because a URL is known. | No public file paths; every download/view goes through an authorised, expiring link. |
| Booking lifecycle: Requested → Confirmed (entitlement/payment) → Scheduled (link set) → Completed / Cancelled / No-show. | State machine in the Booking module; invalid transitions rejected. |
| Subscription expiry automatically removes entitlements. | Hourly Hangfire job + check at access time. |
| Account status, teacher verification status and subscription status are separate fields. | Domain model. |
| Suspension/revocation for users, teachers and content. | Audited admin commands. |
| All administrative, financial, verification, publishing and permission actions are audited. | Audit pipeline (command decorator + EF interceptor). |
| An exam paper or marking scheme is delivered only to an account holding the matching item entitlement, always watermarked to that buyer. | Store fulfilment service; signed, expiring download links. |
| A paper can be listed for sale only with a complete rights record. | Store listing validation. |

## 9. Role model (launch)

| Role | Purpose | Notes |
|---|---|---|
| Super Admin | Platform owner; manages admins and security settings | MFA required; ≤2 people |
| Admin | Operations: users, classes, configuration | MFA required |
| Finance Admin | Payments queue, invoices, refunds, reconciliation, teacher earnings | MFA required; notified on every payment submission |
| Academic Reviewer | Teacher applications, content review | MFA required |
| Content Manager | Curriculum taxonomy, content authoring and publishing schedule | MFA required |
| Support Agent | Tickets, account assistance (no financial or academic decisions) | MFA required |
| Safeguarding Lead | Concerns, restrictions, incident handling | MFA required; may be combined with Admin initially |
| Teacher | Teacher Arena | Gated by verification + onboarding |
| Learner | Learner portal | Minors per O-02 |
| Guardian | Guardian portal | Linked learners only |

Permissions are fine-grained codes (e.g. `payments.confirm`, `content.review`, `teachers.approve`); roles are bundles of permissions seeded from code and reviewable in the Staff Console. The full matrix is produced in Phase 2 and kept in `docs/specs/permission-matrix.md`.
