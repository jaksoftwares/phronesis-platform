PHRONESIS PLATFORM
Backend Foundation & Complete Platform Structure Specification
Repository
phronesis-platform
Frontend Repository
phronesis-frontend
Backend Technology
ASP.NET Core
C#
.NET 10
PostgreSQL
Redis
RabbitMQ
Hangfire
MinIO / S3-compatible object storage
OpenSearch
Docker
OpenTelemetry
Prometheus
Grafana
GitHub Actions
1. BACKEND ROLE

phronesis-platform is the central backend engine for the entire Phronesis ecosystem.

It must provide:

authentication
authorization
user management
learner management
guardian management
teacher management
staff management
curriculum management
academic structure
educational content
resource management
learning
assessments
examinations
subscriptions
payments
M-PESA
virtual classes
scheduling
bookings
attendance
notifications
communication
administration
reporting
audit
search
media management
background processing
platform configuration
security
future AI/ML integration

The backend should therefore be designed as a platform, not merely an API for the first website.

2. HIGH-LEVEL ARCHITECTURE

The architecture should initially be:

                    PHRONESIS FRONTEND
                    phronesis-frontend
                           │
                           │ HTTPS / REST
                           ▼
              ┌─────────────────────────┐
              │    PHRONESIS PLATFORM   │
              │     ASP.NET CORE API    │
              └────────────┬────────────┘
                           │
        ┌──────────────────┼──────────────────┐
        │                  │                  │
        ▼                  ▼                  ▼
   PostgreSQL           Redis             RabbitMQ
  System of Record      Cache          Async Messaging
        │                                     │
        │                                     ▼
        │                                Hangfire
        │                                Background
        │                                  Jobs
        │
        ├───────────────┐
        ▼               ▼
   MinIO / S3       OpenSearch
   Media Storage       Search

Supporting:

OpenTelemetry
Prometheus
Grafana
Centralized Logging
3. CORE ARCHITECTURAL PRINCIPLE

Do not create:

phronesis-identity
phronesis-learners
phronesis-teachers
phronesis-payments
phronesis-content

as separate repositories or APIs.

Instead:

phronesis-platform
│
├── Identity
├── Learners
├── Guardians
├── Teachers
├── Staff
├── Curriculum
├── Content
├── Learning
├── Assessments
├── Subscriptions
├── Payments
├── Classes
├── Scheduling
├── Bookings
├── Notifications
├── Administration
├── Reporting
├── Search
└── Platform

These are modules inside one backend.

4. RECOMMENDED ROOT STRUCTURE

The initial repository should eventually look like:

phronesis-platform/
│
├── src/
│
│   ├── Phronesis.Api/
│   │
│   ├── Phronesis.Application/
│   │
│   ├── Phronesis.Domain/
│   │
│   ├── Phronesis.Infrastructure/
│   │
│   └── Phronesis.Shared/
│
├── tests/
│   │
│   ├── Phronesis.UnitTests/
│   ├── Phronesis.IntegrationTests/
│   └── Phronesis.ArchitectureTests/
│
├── infrastructure/
│   │
│   ├── docker/
│   ├── postgres/
│   ├── redis/
│   ├── rabbitmq/
│   ├── minio/
│   └── opensearch/
│
├── docs/
│
├── scripts/
│
├── .github/
│   └── workflows/
│
├── docker-compose.yml
├── .env.example
├── .gitignore
├── Directory.Build.props
├── Directory.Packages.props
├── Phronesis.sln
└── README.md
5. LAYER ARCHITECTURE

There should be four primary architectural layers.

API

Responsible for:

HTTP
routing
controllers
authentication middleware
authorization
request/response handling
API versioning
OpenAPI
HTTP-specific concerns

It should not contain business logic.

Application

Responsible for:

use cases
commands
queries
DTOs
validation
orchestration
application services
transaction coordination

Example:

RegisterLearner
ApplyAsTeacher
PublishResource
BookClass
SubscribeToPlan
InitiateMpesaPayment
SubmitAssessment
Domain

Responsible for:

business entities
aggregates
value objects
domain rules
domain events
business invariants

The domain should not know about:

PostgreSQL
Redis
RabbitMQ
MinIO
HTTP
ASP.NET
Infrastructure

Responsible for:

PostgreSQL
EF Core
Redis
RabbitMQ
Hangfire
MinIO
OpenSearch
M-PESA integration
email
SMS
external services
persistence
background processing
6. DOMAIN MODULES

These are the major backend modules.

They should be established from the beginning even if some are initially empty.

Identity
Users
Learners
Guardians
Teachers
Staff
Curriculum
Content
Media
Learning
Assessments
Examinations
Subscriptions
Commerce
Payments
Classes
Scheduling
Bookings
Attendance
Notifications
Communication
Administration
Reporting
Search
Audit
Security
Support
Safeguarding
Platform
7. IDENTITY MODULE

This is foundational and should be implemented first.

Responsibilities:

User accounts
Authentication
Authorization
Credentials
Sessions
Refresh tokens
Password management
Email verification
Phone verification
MFA
Account status
Roles
Permissions
Security events

Potential structure:

Identity/
├── Users/
├── Credentials/
├── Sessions/
├── Roles/
├── Permissions/
├── Authentication/
├── Authorization/
├── Verification/
├── PasswordManagement/
├── MultiFactorAuthentication/
└── SecurityEvents/
8. PEOPLE MODULES

Separate identity from people's platform profiles.

Learners
Learner profile
Grade
Academic information
Guardian relationships
Learning preferences
Status
Enrollment
Guardians
Guardian profile
Learner relationships
Contact information
Responsibilities
Teachers
Teacher profile
Professional information
Subjects
Grades
Experience
Availability
Verification status
Staff
Employee profile
Department
Position
Permissions
Status
9. TEACHER ONBOARDING

This deserves its own module.

Teacher Application
       ↓
Personal Information
       ↓
Professional Information
       ↓
Qualifications
       ↓
Certification Documents
       ↓
Experience
       ↓
Subject/Grade Selection
       ↓
Review
       ↓
Verification
       ↓
Approval / Rejection
       ↓
Teacher Account Activation

The backend must support:

application
document upload
qualification records
certification verification
reviewer assignment
review notes
approval
rejection
suspension
re-verification
onboarding fee
teacher subscription
10. CURRICULUM MODULE

This is one of the most important modules.

Structure:

Curriculum
    ↓
Education Level
    ↓
Grade
    ↓
Subject
    ↓
Strand / Category
    ↓
Topic
    ↓
Subtopic
    ↓
Learning Objective

It must support CBC initially while allowing future curricula.

Do not hard-code:

Grade 7
Grade 8
Grade 9

throughout the application.

The academic structure must be data-driven.

11. CONTENT MODULE

This controls Phronesis' educational intellectual property.

Resources include:

notes
revision materials
topical questions
past papers
exams
study guides
assignments
worksheets
videos
animations
presentations
documents
premium resources

Structure:

Content
│
├── Resources
├── ResourceVersions
├── Categories
├── Tags
├── CurriculumMappings
├── ContentReview
├── Publishing
├── AccessControl
├── Copyright
└── Licensing

Resources should have lifecycle states:

Draft
Submitted
UnderReview
Approved
Published
Archived
Rejected
12. MEDIA MODULE

Do not store large media directly in PostgreSQL.

Use:

PostgreSQL
     │
     └── MediaAsset metadata
                │
                ▼
          MinIO / S3

Support:

PDF
DOCX
PPTX
Images
Audio
Video
Animations

Store:

File name
MIME type
Size
Checksum
Storage key
Owner
Uploaded by
Created date
Processing status
Visibility
13. VIDEO MODULE

Video is sufficiently specialized to deserve its own domain boundary.

Support:

Video upload
Processing
Transcoding
HLS
DASH
Thumbnails
Captions
Subtitles
Video metadata
Playback authorization
Progress tracking

Do not make the initial backend responsible for performing huge video encoding operations synchronously.

Use:

Upload
 ↓
Queue
 ↓
Worker
 ↓
Video Processing
 ↓
Storage
 ↓
Streaming Manifest
14. LEARNING MODULE

This represents the learner's actual learning journey.

Support:

Learning workspace
Enrolled subjects
Learning plans
Learning goals
Progress
Resource consumption
Bookmarks
Recently viewed
Learning history
Topic completion

Potential structure:

Learning/
├── Workspaces/
├── Enrollments/
├── Progress/
├── Activities/
├── Goals/
├── Bookmarks/
└── History/
15. ASSESSMENT MODULE

