PHRONESIS HOMESCHOOL
DATABASE ARCHITECTURE & DATA MODEL SPECIFICATION
Foundational PostgreSQL Database Blueprint
Document Item
Specification
Database
PostgreSQL
Application
ASP.NET Core + C# modular monolith
Cache
Redis
Messaging
RabbitMQ
Jobs
Hangfire
Object Storage
MinIO / S3-compatible
Search
OpenSearch
Video
Dedicated HLS/DASH infrastructure
Scope
Full target Phronesis platform
Authority
PostgreSQL is the transactional system of record



1. Purpose and Scope
This document defines the database architecture, logical domains, entities, relationships, constraints, indexes, security, audit, lifecycle and implementation standards for the Phronesis Homeschool platform. It is the foundational database blueprint to be used before physical schema implementation.
The model covers identity, learners, guardians, teachers, teacher verification, CBC curriculum, resources, media, search, learning, assessments, subscriptions, M-PESA payments, virtual classes, scheduling, bookings, attendance, collaboration, notifications, support, administration, reporting, safeguarding, audit and data-rights workflows.
2. Architectural Position
Start with one PostgreSQL database behind the ASP.NET Core modular monolith. Separate domains logically and in application code; do not create multiple databases merely to simulate microservices. Individual high-load domains can later be extracted.
System
Responsibility
Source of Truth?
PostgreSQL
Durable transactional business state
YES
Redis
Cache, rate limiting, ephemeral state
NO
RabbitMQ
Asynchronous events/integration messages
NO
Hangfire
Background job execution state
Operational
MinIO/S3
PDFs, documents, images, audio/video binaries
Binary store
OpenSearch
Search/index projections
NO
Video infrastructure
Encoding/streaming
NO


3. Core Database Principles
PostgreSQL is authoritative for durable business facts.
Use UUID public identifiers; do not expose sequential database IDs.
Separate users/credentials from learner, guardian, teacher and staff profiles.
Use normalized relational structures for transactional data and deliberate read models for reporting.
Do not store large files in PostgreSQL; store object metadata and references.
Prefer lifecycle/status transitions over destructive deletion for historical records.
Version published academic content and assessments.
Preserve financial, verification, booking and audit history.
Use database constraints for hard invariants and application services for complex workflows.
Avoid polymorphic foreign keys for core relationships; use explicit link tables.
Design for future multi-region/global growth without prematurely introducing distributed databases.
4. Logical Domains
Domain
Core Tables / Aggregates
Identity & Access
users, credentials, sessions, verification_tokens, roles, permissions, user_roles
People
learner_profiles, guardian_profiles, teacher_profiles, staff_profiles, learner_guardians, departments
Teacher Compliance
teacher_applications, application_subjects, qualifications, verification_documents, verification_reviews
Academic
curricula, curriculum_versions, grades, curriculum_grades, subjects, grade_subjects, topics, topic_relations, academic_terms
Content
resources, resource_versions, resource_grades, resource_subjects, resource_topics, tags, resource_tags, resource_reviews, access_rules, licenses
Media
media_assets, media_variants, upload_sessions, media_processing_jobs, playback_sessions
Learning
learner_enrollments, learner_subjects, learning_activity, learning_progress, bookmarks, learning_goals
Assessment
question_banks, questions, question_options, assessments, assessment_items, attempts, attempt_answers, results
Commerce
plans, plan_entitlements, subscriptions, subscription_events, entitlements, entitlement_usage, orders, order_items, invoices, refunds, receipts
Payments
payments, payment_transactions, payment_provider_events, reconciliation_records
Classes
class_offerings, class_sessions, teacher_assignments, class_materials, session_access
Booking
availability_rules, availability_exceptions, bookings, booking_events, attendance, session_notes, session_feedback
Collaboration
contributions, contribution_collaborators, comments
Communications
notifications, notification_preferences, notification_templates, notification_deliveries, announcements
Support
support_tickets, ticket_messages, ticket_assignments
Administration
system_settings, feature_flags, staff_actions
Compliance
audit_logs, consent_records, data_requests, security_events
Safeguarding
user_reports, user_restrictions, user_blocks, moderation_actions
Reporting
report_jobs, read models/materialized views as scale requires


