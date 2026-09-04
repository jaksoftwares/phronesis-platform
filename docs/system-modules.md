PHRONESIS HOMESCHOOL
SYSTEM MODULE & IMPLEMENTATION BLUEPRINT
Senior Software Engineering Implementation Order
Item
Details
System
Phronesis Homeschool Digital Education Platform
Purpose
Complete module landscape and dependency-aware implementation sequence.
Approach
Foundation → Identity → Academic Core → Content → Learning → Commerce → Virtual Tuition → Operations → Scale.
Primary Users
Learners, guardians where applicable, teachers, reviewers, content managers, staff and administrators.
Initial Market
Kenya
Initial Curriculum
CBC Grades 7–12
Status
Implementation Blueprint / Technical Planning Baseline



1. Purpose
This blueprint translates the Phronesis Homeschool problem definition into a complete modular implementation plan. It is intended to guide senior software engineers, architects, frontend/backend engineers, UI/UX designers, QA, DevOps and project management.
The order is dependency-driven, not merely screen-driven. Foundations, identity, data and academic structures are established before dependent learning, commerce and virtual-tuition capabilities. This reduces architectural rework and creates a coherent platform.
2. Senior Engineering Principles
Build a modular platform rather than disconnected features.
Establish the domain model and authorization model before complex business interfaces.
Keep identity, roles, permissions, subscriptions and entitlements as separate concepts.
Treat curriculum and educational content as structured domain data.
Treat payments as a financial subsystem with auditable transaction records.
Treat teacher verification and content approval as explicit workflows and state machines.
Design APIs for future web and mobile clients.
Build security, auditability, testing, monitoring and backups from the beginning.
Phase advanced functionality without weakening the long-term domain architecture.
3. Complete Module Register
ID
Module
Phase
Priority
M00
Product, Architecture & Engineering Foundation
0
Critical
M01
Infrastructure, Environments & DevOps
0
Critical
M02
Database & Core Domain Model
0
Critical
M03
Identity, Authentication & Account Management
1
Critical
M04
Roles, Permissions & Access Control
1
Critical
M05
Organization, Staff & Administrative Structure
1
High
M06
Learner & Guardian Management
2
Critical
M07
Teacher Management & Professional Profiles
2
Critical
M08
Teacher Onboarding, Verification & Compliance
2
Critical
M09
Curriculum & Academic Taxonomy
3
Critical
M10
Subject, Topic & Academic Configuration
3
Critical
M11
Educational Content Management
4
Critical
M12
Content Review, Approval & Publishing
4
Critical
M13
Media, Documents & Secure Content Delivery
4
Critical
M14
Content Search, Discovery & Catalog
4
High
M15
Learner Learning Workspace
5
Critical
M16
Assessments, Topical Questions & Exams
5
Critical
M17
Learning Progress, Activity & Analytics
5
High
M18
Subscriptions, Plans & Entitlements
6
Critical
M19
Payments & Financial Transactions
6
Critical
M20
Orders, Invoices, Refunds & Reconciliation
6
High
M21
Virtual Classes & Session Management
7
Critical
M22
Class Booking, Scheduling & Availability
7
Critical
M23
Live Classroom / Video Integration
7
High
M24
Attendance, Session Records & Feedback
7
High
M25
Teacher Resource Contribution & Collaboration
8
High
M26
Communication & Notifications
8
High
M27
Support / Helpdesk & Issue Management
8
Medium
M28
Administration & Platform Configuration
9
Critical
M29
Reporting, Dashboards & Business Intelligence
9
High
M30
Audit Logs, Compliance & Governance
9
Critical
M31
Security, Safeguarding & Abuse Management
9
Critical
M32
Quality Assurance & Automated Testing
Cross-cutting
Critical
M33
Observability, Monitoring & Reliability
Cross-cutting
Critical
M34
Mobile / PWA Experience
10
High
M35
Advanced Personalization & Recommendation
11
Future
M36
Internationalization & Global Expansion
11
Future


4. Implementation Phases
Phase
Modules
Objective
Phase 0 – Foundation
M00–M02
Architecture, infrastructure and core data
Phase 1 – Identity
M03–M05
Authentication, authorization and governance
Phase 2 – User Domains
M06–M08
Learner and teacher ecosystems
Phase 3 – Academic Core
M09–M10
CBC curriculum structure
Phase 4 – Content
M11–M14
Content lifecycle, media and discovery
Phase 5 – Learning
M15–M17
Learner workspace, assessments and progress
Phase 6 – Commerce
M18–M20
Plans, entitlements and payments
Phase 7 – Virtual Tuition
M21–M24
Classes, booking, video and attendance
Phase 8 – Collaboration
M25–M27
Teacher contribution, notifications and support
Phase 9 – Operations
M28–M33
Administration, reporting, audit, security, QA and monitoring
Phase 10 – Mobile
M34
Mobile/PWA reach
Phase 11 – Advanced
M35–M36
Personalization and global expansion