Support:

Question banks
Questions
Question types
Options
Answers
Assessments
Assignments
Quizzes
Attempts
Responses
Marking
Results
Feedback

Question types should be extensible:

Multiple Choice
Multiple Select
True/False
Short Answer
Long Answer
Matching
Ordering
16. EXAMINATION MODULE

Separate general assessments from formal examinations where appropriate.

Support:

Exam creation
Exam scheduling
Candidate registration
Exam attempts
Time limits
Submission
Automatic marking
Manual marking
Results
Grades
Performance analysis
17. SUBSCRIPTIONS MODULE

Phronesis has two major subscription ecosystems.

Learner Arena
1 Hour
1 Day
1 Week
1 Month
1 Year
Teachers Arena
1 Week
1 Month
1 Year

The backend must support configurable plans rather than hard-coding prices.

SubscriptionPlan
Subscription
SubscriptionPeriod
SubscriptionStatus
Entitlement
18. ENTITLEMENT SYSTEM

This is critical.

A subscription should not directly control every resource.

Instead:

Subscription
      ↓
Entitlement
      ↓
Feature / Resource / Service

For example:

Active Monthly Subscription
        ↓
Can access Grade 8 resources
        ↓
Can access premium notes
        ↓
Can book virtual classes

This makes the commercial model flexible.

19. PAYMENTS MODULE

Payment architecture should be abstracted.

Payment
   ↓
Payment Provider
   ├── M-PESA
   ├── Future Provider
   └── Future Provider

The core platform should not be tightly coupled to one payment provider.

Support:

Payment Intent
Payment Transaction
Provider Reference
Callback
Payment Status
Verification
Reconciliation
Refund
Receipt
20. M-PESA MODULE

M-PESA should sit behind the payment abstraction.

Support:

STK Push
Payment initiation
Callback
Callback validation
Transaction lookup
Payment verification
Failed payments
Timeouts
Duplicate callbacks
Reconciliation

Important:

Never activate a subscription simply because the frontend says payment succeeded.

The backend must independently establish payment state.

21. VIRTUAL CLASSES MODULE

Support:

Class
Class type
Subject
Topic
Teacher
Capacity
Duration
Price
Status

Example:

Mathematics
Grade 8
Algebra
Teacher
1 hour
KES 250
22. SCHEDULING MODULE

Scheduling should be separate from classes.

Support:

Teacher availability
Working hours
Blocked periods
Class schedules
Recurring schedules
Time zones
Conflicts
Rescheduling
Cancellation
23. BOOKING MODULE

Support:

Class booking
Booking status
Payment status
Cancellation
Rescheduling
Capacity
Booking confirmation
Booking history

Flow:

Learner
 ↓
Select Class
 ↓
Select Available Slot
 ↓
Payment
 ↓
Booking Confirmation
 ↓
Class Access
24. ATTENDANCE MODULE

Support:

Session attendance
Teacher attendance
Learner attendance
Join time
Leave time
Attendance status
Manual corrections
Attendance reports
25. NOTIFICATION MODULE

Centralize notifications.

Channels:

In-app
Email
SMS
Push notifications

Events may include:

Account verified
Teacher approved
Payment completed
Subscription expiring
Class booked
Class reminder
Class cancelled
Assessment marked
New resource published

Notifications should normally be asynchronous.

26. COMMUNICATION MODULE

Support:

Announcements
System messages
Teacher-to-learner communication
Administrative communication
Email templates
SMS templates
Notification templates

If direct messaging is introduced later:

Conversations
Participants
Messages
Attachments
Read status
Moderation
27. ADMINISTRATION MODULE

This is the internal control center.

Support:

Dashboard
User management
Learner management
Teacher management
Content management
Curriculum management
Subscription management
Payment management
Class management
Booking management
Notifications
Reports
Configuration
Audit
28. CONTENT GOVERNANCE

Because Phronesis wants protected premium educational content, implement:

Content ownership
Content review
Approval workflow
Publication
Access control
Download permissions
View permissions
Copyright metadata
Watermarking capability
Access logging

Do not assume that hiding a PDF URL provides security.

The architecture should support authorized content delivery.

29. SEARCH MODULE

OpenSearch should support:

Resource search
Subject search
Topic search
Teacher search
Class search
Assessment search

Example:

"Grade 8 Mathematics Algebra"

should return relevant:

Topics
Notes
Topicals
Past papers
Videos
Classes

PostgreSQL remains the source of truth.

OpenSearch is a search/read projection.

30. REPORTING MODULE

Reporting should eventually support:

Learner
Progress
Performance
Attendance
Assessment results
Resource usage
Teacher
Classes
Learners taught
Content contributions
Performance
Sessions
Management
Revenue
Subscriptions
Learners
Teachers
Classes
Content
Engagement
Retention
Performance

Do not overload transactional tables with expensive analytical queries forever. The architecture should leave room for reporting projections/data marts later.

31. AUDIT MODULE

Every sensitive operation should be auditable.

Examples:

Teacher approved
Teacher suspended
Resource published
Resource deleted
Payment verified
Subscription modified
User role changed
Assessment result changed
Admin login
Permission changed

Audit records should contain:

Actor
Action
Entity
Entity ID
Timestamp
IP/context where appropriate
Correlation ID
Before/after information where appropriate
32. SECURITY MODULE

Central security infrastructure should include:

Authentication
Authorization
Rate limiting
Account lockout
Session management
Token management
Security events
Suspicious activity
API protection
Input validation
File validation
Audit logging

Sensitive secrets must never be stored in Git.

33. SUPPORT MODULE

The platform should eventually support:

Support tickets
Categories
Priority
Status
Assigned staff
Messages
Attachments
Resolution

This becomes important once Phronesis has thousands of learners.

34. SAFEGUARDING MODULE

Because this is an education platform serving minors, this should be treated as a first-class architectural concern.

Support appropriate controls around:

Learner safety
Teacher interactions
Communication restrictions
Reporting incidents
Moderation
Staff review
Access restrictions
Auditability

The exact policies should be defined with Phronesis management and legal/compliance advisors.

35. PLATFORM MODULE

This contains cross-cutting configuration.

Examples:

System settings
Feature flags
Supported countries
Supported currencies
Academic configuration
Platform configuration
Maintenance mode
API configuration

Avoid hard-coding business configuration into C#.

36. DATABASE

PostgreSQL is the system of record.

It should contain structured business data.

Logical schema separation can be:

identity
people
academic
content
learning
assessment
commerce
payments
classes
booking
notifications
administration
audit
support
security
platform

Example:

identity.users

people.learners
people.teachers
people.guardians

academic.curricula
academic.grades
academic.subjects
academic.topics

content.resources
content.resource_versions

learning.progress

assessment.questions
assessment.assessments
assessment.attempts

commerce.subscription_plans
commerce.subscriptions
commerce.entitlements

payments.transactions

classes.classes
classes.sessions

booking.bookings
booking.attendance
37. REDIS

Redis should not replace PostgreSQL.

Use Redis for:

Caching
Rate limiting
Short-lived state
Temporary tokens
Distributed coordination
Frequently accessed data

Potential future use:

Live class state
Temporary booking locks
OTP state
API throttling
38. RABBITMQ

RabbitMQ handles asynchronous events.

Examples:

UserRegistered
TeacherApplicationSubmitted
TeacherApproved
ResourcePublished
PaymentCompleted
SubscriptionActivated
ClassBooked
AssessmentCompleted
NotificationRequested

This prevents the API from performing every operation synchronously.

39. HANGFIRE

Hangfire handles background jobs.

Examples:

Send email
Send SMS
Expire subscriptions
Generate reports
Process media
Generate thumbnails
Process documents
Send reminders
Clean temporary files
Retry failed operations
40. MINIO / S3

Object storage should hold:

Documents
PDFs
Images
Videos
Animations
Teacher certificates
Other educational media

The database stores metadata and references.

41. OPENSEARCH

OpenSearch should contain searchable projections.

Do not make OpenSearch the authoritative database.

Architecture:

PostgreSQL
    ↓
Domain Event
    ↓
RabbitMQ
    ↓
Search Projection Worker
    ↓
OpenSearch
42. API STRUCTURE

All APIs should be versioned:

/api/v1/

Major groups:

/api/v1/auth
/api/v1/users
/api/v1/learners
/api/v1/guardians
/api/v1/teachers
/api/v1/staff
/api/v1/curriculum
/api/v1/subjects
/api/v1/topics
/api/v1/resources
/api/v1/media
/api/v1/learning
/api/v1/assessments
/api/v1/examinations
/api/v1/subscriptions
/api/v1/payments
/api/v1/classes
/api/v1/schedules
/api/v1/bookings
/api/v1/attendance
/api/v1/notifications
/api/v1/communications
/api/v1/reports
/api/v1/admin
/api/v1/support
43. API DOCUMENTATION

The backend must generate OpenAPI documentation.

Developers should be able to see:

Endpoint
Method
Authentication
Parameters
Request body
Response
Errors
Authorization requirements

The frontend team should consume the API contract rather than guessing endpoint behavior.

44. API RESPONSE STANDARD

Every API should follow consistent response conventions.

Success:

{
  "data": {},
  "message": "Operation completed successfully",
  "traceId": "..."
}

Collection:

{
  "data": [],
  "pagination": {
    "page": 1,
    "pageSize": 20,
    "total": 100
  },
  "traceId": "..."
}

Error:

{
  "type": "...",
  "title": "...",
  "status": 400,
  "code": "...",
  "errors": [],
  "traceId": "..."
}

The exact contract can be standardized during implementation.

45. GLOBAL MIDDLEWARE

The API foundation should include:

Exception Handling
Request Logging
Correlation ID
Authentication
Authorization
Rate Limiting
Request Validation
Security Headers

A request should conceptually flow:

Request
 ↓
Correlation ID
 ↓
Security
 ↓
Authentication
 ↓
Authorization
 ↓
Validation
 ↓
Controller
 ↓
Application
 ↓
Domain
 ↓
Infrastructure
 ↓
Response
46. DOMAIN EVENTS

Establish domain-event infrastructure from the beginning.

Examples:

UserRegistered
LearnerCreated
TeacherApplicationSubmitted
TeacherApproved
ResourcePublished
PaymentCompleted
SubscriptionActivated
ClassBooked
AssessmentSubmitted
ExamCompleted

These can eventually trigger:

Notifications
Search indexing
Analytics
Emails
Background jobs
Other modules
47. OUTBOX PATTERN

For critical events:

Database Transaction
       │
       ├── Business record
       │
       └── Outbox event
                  │
                  ▼
             Event Worker
                  │
                  ▼
              RabbitMQ

This is especially important for:

Payments
Subscriptions
Bookings
Teacher approvals
Content publishing
48. TRANSACTIONS

Business operations involving multiple database changes should be transactional.

For example:

Payment completed
      ↓
Create transaction
      ↓
Activate subscription
      ↓
Create entitlement
      ↓
Record audit

These should not leave the database in a half-completed state.

49. CONCURRENCY

Pay particular attention to:

Class capacity
Bookings
Payments
Subscription activation
Assessment submissions
Inventory-like resources
Teacher scheduling

For example, two learners should not successfully book the final available seat simultaneously.

50. TESTING STRUCTURE

Three levels:

Unit tests

Business rules.

TeacherApprovalTests
SubscriptionTests
BookingTests
PaymentTests
AssessmentTests
Integration tests
API
PostgreSQL
Redis
RabbitMQ
Architecture tests

Verify rules such as:

Domain cannot depend on Infrastructure
Application cannot depend on API
Modules cannot bypass boundaries

This is particularly valuable for a modular monolith.

51. HEALTH CHECKS

Implement:

/health
/health/live
/health/ready

Readiness should verify required infrastructure.

For example:

PostgreSQL
Redis
RabbitMQ
52. OBSERVABILITY

Implement:

OpenTelemetry
Prometheus
Grafana
Structured Logging
Correlation IDs
Distributed Tracing

Monitor:

API latency
Error rate
Database latency
Queue depth
Background jobs
Memory
CPU
Storage
Payment failures
Authentication failures
53. SECURITY LOGGING

Security events should be separately identifiable.

Examples:

LoginSuccess
LoginFailure
PasswordChanged
PasswordReset
AccountLocked
MfaEnabled
RoleChanged
PermissionChanged
SuspiciousRequest
54. FILE SECURITY

Any upload endpoint must validate:

Extension
MIME type
File size
File signature
Virus/malware scanning capability
Storage path
User authorization

Never trust the filename supplied by the client.

55. ENVIRONMENT STRUCTURE

Support:

Development
Testing
Staging
Production