5. Naming and Type Standards
Area
Standard
Tables/columns
snake_case; plural table names
Primary keys
uuid
Foreign keys
<entity>_id uuid
Timestamps
timestamptz
Money
numeric(19,4) + currency_code; never float
Flexible metadata
jsonb only where justified
Email
citext or normalized lowercase + unique index
Status
controlled values; preferably lookup/check constraints
Files
metadata in PostgreSQL; binary in object storage
Concurrency
version/ETag strategy on concurrently edited records
Deletion
archive/deactivate where history matters


6. Identity & Access
6.1 users
Central account record shared by all personas.
Column
Type
Null
Key / Rule
Description
id
uuid
NO
PK
Account ID
email
citext
YES
UNIQUE
Normalized email
phone_number
varchar(30)
YES
INDEX
Phone
email_verified_at
timestamptz
YES


Verification time
phone_verified_at
timestamptz
YES


Verification time
status
varchar(30)
NO
CHECK
pending/active/suspended/deactivated
last_login_at
timestamptz
YES


Last login
created_at
timestamptz
NO


Created
updated_at
timestamptz
NO


Updated


Table
Important Columns
Purpose
credentials
id, user_id, type, provider, provider_subject, secret_hash, created_at, last_used_at
Authentication credentials
roles
id, code, name, description, status
RBAC roles
permissions
id, code, name, module
Atomic capabilities
role_permissions
role_id, permission_id
Role-to-permission map
user_roles
user_id, role_id, assigned_at, assigned_by
User roles
sessions
id, user_id, refresh_token_hash, expires_at, revoked_at, device metadata
Session lifecycle
verification_tokens
id, user_id, purpose, token_hash, expires_at, used_at
Verification/reset tokens


7. People and Profiles
Table
Important Columns
Purpose
learner_profiles
user_id PK/FK, admission_no UNIQUE, date_of_birth, current_grade_id, curriculum_id, learning_status
Learner profile
guardian_profiles
user_id PK/FK, status
Guardian profile
learner_guardians
learner_id, guardian_id, relationship_type, is_primary, starts_at, ends_at
Explicit guardian relationship
staff_profiles
user_id PK/FK, staff_no UNIQUE, department_id, job_title, status
Employee profile
departments
id, code UNIQUE, name, status
Internal organization
learner_subjects
learner_id, subject_id, status, started_at, ended_at
Subject enrollment


A learner/teacher/guardian is an extension of a user account. Login credentials must never be duplicated into profile tables.
8. Teacher Onboarding & Verification
Table
Important Columns
Purpose
teacher_profiles
user_id, teacher_no UNIQUE, bio, verification_status, teaching_status
Approved teacher identity
teacher_applications
id, user_id, application_no, status, submitted_at, reviewed_at, assigned_reviewer_id
Onboarding case
teacher_application_subjects
application_id, subject_id, grade_id
Requested teaching scope
teacher_qualifications
id, teacher_id, qualification_type, institution, award, year, verification_status
Qualifications
verification_documents
id, application_id/teacher_id, document_type, media_asset_id, status
Evidence documents
verification_reviews
id, application_id/teacher_id, reviewer_id, decision, reason, reviewed_at
Immutable review history
teacher_subjects
teacher_id, subject_id, status
Approved subjects
teacher_grades
teacher_id, grade_id, status
Approved grades


Verification decisions must never overwrite previous evidence. Every approval, rejection, correction request or revocation is retained as a historical review record.
9. Curriculum & Academic Structure
Table
Important Columns
Purpose
curricula
id, code UNIQUE, name, country_code, status
Top-level curriculum
curriculum_versions
id, curriculum_id, version_no, status, effective_from, published_at
Versioned curriculum
grades
id, code UNIQUE, name, level, age_min, age_max, sort_order
Grade catalog
curriculum_grades
curriculum_version_id, grade_id
Curriculum grade membership
subjects
id, code UNIQUE, name, category, description, status
Subject catalog
grade_subjects
grade_id, subject_id, curriculum_version_id, required_flag, sort_order
Subject offering per grade
topics
id, subject_id, parent_topic_id, code, name, description, sort_order, status
Hierarchical topic tree
topic_relations
from_topic_id, to_topic_id, relation_type
Prerequisites/related topics
academic_terms
id, curriculum_version_id, name, sequence, starts_at, ends_at
Optional academic periods