5. Module Specifications
M00 – Product, Architecture & Engineering Foundation
Requirements baseline and MVP boundary
Domain/module boundaries
System context and architecture
API conventions/versioning
Coding standards and repository strategy
CI/CD and Definition of Done standards
Configuration, secrets, logging and error conventions
Data classification, retention and documentation standards
M01 – Infrastructure, Environments & DevOps
Application/runtime infrastructure
Development, staging and production environments
Database and object storage
TLS/domain/CDN strategy
CI/CD deployment pipeline
Secrets management
Migrations
Backups, restore and disaster recovery
M02 – Database & Core Domain Model
Users/identities
Roles/permissions
Learners/guardians
Teachers
Curriculum
Resources/media
Subscriptions/entitlements
Payments
Classes/bookings
Assessments/attempts
Notifications/audit
M03 – Identity, Authentication & Account Management
Registration
Login/logout
Email/phone verification
Password reset/account recovery
Sessions/tokens
Profile management
Account status and security controls
M04 – Roles, Permissions & Access Control
Role definitions
Permission definitions
Role-permission mapping
Object/resource authorization
Subscription-aware access
Administrative separation of duties
Authorization auditability
M05 – Organization, Staff & Administrative Structure
Phronesis organization profile
Staff accounts
Academic reviewers
Content managers
Support staff
Administrative responsibilities
M06 – Learner & Guardian Management
Learner profile
Grade/academic level
Subjects
Guardian relationships where applicable
Guardian visibility rules
Learner status
Subscription association
M07 – Teacher Management & Professional Profiles
Professional profile
Subjects/grades
Qualifications
Experience
Teaching skills
Availability
Verification state
Teacher status
M08 – Teacher Onboarding, Verification & Compliance
Application
Certification/document submission
Completeness checks
Reviewer workflow
Verification states
Interview/demo lesson where required
Approval/rejection/correction
Onboarding fees
Policy/agreement acceptance
Suspension/revocation
M09 – Curriculum & Academic Taxonomy
Curriculum
Education levels
Grades 7–12
Subjects
Strands/areas
Topics/subtopics
Learning objectives
Curriculum versioning
M10 – Subject, Topic & Academic Configuration
Subject metadata
Topic ordering
Grade-subject relationships
Topic prerequisites
Resource mapping
Assessment mapping
Teacher competence mapping
M11 – Educational Content Management
Create/upload
Rich metadata
Notes/past papers/questions/exams/videos/animations/worksheets
Grade/subject/topic classification
Free/premium status
Draft editing
Versioning
Ownership metadata
M12 – Content Review, Approval & Publishing
Submission
Reviewer assignment
Academic checklist
Comments/revisions
Approval/rejection
Publication scheduling
Version approval
Archiving
Review history
M13 – Media, Documents & Secure Content Delivery
Secure storage
Video processing
Streaming
Document viewing
Access-controlled delivery
Watermarking where appropriate
Download policy
Media lifecycle
M14 – Content Search, Discovery & Catalog
Search
Grade/subject/topic/resource filters
Free/premium filters
Ranking
Recently updated
Featured resources
M15 – Learner Learning Workspace
Dashboard
My subjects/topics
Continue learning
Bookmarks
Recent activity
Learning activities
Subscription-aware access
Class access
Notifications
M16 – Assessments, Topical Questions & Exams
Question bank
Question types
Topical exercises
Timed assessments
Mock exams
Attempts
Scoring
Answer review
Marking schemes
M17 – Learning Progress, Activity & Analytics
Resource engagement
Topic completion
Assessment performance
Attendance
Learning timeline
Progress dashboards
Role-controlled visibility
Aggregate analytics
M18 – Subscriptions, Plans & Entitlements
Learner plans
Teacher plans
Pricing/duration
Features/entitlements
Activation/expiry
Renewal
Cancellation
Suspension
Promotions where required
M19 – Payments & Financial Transactions
Payment initiation
Callbacks/webhooks
Server-side verification
Transaction status
Payment references
Payment-method abstraction
Failed-payment handling
Duplicate protection
M20 – Orders, Invoices, Refunds & Reconciliation
Orders
Receipts/invoices
Refunds
Cancellations
Reconciliation
Revenue reports
Exports
Financial audit trail
M21 – Virtual Classes & Session Management
Class creation
Teacher assignment
Subject/topic
Individual/group class types
Duration/capacity
Class status
Supporting resources
M22 – Class Booking, Scheduling & Availability
Teacher availability
Time slots
Learner booking
Confirmation
Payment/entitlement validation
Schedule publication
Rescheduling
Cancellation
Capacity
Conflict detection
M23 – Live Classroom / Video Integration
Video provider integration
Secure session access
Teacher classroom controls
Learner joining
Session tokens
Optional recording
Resource sharing
M24 – Attendance, Session Records & Feedback
Attendance
Completion
Teacher notes
Learner feedback
Ratings where approved
No-shows/cancellations
Session history
M25 – Teacher Resource Contribution & Collaboration
Resource submission
Shared workspaces
Co-authoring where needed
Reviewer feedback
Contribution history
Attribution
M26 – Communication & Notifications
In-app notifications
Email
SMS/other approved channels
Booking reminders
Payment notices
Expiry reminders
Verification notices
Announcements
Preferences
M27 – Support / Helpdesk & Issue Management
Support requests
Tickets
Assignment
Status
Internal notes
Escalation
Resolution history
M28 – Administration & Platform Configuration
User administration
Teacher administration
Academic configuration
Content configuration
Subscription plans
Class administration
Notifications
Feature flags
System settings
M29 – Reporting, Dashboards & Business Intelligence
Learner reports
Teacher reports
Content reports
Subscriptions
Revenue
Classes
Attendance
Assessments
Management dashboards
Exports
M30 – Audit Logs, Compliance & Governance
Administrative audit
Content lifecycle audit
Teacher verification audit
Payment audit
Permission-change logs
Sensitive-access logs
Retention controls
Data export/deletion workflows
M31 – Security, Safeguarding & Abuse Management
Abuse detection
Suspicious activity controls
Teacher/learner reporting
Safeguarding incidents
Communication controls
Blocking/suspension
Content abuse reporting
Security incident workflow
M32 – Quality Assurance & Automated Testing
Unit tests
Integration/API tests
End-to-end tests
Payment tests
Authorization tests
Content-access tests
Regression
Performance/load
Security testing
M33 – Observability, Monitoring & Reliability
Application logs
Error tracking
Infrastructure/database monitoring
Payment monitoring
Background-job monitoring
Uptime
Alerts
Performance metrics
Backup verification
M34 – Mobile / PWA Experience
Responsive web
PWA where appropriate
Mobile learning
Mobile assessments
Mobile class access
Push notifications for native apps
Optimized media
M35 – Advanced Personalization & Recommendation
Personalized paths
Recommended resources
Weak-topic identification
Adaptive practice
Activity-based recommendations
Advanced learner analytics
M36 – Internationalization & Global Expansion
Multiple currencies
Country payment methods
Time zones
Localization
Additional curricula
Regional teacher verification
International compliance considerations
6. Portal / Workspace Composition
Portal
Major Modules
Public Website
Marketing, organization, subjects, resource previews, pricing, registration, support, legal pages
Learner Portal
M03,M04,M06,M09–M20,M21–M24,M26,M27,M34
Guardian Portal (if approved)
M03,M04,M06,M17,M18–M20,M21–M24,M26,M27
Teacher Arena
M03,M04,M07,M08,M09–M13,M21–M26,M29–M31
Academic Reviewer
M04,M05,M08,M11,M12,M30,M31
Content Manager
M04,M05,M09–M14,M25,M28–M30
Admin Portal
M03–M05,M08–M33
Management Dashboard
M28–M33
Support Workspace
M04,M26,M27,M30,M31