Configuration should never be embedded into source code.

Example:

PostgreSQL
Redis
RabbitMQ
MinIO
OpenSearch
M-PESA
Email
JWT
OAuth

must all be externally configurable.

56. LOCAL DEVELOPMENT

A developer should ideally be able to clone the repository and run:

docker compose up -d

then:

dotnet restore
dotnet build
dotnet test
dotnet run

and have the backend connect automatically to:

PostgreSQL
Redis
RabbitMQ
MinIO
OpenSearch
57. DOCKER DEVELOPMENT ENVIRONMENT

Docker Compose should initially provide:

phronesis-postgres
phronesis-redis
phronesis-rabbitmq
phronesis-minio
phronesis-opensearch

The backend itself can also have a Docker image:

phronesis-api

This makes the system Kubernetes-ready later.

58. CI/CD

GitHub Actions should eventually perform:

Checkout
 ↓
Restore
 ↓
Build
 ↓
Unit Tests
 ↓
Integration Tests
 ↓
Architecture Tests
 ↓
Security Scan
 ↓
Docker Build
 ↓
Push Image
 ↓
Deploy

Do not deploy code that has not passed automated tests.

59. DATABASE MIGRATIONS

Use EF Core migrations.

All schema changes must be:

Version controlled
Reviewable
Repeatable
Deployable
Rollback-considered

Never allow developers to casually modify production tables manually.

60. SEED DATA

The initial seed system should establish:

System roles
Permissions
Curricula
Grades
Subjects
Initial academic structure
Subscription plans
System configuration

For example:

SuperAdmin
Admin
ContentManager
Teacher
Learner
Guardian
SupportAgent
FinanceOfficer
AcademicCoordinator

The final roles should be determined from the business requirements.

61. AUDITABILITY

Sensitive business operations must generate audit records.

Especially:

Payments
Teacher verification
Content publishing
Content deletion
Subscription changes
Role changes
Permission changes
Assessment results
Administrative actions
62. FRONTEND/BACKEND CONTRACT

phronesis-frontend should never directly access:

PostgreSQL
Redis
RabbitMQ
MinIO
OpenSearch

It should communicate only with:

phronesis-platform API

Architecture:

phronesis-frontend
       │
       │ HTTPS
       ▼
phronesis-platform
       │
       ├── PostgreSQL
       ├── Redis
       ├── RabbitMQ
       ├── MinIO
       └── OpenSearch

This is extremely important.

63. FUTURE MOBILE APPLICATION

Even though mobile may not exist yet, design the API so that later:

phronesis-frontend
phronesis-mobile
future-partner-app

can all consume:

phronesis-platform

The backend should therefore not contain frontend-specific business logic.

64. FUTURE MICROSERVICE EXTRACTION

The architecture must make it possible to eventually extract:

Video Service
Search Service
Notification Service
Payment Service
AI Service
Analytics Service

without rewriting the whole platform.

Initially:

                    Phronesis Platform
                         │
              Modular Monolith

Later:

                 Phronesis Core
                      │
       ┌──────────────┼──────────────┐
       ▼              ▼              ▼
 Payment Service   Video Service   AI Service

Only extract a module when there is a real reason.

65. WHAT ANTIGRAVITY SHOULD NOT DO

This is just as important as what it should do.

It should not:

create microservices immediately
create separate repositories per module
put business logic in controllers
put everything in one giant Models folder
put everything in one giant Services folder
connect the frontend directly to PostgreSQL
store videos in PostgreSQL
store secrets in Git
hard-code subscription prices
hard-code grades and subjects into application logic
couple payments directly to M-PESA
use Redis as the primary database
use OpenSearch as the source of truth
perform heavy video processing inside API requests
create hundreds of endpoints before establishing conventions
generate every database entity blindly without validating relationships
implement AI before the core platform is stable
66. WHAT SHOULD BE BUILT NOW

The initial Antigravity task should be infrastructure and architecture only.

It should establish:

1. Repository
2. Solution
3. Projects
4. Layer architecture
5. Module boundaries
6. Dependency rules
7. PostgreSQL
8. EF Core
9. Database configuration
10. Redis
11. RabbitMQ
12. Hangfire
13. MinIO
14. OpenSearch
15. Docker Compose
16. Environment configuration
17. Authentication foundation
18. Authorization foundation
19. API versioning
20. OpenAPI
21. Error handling
22. Validation
23. Logging
24. Correlation IDs
25. Health checks
26. OpenTelemetry foundation
27. Testing framework
28. Architecture tests
29. Database migrations
30. Seed framework
31. Domain events
32. Outbox foundation
33. CI/CD foundation
34. README
35. Developer documentation
67. WHAT SHOULD NOT BE IMPLEMENTED IN THIS INITIAL SETUP

Do not ask Antigravity to immediately implement all:

Learner endpoints
Teacher endpoints
Payment endpoints
Content endpoints
Class endpoints
Assessment endpoints

The foundation should first prove that:

ASP.NET Core
      ↓
Application
      ↓
Domain
      ↓
Infrastructure
      ↓
PostgreSQL

works correctly.

And that:

Redis
RabbitMQ
Hangfire
MinIO
OpenSearch

are properly integrated and available locally.

68. INITIAL SUCCESS CRITERIA

When Antigravity finishes the initial setup, you should be able to clone:

phronesis-platform

and execute something equivalent to:

docker compose up -d
dotnet restore
dotnet build
dotnet test
dotnet run

Then:

API
 ↓
/health

returns healthy.

OpenAPI is available.

PostgreSQL connects.

Redis connects.

RabbitMQ connects.

Hangfire starts.

MinIO starts.

OpenSearch starts.

Logging works.

A database migration can be executed.

A test can be written and passed.

And the architecture tests confirm that the dependency rules are being respected.

At that point, the backend is ready for actual feature development.

69. IMPLEMENTATION ORDER AFTER FOUNDATION

Once this foundation is complete, development should proceed:

PHASE 0
Backend Foundation
        ↓
PHASE 1
Identity & Authentication
        ↓
PHASE 2
Users / Learners / Guardians / Teachers / Staff
        ↓
PHASE 3
Teacher Onboarding & Verification
        ↓
PHASE 4
Curriculum
        ↓
PHASE 5
Educational Content
        ↓
PHASE 6
Learning Workspace
        ↓
PHASE 7
Assessments & Examinations
        ↓
PHASE 8
Subscriptions & Entitlements
        ↓
PHASE 9
Payments & M-PESA
        ↓
PHASE 10
Virtual Classes
        ↓
PHASE 11
Scheduling & Bookings
        ↓
PHASE 12
Attendance
        ↓
PHASE 13
Notifications & Communication
        ↓
PHASE 14
Administration
        ↓
PHASE 15
Reporting & Analytics
        ↓
PHASE 16
Search
        ↓
PHASE 17
Advanced Media / Video
        ↓
PHASE 18
AI / ML
70. THE MOST IMPORTANT CONCEPTUAL MODEL

The backend team should think of Phronesis as:

                         PHRONESIS PLATFORM
                                │
        ┌───────────────────────┼────────────────────────┐
        │                       │                        │
     PEOPLE                  ACADEMICS                COMMERCE
        │                       │                        │
 Learners                  Curriculum              Subscriptions
 Teachers                  Subjects                Payments
 Guardians                 Topics                  Entitlements
 Staff                     Content                 M-PESA
        │                       │                        │
        └───────────────────────┼────────────────────────┘
                                │
                           LEARNING
                                │
                 ┌──────────────┼──────────────┐
                 │              │              │
             Resources     Assessments       Classes
                 │              │              │
                 │              │              │
                 └──────────────┼──────────────┘
                                │
                         PLATFORM SERVICES
                                │
        ┌──────────────┬────────┼─────────┬──────────────┐
        │              │        │         │              │
     Identity      Payments   Search   Notifications   Media
        │              │        │         │              │
        └──────────────┴────────┼─────────┴──────────────┘
                                │
                         ADMINISTRATION
                                │
                 Audit • Reports • Security

That is the mental model I would give the engineering team.

Final recommendation

For your current situation, I would make the first Antigravity assignment explicitly:

"Initialize phronesis-platform as the production backend foundation. Do not implement business modules yet. Establish the modular-monolith architecture, project structure, dependency boundaries, local infrastructure, configuration system, database infrastructure, authentication foundation, API standards, observability, messaging, background jobs, object storage, search infrastructure, testing, migrations, Docker and CI/CD. All future Phronesis modules must plug into this architecture without restructuring the foundation."