This supports Grade 7–12 and the supplied subject categories, including sciences, languages, humanities, CRE and other approved offerings without schema changes.
10. Educational Content
Table
Important Columns
Purpose
resources
id, resource_code, title, resource_type, description, status, visibility, access_level, author_user_id
Logical educational resource
resource_versions
id, resource_id, version_no, status, content_format, media_asset_id, text_content, published_at
Versioned content
resource_grades
resource_id, grade_id
Grade mapping
resource_subjects
resource_id, subject_id
Subject mapping
resource_topics
resource_id, topic_id
Topic mapping
tags
id, slug UNIQUE, name
Tags
resource_tags
resource_id, tag_id
Resource tags
resource_reviews
id, resource_id, version_id, reviewer_id, status, comments, reviewed_at
Quality review
resource_access_rules
id, resource_id, access_type, plan_id, starts_at, ends_at
Free/premium entitlement rules
resource_licenses
id, resource_id, license_type, rights_holder, license_reference, expires_at
Copyright/licensing metadata
resource_publication_events
id, resource_id, version_id, action, actor_id, occurred_at
Publication history


Past papers, updated notes, topical questions, exams, videos and animations are represented as resource types. Premium content is protected by access rules and entitlement checks.
11. Media & Secure Content
Table
Important Columns
Purpose
media_assets
id, storage_provider, bucket, object_key, original_filename, mime_type, size_bytes, checksum, status
Object metadata
media_variants
id, media_asset_id, variant_type, object_key, mime_type, size_bytes, width, height, bitrate
Derived media
upload_sessions
id, user_id, media_asset_id, status, expires_at
Controlled upload
media_processing_jobs
id, media_asset_id, job_type, status, attempts, started_at, completed_at
Scanning/transcoding
playback_sessions
id, resource_id/media_asset_id, user_id, expires_at, revoked_at, policy_snapshot
Short-lived video access


12. Learning Workspace
Table
Important Columns
Purpose
learner_enrollments
id, learner_id, grade_id, curriculum_version_id, status, starts_at, ends_at
Academic enrollment
learner_subjects
learner_id, subject_id, status, started_at, ended_at
Subjects
learning_activity
id, learner_id, resource_id, topic_id, activity_type, started_at, completed_at, duration_seconds, progress_percent
Historical activity
learning_progress
id, learner_id, topic_id/resource_id, progress_percent, mastery_score, last_activity_at
Current progress projection
bookmarks
id, learner_id, resource_id, created_at
Saved resources
learning_goals
id, learner_id, title, target_date, status
Personal goals


learning_activity is historical; learning_progress is current/derived state. High-volume activity can later be partitioned.
13. Assessments, Questions & Exams
Table
Important Columns
Purpose
question_banks
id, name, subject_id, grade_id, topic_id, owner_id, status
Question collection
questions
id, bank_id, question_type, stem, difficulty, marks, explanation, status
Question
question_options
id, question_id, option_key, option_text, is_correct
Objective options
question_tags
question_id, tag_id
Classification
assessments
id, title, type, subject_id, grade_id, duration_seconds, total_marks, status, published_at
Exam/test/topical assessment
assessment_items
assessment_id, question_id, sequence, marks
Assessment composition
assessment_access_rules
assessment_id, access_type, plan_id
Access policy
assessment_attempts
id, assessment_id, learner_id, started_at, submitted_at, status, score, percentage
Attempt
attempt_answers
id, attempt_id, question_id, selected_option_id, answer_text, marks_awarded, answered_at
Answers
assessment_results
id, attempt_id, score, percentage, grade, feedback, finalized_at
Final result
assessment_feedback
id, result_id, author_id, feedback_text, created_at
Teacher feedback