7. Critical Dependency Chain
Architecture + infrastructure + database foundation.
Authentication + roles + permissions.
Learner, teacher and staff domain models.
Teacher onboarding and verification.
Curriculum and academic taxonomy.
Content management, review and secure media.
Learner workspace and content discovery.
Assessments and learning progress.
Subscriptions, entitlements and payments.
Virtual classes, booking and scheduling.
Live classroom integration.
Attendance and feedback.
Collaboration, notifications and support.
Administration, reporting, audit, safeguarding, QA and monitoring.
Mobile and advanced expansion.
8. Recommended Implementation Waves
Wave
Modules
Exit Criteria
0
M00–M02
Architecture approved; environments operational; database and deployment foundation working.
1
M03–M05
Secure authentication/RBAC and internal governance working.
2
M06–M08
Learner/teacher domains and complete teacher verification lifecycle working.
3
M09–M10
CBC grades, subjects and topics configurable.
4
M11–M14
Content can be created, reviewed, approved, published and securely discovered.
5
M15–M17
Learners can study, practice and view progress.
6
M18–M20
Plans, entitlements, payments, receipts and reconciliation work end-to-end.
7
M21–M24
Classes can be created, booked, scheduled, delivered and tracked.
8
M25–M27
Teacher contribution, notifications and support workflows operate.
9
M28–M33
Production operations, reporting, audit, safeguarding, testing and monitoring are ready.
10
M34
Mobile/PWA experience is production-ready.
11
M35–M36
Advanced personalization and global expansion introduced deliberately.


9. Recommended MVP Boundary
The first production release should prove the core value chain: verified teachers + structured educational content + learner access + paid services + virtual tuition + administration.
M00–M04: Foundation, infrastructure, identity and authorization.
M06–M08: Learner/teacher management and verification.
M09–M10: CBC academic taxonomy.
M11–M14: Content, review, secure delivery and discovery.
M15–M16: Learner workspace and core assessments.
M18–M19: Subscriptions, entitlements and payment integration.
M21–M22: Virtual classes, booking and scheduling.
M23–M24: Basic live-class integration and attendance.
M26: Essential notifications.
M28–M33: Minimum administration, reporting, audit, security, QA and observability.
10. Capabilities That Should Normally Be Post-MVP
Advanced AI tutoring and adaptive learning.
Sophisticated recommendation engines.
Full native mobile apps if responsive/PWA meets initial needs.
Large-scale social/community networking.
International multi-currency and multi-country operations.
Highly customized analytics beyond critical management reports.
Offline premium-content distribution unless validated and legally supported.
11. Cross-Cutting Engineering Requirements
Area
Must Be Present Throughout
Security
Authentication, authorization, secure secrets, encryption in transit, least privilege, abuse controls
Testing
Unit, integration, end-to-end, regression and performance testing
Observability
Structured logs, metrics, traces/error tracking and alerts
Data
Migrations, integrity constraints, backups, recovery and retention
Audit
Important administrative, content, teacher and financial actions
API
Versioning, validation, rate limits, consistent errors and documentation
UX
Responsive design, accessibility, clear loading/error/empty states
DevOps
CI/CD, environment isolation, rollback and deployment controls
Documentation
Architecture decisions, APIs, workflows, runbooks and module ownership


12. Definition of Done for Every Module
Business requirements and acceptance criteria approved.
Domain/database changes finalized.
API contracts documented where applicable.
Authorization rules implemented and tested.
Frontend workflow implemented where applicable.
Validation, error and empty states handled.
Audit requirements implemented.
Automated tests added.
Critical end-to-end tests added.
Security review completed for sensitive functionality.
Logging/monitoring implemented.
Documentation updated.
Staging deployment completed.
Product/client acceptance completed.
13. Recommended Engineering Team Workstreams
Workstream
Primary Responsibility
Modules
Architecture / Backend Core
Domain model, APIs, business logic, authorization
M00–M05,M09–M10,M18–M20,M28–M33
Frontend / Web
Public, learner, teacher, reviewer and admin interfaces
M03–M34
Academic / Content
Taxonomy, CMS, review and assessments
M09–M17
Commerce
Subscriptions, payments and reconciliation
M18–M20
Virtual Tuition
Classes, scheduling, booking, video and attendance
M21–M24
Platform / DevOps
Infrastructure, CI/CD, monitoring, backups
M01,M32–M33
QA / Automation
Test automation, regression, performance and security
M32
Product / UX
Journeys, requirements, design system and accessibility
All user-facing modules


14. Recommended Immediate Engineering Artifacts
Software Requirements Specification (SRS).
System context and architecture diagrams.
Detailed module-by-module functional requirements.
Role and permission matrix.
Complete database ERD/domain model.
User journeys and process flows.
API specification.
UI/UX information architecture and screen inventory.
Prioritized MVP backlog and user stories.
Sprint plan/Gantt.
QA and test strategy.
Infrastructure/deployment architecture.
Security, privacy and safeguarding requirements.
Content licensing and governance requirements.
15. Senior Engineering Conclusion
Phronesis Homeschool should be implemented as a digital education platform rather than a conventional website. Its architecture must support multiple actors, structured academic content, teacher verification, commercial entitlements, virtual tuition, learner progress and institutional operations.
The implementation order intentionally builds from the inside out: foundations → identity → users → academic model → content → learning → commerce → virtual tuition → collaboration → operations → mobile → advanced capabilities. This creates stable dependencies, minimizes rework and provides a clear path from MVP to a national and eventually global platform.
After client validation, every module should be decomposed into epics, user stories, acceptance criteria, database migrations, API endpoints, UI screens, permissions, test cases and deployment tasks.
16. Client / Engineering Validation
Role
Name
Approval / Comments
Date
Phronesis Representative






Academic Representative






Management Representative






Senior Software Engineer






Product/Project Lead