Published assessments must be immutable. Changes create a new version/assessment rather than altering the questions used by historical attempts.
14. Subscriptions, Entitlements & Commerce
Table
Important Columns
Purpose
plans
id, code UNIQUE, name, audience, billing_period, price, currency_code, status
Commercial plan
plan_entitlements
plan_id, entitlement_code, limits_jsonb
Plan capabilities
subscriptions
id, user_id, plan_id, status, starts_at, expires_at, auto_renew, cancelled_at
Subscription lifecycle
subscription_events
id, subscription_id, event_type, occurred_at, actor_id, metadata
History
entitlements
id, user_id, entitlement_code, source_type, source_id, starts_at, expires_at, status
Effective access
entitlement_usage
id, entitlement_id, usage_key, quantity, period_start, period_end
Usage limits
orders
id, order_no UNIQUE, user_id, status, currency_code, subtotal, discount, tax, total, placed_at
Commercial order
order_items
id, order_id, item_type, reference_id, description, quantity, unit_price, total
Purchased item
invoices
id, invoice_no UNIQUE, order_id, status, issued_at, due_at, total
Invoice
refunds
id, order_id, payment_id, amount, currency_code, reason, status, requested_at, processed_at
Refund
receipts
id, payment_id, receipt_no UNIQUE, issued_at, document_media_asset_id
Receipt


Plan prices are configuration, not hard-coded schema logic. The supplied learner and teacher pricing can be seeded into plans and changed by authorized administrators.
15. Payments & M-PESA
Table
Important Columns
Purpose
payments
id, payment_no UNIQUE, user_id, order_id, method, provider, amount, currency_code, status, idempotency_key UNIQUE, initiated_at, completed_at
Provider-neutral payment
payment_transactions
id, payment_id, provider_reference, transaction_type, amount, status, occurred_at
Provider transaction history
payment_provider_events
id, provider, external_event_id, event_type, payload_jsonb, signature_valid, processed_at, status
Verified callback record
reconciliation_records
id, provider, statement_reference, payment_id, expected_amount, received_amount, status, reconciled_at
Finance reconciliation


M-PESA callbacks are stored for traceability, but normalized payment state is authoritative. Provider events must be idempotent and protected against replay.
16. Virtual Classes & Sessions
Table
Important Columns
Purpose
class_offerings
id, class_code, title, teacher_id, subject_id, grade_id, topic_id, description, capacity, status
Bookable offering
class_sessions
id, class_offering_id, starts_at, ends_at, status, meeting_provider, external_session_id, recording_media_asset_id
Scheduled session
teacher_assignments
id, teacher_id, class_offering_id/session_id, role, starts_at, ends_at
Teacher responsibility
class_materials
id, class_offering_id/session_id, resource_id, visibility
Class resources
session_access
id, session_id, user_id, access_type, granted_at, revoked_at
Resolved access


17. Scheduling, Bookings & Attendance
Table
Important Columns
Purpose
availability_rules
id, teacher_id, day_of_week, start_time, end_time, timezone, effective_from, effective_to
Recurring availability
availability_exceptions
id, teacher_id, starts_at, ends_at, exception_type, reason
Blocked/custom periods
bookings
id, booking_no UNIQUE, session_id, learner_id, status, booked_at, confirmed_at, cancelled_at
Booking lifecycle
booking_events
id, booking_id, event_type, actor_id, occurred_at, metadata
Booking history
attendance
id, session_id, learner_id, status, joined_at, left_at, duration_seconds, marked_by
Attendance
session_notes
id, session_id, teacher_id, note_text, created_at
Teacher notes
session_feedback
id, session_id, user_id, rating, feedback_text, created_at
Feedback


18. Teacher Contributions & Collaboration
Table
Important Columns
Purpose
contributions
id, teacher_id, resource_id, status, submitted_at, approved_at
Teacher contribution
contribution_collaborators
contribution_id, user_id, role, added_at
Collaborators
comments
id, contribution_id, user_id, parent_comment_id, body, status, created_at
Threaded comments


19. Notifications & Announcements
Table
Important Columns
Purpose
notifications
id, user_id, type, title, body, channel, status, read_at, created_at
Notification
notification_preferences
user_id, notification_type, channel, enabled
Preferences
notification_templates
id, code UNIQUE, channel, subject_template, body_template, status
Templates
notification_deliveries
id, notification_id, channel, provider, provider_reference, status, attempted_at, delivered_at
Delivery tracking
announcements
id, title, body, audience_rule, status, published_at, expires_at, author_id
Platform announcements


20. Support
Table
Important Columns
Purpose
support_tickets
id, ticket_no UNIQUE, requester_id, category, priority, status, subject, created_at, closed_at
Support case
ticket_messages
id, ticket_id, author_id, body, attachment_media_asset_id, created_at
Conversation
ticket_assignments
id, ticket_id, staff_id, assigned_at, unassigned_at
Ownership


21. Administration
Table
Important Columns
Purpose
system_settings
key PK, value_jsonb, category, sensitive_flag, updated_by, updated_at
Runtime configuration
feature_flags
key PK, enabled, rollout_jsonb, updated_by, updated_at
Controlled feature rollout
staff_actions
id, staff_id, action_type, target_type, target_id, reason, created_at
Operational actions


Secrets and credentials must live in a dedicated secret-management mechanism, not system_settings.
22. Audit, Compliance & Data Rights
Table
Important Columns
Purpose
audit_logs
id, actor_user_id, action, entity_type, entity_id, before_jsonb, after_jsonb, ip_address, user_agent, correlation_id, occurred_at
Audit trail
consent_records
id, user_id, consent_type, version, granted, granted_at, withdrawn_at, evidence_jsonb
Consent history
data_requests
id, user_id, request_type, status, requested_at, completed_at, reviewed_by, reason
Export/deletion workflow
security_events
id, user_id, event_type, severity, ip_address, user_agent, metadata_jsonb, occurred_at
Security events


23. Safeguarding & Moderation
Table
Important Columns
Purpose
user_reports
id, reporter_id, subject_user_id, resource_id, report_type, reason, status, assigned_to, created_at, resolved_at
Report user/content
user_restrictions
id, user_id, restriction_type, starts_at, ends_at, reason, imposed_by, status
Account restrictions
user_blocks
blocker_user_id, blocked_user_id, created_at, removed_at
User blocking
moderation_actions
id, report_id, moderator_id, action, reason, created_at
Moderation history


24. Core Relationships
Parent
Relationship
Child
users
1 : 0..1
learner_profiles / teacher_profiles / guardian_profiles / staff_profiles
users
M : N
roles via user_roles
learners
M : N
guardians via learner_guardians
curricula
1 : M
curriculum_versions
curriculum_versions
M : N
grades
grades
M : N
subjects
subjects
1 : M
topics
resources
M : N
grades, subjects and topics
resources
1 : M
resource_versions
resources
1 : M
resource_reviews
learners
1 : M
assessment_attempts
assessments
M : N
questions
users
1 : M
subscriptions
orders
1 : M
payments
class_offerings
1 : M
class_sessions
learners
1 : M
bookings
class_sessions
1 : M
attendance
users
1 : M
notifications
users
1 : M
data_requests


25. Critical Constraints
Rule
Enforcement
Email is unique
Unique normalized email index
Admission/teacher numbers unique
Unique indexes
One primary guardian where required
Partial unique index
Published resource references valid version
FK + publishing transaction
Published assessment cannot mutate historical attempts
Version/status design
Money cannot be negative
CHECK constraints
Refund cannot exceed captured amount
Transactional business validation
Booking capacity cannot be exceeded
Transaction + locking
Provider callback processed once
Unique provider/external_event_id
Payment initiation retry-safe
Unique idempotency key
Audit records append-only
DB permissions/triggers + application policy


26. Indexing Strategy
Area
Indexes
users
unique normalized email; phone; status
learners
admission_no; current_grade_id; status
teachers
teacher_no; verification_status; teaching_status
applications
status; reviewer; submitted_at
topics
subject_id; parent_topic_id; status
resources
status; type; access_level; published_at; author
resource mappings
academic_id + resource_id in both directions
learning_activity
learner_id + occurred_at; resource_id + occurred_at
attempts
learner_id + started_at; assessment_id + status
subscriptions
user_id + status; expires_at
payments
user_id + initiated_at; status; provider_reference
provider events
unique provider + external_event_id
sessions
starts_at + status; teacher
bookings
learner_id + booked_at; session_id + status
notifications
user_id + read_at + created_at
audit
occurred_at; actor; entity; correlation_id


27. Search & Object Storage Boundaries
OpenSearch is a derived index. PostgreSQL remains authoritative.
Content changes should create an outbox/indexing event; indexing is asynchronous and idempotent.
Search may temporarily lag publication without invalidating the PostgreSQL transaction.
Media binaries live in MinIO/S3-compatible storage.
PostgreSQL stores media ownership, object key, checksum, type, size, lifecycle and access metadata.
Premium playback/download authorization must be short-lived; permanent public object URLs must not be exposed.
28. Transactions & Concurrency
Use explicit transactions for subscription purchase, payment state changes, booking confirmation, assessment submission and verification/publication transitions.
Use optimistic concurrency for editable content and administrative records.
Use row locking/transactional capacity checks for scarce class slots.
Redis must never be the sole correctness mechanism for financial or booking state.
Payment and webhook processing must tolerate retries.
29. Partitioning & Growth
Do not partition everything initially. Add partitioning after observed volume justifies it.
Candidate
Likely Partition Key
learning_activity
occurred_at
resource_views if introduced
viewed_at
audit_logs
occurred_at
security_events
occurred_at
notification_deliveries
attempted_at
payment_provider_events
received_at


30. Security & Database Access
Use separate migration and runtime database roles.
Production runtime must not have unrestricted schema-management privileges.
Encrypt database connections and backups.
Restrict production database access using least privilege.
Never store raw passwords, refresh tokens, payment secrets or provider credentials.
Sensitive teacher verification documents require dedicated access controls.
Audit production administrative database access.
31. Data Lifecycle & Deletion
Account deactivation and irreversible deletion are separate operations.
Financial, audit and legally required records may require retention even after account deletion, subject to legal advice.
Published content should normally be archived rather than destructively deleted where learner history depends on it.
Deletion/export should run as controlled asynchronous jobs.
Resolve foreign-key dependencies through anonymization, archival, detachment or controlled deletion.
32. Backup, Recovery & Availability
Capability
Baseline
Backups
Automated PostgreSQL backups with point-in-time recovery where available
Redundancy
Separate backup location
Encryption
At rest and in transit
Restore testing
Scheduled restoration drills
Migrations
Version-controlled and reviewed
Replication
Introduce read replica/HA when availability or read volume requires
RPO/RTO
Explicitly approved before production SLA


33. PostgreSQL Logical Schema Organization
One physical database may use PostgreSQL schemas to make module boundaries explicit without creating distributed-database complexity.
Schema
Representative Tables
identity
users, credentials, sessions, verification_tokens
access
roles, permissions, role_permissions, user_roles
people
learner_profiles, guardian_profiles, teacher_profiles, staff_profiles, learner_guardians
academic
curricula, curriculum_versions, grades, subjects, topics, mappings
content
resources, versions, reviews, tags, access_rules, licenses
media
media_assets, variants, uploads, processing_jobs, playback_sessions
learning
enrollments, activity, progress, bookmarks, goals
assessment
banks, questions, options, assessments, attempts, answers, results
commerce
plans, entitlements, subscriptions, orders, invoices, refunds
payments
payments, transactions, provider_events, reconciliation
classes
offerings, sessions, assignments, availability
booking
bookings, events, attendance, notes, feedback
collaboration
contributions, collaborators, comments
communications
notifications, preferences, templates, deliveries, announcements
support
tickets, messages, assignments
administration
settings, feature_flags, staff_actions
compliance
audit_logs, consent_records, data_requests, security_events
safeguarding
reports, restrictions, blocks, moderation_actions


34. Module Dependency Rules
Module
May Depend On
Must Not Own
Identity
None
Business profiles
Access
Identity
Domain data
People
Identity + academic references
Authentication
Academic
Foundation
Content files
Content
People + Academic + Media
Payment internals
Learning
People + Academic + Content + assessment interfaces
Payment internals
Assessment
People + Academic + Content where required
Payment internals
Commerce
Identity + entitlement definitions
Academic structure
Payments
Commerce + Identity
Learning state
Classes
People + Academic + Commerce
Authentication
Booking
Classes + People + Commerce
Content internals
Communications
Identity + events
Core domain state
Audit
Cross-cutting
Business ownership


35. Reporting & Analytics
Start with indexed operational queries and materialized views where needed.
Introduce dedicated read models as dashboard/report volume grows.
Use background jobs for large exports.
Store report parameters and generation status for reproducibility.
Do not make Redis dashboard caches authoritative for financial or academic figures.
36. Initial Seed Plans
Audience
Plan
Period
Initial Fee (KES)
Learner
1 Hour
HOUR
250
Learner
1 Day
DAY
300
Learner
1 Week
WEEK
2,000
Learner
1 Month
MONTH
8,000
Learner
1 Year
YEAR
100,000
Teacher
1 Week
WEEK
300
Teacher
1 Month
MONTH
500
Teacher
1 Year
YEAR
1,000


These are configuration values from the product definition and should remain editable through authorized plan administration.
37. Implementation Order
Create PostgreSQL, extensions, environments and migration pipeline.
Implement identity and access.
Implement learner, guardian, teacher and staff profiles.
Implement teacher applications, documents and verification.
Implement curriculum, grades, subjects and topics.
Implement resources, media metadata and content review/publishing.
Implement learning workspace.
Implement assessments and results.
Implement plans, subscriptions and entitlements.
Implement orders, M-PESA payments, receipts and reconciliation.
Implement classes, sessions, availability and bookings.
Implement attendance, notes and feedback.
Implement collaboration, notifications and support.
Implement administration, reporting, audit, compliance and safeguarding.
Add OpenSearch projections, replicas and partitioning only when justified by real workload.
38. Database Definition of Done
Every table has a documented business purpose and owning module.
Every relationship has explicit cardinality and FK behavior.
Primary keys, unique constraints and CHECK constraints are implemented.
Delete/archive behavior is explicitly defined.
Sensitive data classification is documented.
Indexes are tied to known query patterns.
Migrations and seed data are reproducible.
Critical payment, booking, entitlement and assessment transitions have integration tests.
Backup restoration is tested before production launch.
Audit and data-rights workflows are tested.
Slow-query, connection-pool and database health monitoring is enabled.
39. Items to Freeze Before Physical Schema Generation
Final role/permission matrix.
Exact learner/guardian account and purchasing relationship.
Final approved CBC subject and topic taxonomy.
Content licensing, copyright and retention rules.
Teacher onboarding fee versus recurring platform subscription behavior.
Subscription expiry/renewal rules, especially hourly access.
Final M-PESA provider contract and callback fields.
Virtual classroom/video infrastructure contract.
Assessment grading and result-override rules.
Data retention/deletion periods following legal review.
Expected initial and 3-year learner/teacher/content volumes.
Production RPO/RTO and availability targets.
40. Senior Engineering Position
The correct starting point is one PostgreSQL database with strong domain boundaries, not a collection of disconnected databases. The proposed model is deliberately comprehensive while remaining extensible. PostgreSQL owns durable business state; Redis, RabbitMQ, Hangfire, object storage, OpenSearch and video infrastructure have clear supporting roles.
The database must preserve historical facts rather than repeatedly overwriting them. Teacher verification decisions, content versions, assessment attempts, bookings, subscription events, payment transactions, refunds, safeguarding decisions and audit events therefore have explicit history structures.
This design provides the foundation for the Kenyan rollout and later international expansion while retaining the ability to extract high-load domains into independent services when actual scale, team boundaries or operational requirements justify it.
41. Final Architecture Summary
Layer
Technology
Responsibility
Application
ASP.NET Core + C#
Business logic / transactions
System of Record
PostgreSQL
Durable relational state
Logical isolation
PostgreSQL schemas/modules
Domain boundaries
Cache
Redis
Fast derived/ephemeral state
Events
RabbitMQ
Async messaging
Jobs
Hangfire
Background processing
Files
MinIO/S3-compatible
Documents and media
Search
OpenSearch
Derived search index
Video
HLS/DASH infrastructure
Streaming
Observability
OpenTelemetry + Prometheus + Grafana
Monitoring/tracing


42. Approval / Sign-Off
Role
Name
Comments / Approval
Date
Phronesis Management






Academic Lead






Product / Project Lead






Senior Software Engineer / Architect






Backend Lead






Database Engineer / DBA






QA Lead








