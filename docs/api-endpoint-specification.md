PHRONESIS HOMESCHOOL
END-TO-END API ENDPOINT SPECIFICATION
Foundational REST / OpenAPI Implementation Blueprint
Item
Value
Backend
ASP.NET Core + C#
API
REST + OpenAPI; gRPC internally where justified
Architecture
Modular Monolith → services only when justified
Infrastructure
PostgreSQL, Redis, RabbitMQ, Hangfire, S3-compatible storage, OpenSearch
Consumers
Next.js initially; future mobile clients/integrations
Base path
/api/v1
Status
Foundational endpoint inventory
Rule
Exact DTOs, validation rules and schemas are finalized in OpenAPI/SRS.



1. Purpose
This document defines the end-to-end REST API surface required to support the Phronesis Homeschool platform. It groups endpoints by business module and places those modules in the recommended dependency-aware implementation order. It is intended to guide backend engineering, frontend integration, QA, security review and later service extraction.
The inventory covers the target platform, not only the MVP. Future endpoints may be deferred without changing the architectural direction.
2. API Engineering Principles
Version all public APIs under /api/v1.
Keep business rules in the backend; the Next.js client must not be the source of truth for access, payment, booking or academic rules.
Enforce authentication and object-level authorization on every protected resource.
Use REST resources for CRUD and explicit command/action endpoints for state transitions.
Use pagination, filtering, sorting and stable public identifiers for collections.
Use idempotency for payment initiation, webhooks and retry-sensitive commands.
Use asynchronous jobs for long-running processing.
Use OpenAPI as the contract between ASP.NET Core and frontend clients.
Use RabbitMQ for domain/integration events; do not expose internal events as REST merely for convenience.
Audit significant administrative, financial, verification, publishing and security actions.
Design APIs so a future mobile application can consume the same backend without duplicating business logic.
3. Standard API Conventions
Area
Standard
Base
/api/v1
IDs
Opaque public IDs / UUIDs
Auth
Bearer access tokens using OIDC/OAuth2-compatible identity
Errors
RFC 9457-style Problem Details with stable error code + correlation ID
Pagination
page + pageSize initially; cursor pagination for high-volume streams later
Filtering
Query parameters such as gradeId, subjectId, topicId, status
Sorting
sortBy + sortDirection
Time
ISO 8601; store canonical timestamps in UTC; convert for class schedules
Files
Upload sessions/pre-signed URLs rather than large JSON requests
Concurrency
ETag/If-Match or resource version where needed
Payments
Idempotency-Key + verified provider callbacks
Async
202 Accepted + job resource/status for long operations


4. Phase 0 — M00/M01/M02 Platform Foundation
Operational endpoints that establish the API runtime and infrastructure contract.
Method
Endpoint
Auth / Role
Purpose
GET
/health
Internal
Liveness
GET
/health/ready
Internal
Readiness and dependency health
GET
/api/v1/meta
Public
API/platform metadata
GET
/api/v1/config/public
Public
Safe public configuration
GET
/api/v1/system/status
Admin
Platform status
GET
/api/v1/jobs/{jobId}
Authenticated
Background job status
POST
/api/v1/jobs/{jobId}/cancel
Authorized
Cancel cancellable job




5. Phase 1 — M03 Identity & Authentication
Account creation, authentication and session lifecycle.
Method
Endpoint
Auth / Role
Purpose
POST
/api/v1/auth/register
Public
Register account
POST
/api/v1/auth/login
Public
Authenticate
POST
/api/v1/auth/logout
Authenticated
Logout
POST
/api/v1/auth/refresh
Refresh token
Refresh access token
POST
/api/v1/auth/verify-email
Public
Verify email
POST
/api/v1/auth/resend-email-verification
Public
Resend verification
POST
/api/v1/auth/request-password-reset
Public
Start password reset
POST
/api/v1/auth/reset-password
Public
Complete password reset
POST
/api/v1/auth/change-password
Authenticated
Change password
GET
/api/v1/auth/me
Authenticated
Current identity
GET
/api/v1/auth/sessions
Authenticated
Active sessions
DELETE
/api/v1/auth/sessions/{sessionId}
Authenticated
Revoke session
POST
/api/v1/auth/sessions/revoke-all
Authenticated
Revoke other sessions
POST
/api/v1/auth/phone/send-verification
Authenticated
Send phone verification
POST
/api/v1/auth/phone/verify
Authenticated
Verify phone


6. Phase 1 — M04 Authorization & M05 Staff Administration
RBAC, permissions and internal staff management.
Method
Endpoint
Auth / Role
Purpose
GET
/api/v1/roles
Admin
List roles
POST
/api/v1/roles
Admin
Create role
GET
/api/v1/roles/{roleId}
Admin
Get role
PATCH
/api/v1/roles/{roleId}
Admin
Update role
DELETE
/api/v1/roles/{roleId}
Admin
Deactivate role
GET
/api/v1/permissions
Admin
List permissions
GET
/api/v1/users/{userId}/roles
Admin
User roles
PUT
/api/v1/users/{userId}/roles
Admin
Assign/replace roles
GET
/api/v1/users/{userId}/permissions
Admin
Effective permissions
GET
/api/v1/staff
Admin
List staff
POST
/api/v1/staff
Admin
Create staff
GET
/api/v1/staff/{staffId}
Admin
Get staff
PATCH
/api/v1/staff/{staffId}
Admin
Update staff
POST
/api/v1/staff/{staffId}/suspend
Admin
Suspend staff
POST
/api/v1/staff/{staffId}/activate
Admin
Activate staff


7. Phase 2 — M06 Learners & Guardians
Learner accounts, academic profile and guardian relationships.
Method
Endpoint
Auth / Role
Purpose
GET
/api/v1/learners
Staff/Admin
List learners
POST
/api/v1/learners
Staff/Admin
Create learner
GET
/api/v1/learners/{learnerId}
Learner/Guardian/Staff
Get learner
PATCH
/api/v1/learners/{learnerId}
Learner/Guardian/Staff
Update allowed profile
POST
/api/v1/learners/{learnerId}/activate
Admin
Activate
POST
/api/v1/learners/{learnerId}/suspend
Admin
Suspend
GET
/api/v1/learners/{learnerId}/guardians
Authorized
List relationships
POST
/api/v1/learners/{learnerId}/guardians
Guardian/Staff
Add guardian
PATCH
/api/v1/learners/{learnerId}/guardians/{relationshipId}
Authorized
Update relationship
DELETE
/api/v1/learners/{learnerId}/guardians/{relationshipId}
Authorized
Remove relationship
GET
/api/v1/me/learner-profile
Learner
Own learner profile
PATCH
/api/v1/me/learner-profile
Learner
Update learner preferences


8. Phase 2 — M07 Teachers
Teacher profiles, qualifications, subjects and availability.
Method
Endpoint
Auth / Role
Purpose
GET
/api/v1/teachers
Public/Authorized
List/search teachers
POST
/api/v1/teachers
Admin/Onboarding
Create teacher
GET
/api/v1/teachers/{teacherId}
Authorized/Public subset
Teacher profile
PATCH
/api/v1/teachers/{teacherId}
Teacher/Admin
Update profile
GET
/api/v1/teachers/{teacherId}/qualifications
Teacher/Reviewer/Admin
Qualifications
POST
/api/v1/teachers/{teacherId}/qualifications
Teacher/Admin
Add qualification
PATCH
/api/v1/teachers/{teacherId}/qualifications/{qualificationId}
Teacher/Admin
Update qualification
DELETE
/api/v1/teachers/{teacherId}/qualifications/{qualificationId}
Teacher/Admin
Remove qualification
GET
/api/v1/teachers/{teacherId}/subjects
Teacher/Admin
Teaching subjects
PUT
/api/v1/teachers/{teacherId}/subjects
Teacher/Admin
Set subjects
GET
/api/v1/teachers/{teacherId}/availability
Teacher/Admin
Get availability
PUT
/api/v1/teachers/{teacherId}/availability
Teacher
Set availability
GET
/api/v1/me/teacher-profile
Teacher
Own teacher profile


9. Phase 2 — M08 Teacher Onboarding, Verification & Compliance
End-to-end teacher application and professional verification workflow.
Method
Endpoint
Auth / Role
Purpose
POST
/api/v1/teacher-applications
Public/Authenticated
Create application
GET
/api/v1/teacher-applications/{applicationId}
Applicant/Reviewer/Admin
Application status
PATCH
/api/v1/teacher-applications/{applicationId}
Applicant
Edit before submission
POST
/api/v1/teacher-applications/{applicationId}/submit
Applicant
Submit application
POST
/api/v1/teacher-applications/{applicationId}/documents
Applicant
Create document upload
GET
/api/v1/teacher-applications/{applicationId}/documents
Applicant/Reviewer
List documents
DELETE
/api/v1/teacher-applications/{applicationId}/documents/{documentId}
Applicant/Reviewer
Remove/replace document
GET
/api/v1/admin/teacher-applications
Reviewer/Admin
Review queue
POST
/api/v1/admin/teacher-applications/{applicationId}/assign
Reviewer/Admin
Assign reviewer
POST
/api/v1/admin/teacher-applications/{applicationId}/request-correction
Reviewer
Request correction
POST
/api/v1/admin/teacher-applications/{applicationId}/approve
Reviewer/Admin
Approve
POST
/api/v1/admin/teacher-applications/{applicationId}/reject
Reviewer/Admin
Reject
POST
/api/v1/admin/teachers/{teacherId}/suspend
Admin
Suspend teacher
POST
/api/v1/admin/teachers/{teacherId}/revoke-verification
Admin
Revoke verification
GET
/api/v1/admin/teachers/{teacherId}/verification-history
Reviewer/Admin
Verification history


10. Phase 3 — M09/M10 Curriculum & Academic Taxonomy
Structured CBC hierarchy used by every content, learning and assessment module.
Method
Endpoint
Auth / Role
Purpose
GET
/api/v1/curricula
Public/Academic
List curricula
POST
/api/v1/curricula
Academic Admin
Create curriculum
GET
/api/v1/curricula/{curriculumId}
Public/Authorized
Get curriculum
PATCH
/api/v1/curricula/{curriculumId}
Academic Admin
Update curriculum
POST
/api/v1/curricula/{curriculumId}/publish
Academic Admin
Publish version
GET
/api/v1/curricula/{curriculumId}/grades
Public/Authorized
List grades
POST
/api/v1/curricula/{curriculumId}/grades
Academic Admin
Create grade
PATCH
/api/v1/grades/{gradeId}
Academic Admin
Update grade
GET
/api/v1/grades/{gradeId}/subjects
Public/Authorized
Grade subjects
POST
/api/v1/grades/{gradeId}/subjects
Academic Admin
Attach subject
DELETE
/api/v1/grades/{gradeId}/subjects/{subjectId}
Academic Admin
Detach subject
GET
/api/v1/subjects
Public/Authorized
List/search subjects
POST
/api/v1/subjects
Academic Admin
Create subject
GET
/api/v1/subjects/{subjectId}
Public/Authorized
Get subject
PATCH
/api/v1/subjects/{subjectId}
Academic Admin
Update subject
GET
/api/v1/subjects/{subjectId}/topics
Public/Authorized
List topics
POST
/api/v1/subjects/{subjectId}/topics
Academic Admin
Create topic
GET
/api/v1/topics/{topicId}
Public/Authorized
Get topic
PATCH
/api/v1/topics/{topicId}
Academic Admin
Update topic
POST
/api/v1/topics/{topicId}/children
Academic Admin
Create subtopic
GET
/api/v1/topics/{topicId}/resources
Authorized
Mapped resources
GET
/api/v1/academic/catalog
Public
Full academic catalog


11. Phase 4 — M11 Content Management
Educational resources, metadata, versions and access classification.
Method
Endpoint
Auth / Role
Purpose
GET
/api/v1/resources
Public/Authenticated
List/search resources
POST
/api/v1/resources
Teacher/Content Staff
Create resource
GET
/api/v1/resources/{resourceId}
Entitled/Authorized
Get resource
PATCH
/api/v1/resources/{resourceId}
Owner/Content Staff
Update resource
DELETE
/api/v1/resources/{resourceId}
Owner/Admin
Archive/delete
POST
/api/v1/resources/{resourceId}/versions
Owner/Content Staff
Create version
GET
/api/v1/resources/{resourceId}/versions
Authorized
List versions
GET
/api/v1/resources/{resourceId}/versions/{versionId}
Authorized
Get version
PUT
/api/v1/resources/{resourceId}/classification
Content Staff
Set grade/subject/topic
PUT
/api/v1/resources/{resourceId}/access
Content Staff/Admin
Set free/premium policy
POST
/api/v1/resources/{resourceId}/submit-review
Teacher/Content Staff
Submit review
POST
/api/v1/resources/{resourceId}/publish
Publisher/Admin
Publish
POST
/api/v1/resources/{resourceId}/unpublish
Publisher/Admin
Unpublish
POST
/api/v1/resources/{resourceId}/archive
Content/Admin
Archive


12. Phase 4 — M12 Content Review & Publishing
Academic quality assurance and publication workflow.
Method
Endpoint
Auth / Role
Purpose
GET
/api/v1/content-review/queue
Reviewer
Review queue
GET
/api/v1/content-review/{reviewId}
Reviewer/Author
Review details
POST
/api/v1/resources/{resourceId}/reviews
Reviewer
Create review
POST
/api/v1/resources/{resourceId}/request-changes
Reviewer
Request changes
POST
/api/v1/resources/{resourceId}/approve
Reviewer
Approve
POST
/api/v1/resources/{resourceId}/reject
Reviewer
Reject
GET
/api/v1/resources/{resourceId}/review-history
Reviewer/Admin/Author
Review history


13. Phase 4 — M13 Media & Secure Content Delivery
Media upload, processing and premium-content access.
Method
Endpoint
Auth / Role
Purpose
POST
/api/v1/media/upload-sessions
Authorized
Create upload session
POST
/api/v1/media/upload-sessions/{sessionId}/complete
Uploader
Complete upload
GET
/api/v1/media/{mediaId}
Authorized
Media metadata
GET
/api/v1/resources/{resourceId}/access
Entitled/Authorized
Resolve access
POST
/api/v1/resources/{resourceId}/playback-session
Entitled
Create short-lived playback authorization
POST
/api/v1/resources/{resourceId}/download-session
Entitled/Authorized
Create controlled download authorization
GET
/api/v1/resources/{resourceId}/watermark
Authorized
Resolve watermark/view policy
GET
/api/v1/files/{fileId}/status
Authorized
Processing status


14. Phase 4 — M14 Search & Discovery
OpenSearch-backed content and teacher discovery.
Method
Endpoint
Auth / Role
Purpose
GET
/api/v1/search/resources
Public/Authenticated
Search resources
GET
/api/v1/search/teachers
Public
Search teachers
GET
/api/v1/search/subjects
Public
Search subjects
GET
/api/v1/search/topics
Public
Search topics
GET
/api/v1/discover/featured
Public
Featured resources
GET
/api/v1/discover/recent
Public
Recently added
GET
/api/v1/discover/popular
Public/Authenticated
Popular resources
GET
/api/v1/discover/recommended
Authenticated
Recommendations where available


15. Phase 5 — M15 Learner Learning Workspace
Learner dashboard, library, activity and class/subscription views.
Method
Endpoint
Auth / Role
Purpose
GET
/api/v1/me/dashboard
Authenticated
Dashboard
GET
/api/v1/me/subjects
Learner
My subjects
GET
/api/v1/me/topics
Learner
Relevant topics
GET
/api/v1/me/learning/continue
Learner
Continue learning
GET
/api/v1/me/bookmarks
Learner
Bookmarks
POST
/api/v1/me/bookmarks
Learner
Bookmark resource
DELETE
/api/v1/me/bookmarks/{resourceId}
Learner
Remove bookmark
GET
/api/v1/me/recent-resources
Learner
Recent resources
POST
/api/v1/resources/{resourceId}/activity
Authenticated
Record learning activity
GET
/api/v1/me/classes/upcoming
Learner
Upcoming classes
GET
/api/v1/me/subscriptions
Authenticated
Subscriptions
GET
/api/v1/me/entitlements
Authenticated
Effective entitlements


16. Phase 5 — M16 Assessments, Questions & Exams
Question banks, topical practice, exams, attempts and results.
Method
Endpoint
Auth / Role
Purpose
GET
/api/v1/question-banks
Teacher/Reviewer/Admin
List banks
POST
/api/v1/question-banks
Teacher/Reviewer/Admin
Create bank
GET
/api/v1/question-banks/{bankId}
Authorized
Get bank
PATCH
/api/v1/question-banks/{bankId}
Owner/Admin
Update bank
GET
/api/v1/question-banks/{bankId}/questions
Authorized
List questions
POST
/api/v1/question-banks/{bankId}/questions
Teacher/Reviewer/Admin
Create question
GET
/api/v1/questions/{questionId}
Authorized
Get question
PATCH
/api/v1/questions/{questionId}
Owner/Reviewer/Admin
Update question
DELETE
/api/v1/questions/{questionId}
Owner/Admin
Archive question
GET
/api/v1/assessments
Authorized
List assessments
POST
/api/v1/assessments
Teacher/Content Staff
Create assessment
GET
/api/v1/assessments/{assessmentId}
Authorized
Get assessment
PATCH
/api/v1/assessments/{assessmentId}
Owner/Admin
Update assessment
POST
/api/v1/assessments/{assessmentId}/publish
Authorized
Publish assessment
POST
/api/v1/assessments/{assessmentId}/attempts
Learner
Start attempt
GET
/api/v1/attempts/{attemptId}
Learner/Authorized Teacher
Get attempt
PUT
/api/v1/attempts/{attemptId}/answers/{questionId}
Learner
Save answer
POST
/api/v1/attempts/{attemptId}/submit
Learner
Submit attempt
GET
/api/v1/attempts/{attemptId}/result
Learner/Teacher/Guardian
Get result
GET
/api/v1/me/assessment-history
Learner
Assessment history


17. Phase 5 — M17 Progress & Learning Analytics
Learning progress and performance views.
Method
Endpoint
Auth / Role
Purpose
GET
/api/v1/learners/{learnerId}/progress
Learner/Guardian/Teacher/Staff
Progress
GET
/api/v1/learners/{learnerId}/progress/topics
Authorized
Topic progress
GET
/api/v1/learners/{learnerId}/performance
Authorized
Performance
GET
/api/v1/learners/{learnerId}/activity
Authorized
Activity timeline
GET
/api/v1/me/progress
Learner
Own progress
GET
/api/v1/me/performance
Learner
Own performance
GET
/api/v1/me/activity
Learner
Own activity


18. Phase 6 — M18 Subscriptions, Plans & Entitlements
Commercial plans and access entitlement lifecycle.
Method
Endpoint
Auth / Role
Purpose
GET
/api/v1/plans
Public
Active plans
POST
/api/v1/plans
Admin
Create plan
GET
/api/v1/plans/{planId}
Public/Authorized
Get plan
PATCH
/api/v1/plans/{planId}
Admin
Update plan
POST
/api/v1/plans/{planId}/activate
Admin
Activate
POST
/api/v1/plans/{planId}/deactivate
Admin
Deactivate
GET
/api/v1/subscriptions
Owner/Admin
List subscriptions
POST
/api/v1/subscriptions
Authenticated
Purchase subscription
GET
/api/v1/subscriptions/{subscriptionId}
Owner/Admin
Get subscription
POST
/api/v1/subscriptions/{subscriptionId}/cancel
Owner/Admin
Cancel
POST
/api/v1/subscriptions/{subscriptionId}/renew
Owner/Admin
Renew
GET
/api/v1/learners/{learnerId}/entitlements
Authorized
Learner entitlements
GET
/api/v1/teachers/{teacherId}/entitlements
Authorized
Teacher entitlements


19. Phase 6 — M19 Payments & M-PESA
Payment abstraction and provider callbacks.
Method
Endpoint
Auth / Role
Purpose
GET
/api/v1/payment-methods
Authenticated
Available payment methods
POST
/api/v1/payments
Authenticated
Initiate payment
GET
/api/v1/payments/{paymentId}
Owner/Admin
Payment status
POST
/api/v1/payments/{paymentId}/retry
Owner/Admin
Retry failed payment
POST
/api/v1/payments/mpesa/stk-push
Authenticated
Initiate M-PESA STK
POST
/api/v1/payments/webhooks/mpesa
Provider-authenticated
Receive M-PESA callback
POST
/api/v1/payments/webhooks/{provider}
Provider-authenticated
Future provider callback
GET
/api/v1/payments/{paymentId}/receipt
Owner/Admin
Payment receipt


20. Phase 6 — M20 Orders, Invoices, Refunds & Reconciliation
Financial records and operational finance.
Method
Endpoint
Auth / Role
Purpose
GET
/api/v1/orders
Owner/Admin
List orders
POST
/api/v1/orders
System/Authorized
Create order
GET
/api/v1/orders/{orderId}
Owner/Admin
Get order
GET
/api/v1/invoices
Owner/Admin
List invoices
GET
/api/v1/invoices/{invoiceId}
Owner/Admin
Get invoice
GET
/api/v1/invoices/{invoiceId}/download
Owner/Admin
Download invoice
POST
/api/v1/refunds
Finance/Admin
Process refund
GET
/api/v1/refunds/{refundId}
Finance/Admin/Owner
Get refund
GET
/api/v1/admin/finance/reconciliation
Finance/Admin
Reconciliation view
POST
/api/v1/admin/finance/reconciliation/run
Finance/Admin
Run reconciliation


21. Phase 7 — M21 Virtual Classes & Sessions
Class offerings, teachers, sessions and class lifecycle.
Method
Endpoint
Auth / Role
Purpose
GET
/api/v1/classes
Public/Authorized
List offerings
POST
/api/v1/classes
Teacher/Admin
Create class
GET
/api/v1/classes/{classId}
Public/Authorized
Get class
PATCH
/api/v1/classes/{classId}
Teacher/Admin
Update class
POST
/api/v1/classes/{classId}/publish
Teacher/Admin
Publish
POST
/api/v1/classes/{classId}/unpublish
Teacher/Admin
Unpublish
GET
/api/v1/classes/{classId}/sessions
Authorized
List sessions
POST
/api/v1/classes/{classId}/sessions
Teacher/Admin
Create session
GET
/api/v1/sessions/{sessionId}
Authorized
Get session
PATCH
/api/v1/sessions/{sessionId}
Teacher/Admin
Update session
POST
/api/v1/sessions/{sessionId}/cancel
Teacher/Admin
Cancel session


22. Phase 7 — M22 Scheduling & Booking
Teacher availability, time slots and learner booking.
Method
Endpoint
Auth / Role
Purpose
GET
/api/v1/teachers/{teacherId}/availability
Public/Authorized
Get availability
PUT
/api/v1/teachers/{teacherId}/availability
Teacher
Set recurring availability
GET
/api/v1/classes/{classId}/availability
Public/Authorized
Available slots
GET
/api/v1/sessions/{sessionId}/availability
Public/Authorized
Session availability
POST
/api/v1/bookings
Learner/Guardian
Create booking
GET
/api/v1/bookings/{bookingId}
Owner/Teacher/Staff
Get booking
GET
/api/v1/me/bookings
Learner/Teacher
Own bookings
POST
/api/v1/bookings/{bookingId}/confirm
System/Authorized
Confirm booking
POST
/api/v1/bookings/{bookingId}/reschedule
Owner/Teacher/Admin
Reschedule
POST
/api/v1/bookings/{bookingId}/cancel
Owner/Teacher/Admin
Cancel
GET
/api/v1/admin/bookings
Admin/Staff
Booking administration


23. Phase 7 — M23 Live Classroom
Secure video/classroom access and session controls.
Method
Endpoint
Auth / Role
Purpose
POST
/api/v1/sessions/{sessionId}/join
Booked Learner/Teacher
Create join context
GET
/api/v1/sessions/{sessionId}/classroom
Booked Learner/Teacher
Classroom state
POST
/api/v1/sessions/{sessionId}/start
Teacher
Start
POST
/api/v1/sessions/{sessionId}/end
Teacher
End
POST
/api/v1/sessions/{sessionId}/recording
Teacher/Admin
Configure recording
GET
/api/v1/sessions/{sessionId}/recording
Authorized
Recording metadata/access


24. Phase 7 — M24 Attendance, Session Records & Feedback
Attendance, teacher notes and learner feedback.
Method
Endpoint
Auth / Role
Purpose
GET
/api/v1/sessions/{sessionId}/attendance
Teacher/Admin
Attendance list
POST
/api/v1/sessions/{sessionId}/attendance
Teacher/Admin/System
Record attendance
PATCH
/api/v1/sessions/{sessionId}/attendance/{attendanceId}
Teacher/Admin
Correct attendance
POST
/api/v1/sessions/{sessionId}/feedback
Learner/Teacher
Submit feedback
GET
/api/v1/sessions/{sessionId}/feedback
Teacher/Admin
View feedback
POST
/api/v1/sessions/{sessionId}/notes
Teacher
Submit notes
GET
/api/v1/sessions/{sessionId}/notes
Teacher/Admin/Authorized
Get notes


25. Phase 8 — M25 Teacher Contributions & Collaboration
Teacher-authored resources and collaborative content workflows.
Method
Endpoint
Auth / Role
Purpose
GET
/api/v1/teachers/{teacherId}/resources
Teacher/Admin
Teacher portfolio
POST
/api/v1/teachers/{teacherId}/resources
Teacher
Create contribution
GET
/api/v1/contributions
Teacher/Reviewer/Admin
List contributions
GET
/api/v1/contributions/{contributionId}
Authorized
Get contribution
PATCH
/api/v1/contributions/{contributionId}
Owner/Reviewer
Update
POST
/api/v1/contributions/{contributionId}/submit
Teacher
Submit
POST
/api/v1/contributions/{contributionId}/request-collaboration
Teacher/Authorized
Request collaborator
POST
/api/v1/contributions/{contributionId}/collaborators
Owner/Admin
Add collaborator
DELETE
/api/v1/contributions/{contributionId}/collaborators/{userId}
Owner/Admin
Remove collaborator
GET
/api/v1/contributions/{contributionId}/comments
Authorized
List comments
POST
/api/v1/contributions/{contributionId}/comments
Collaborator/Reviewer
Add comment


26. Phase 8 — M26 Notifications & Announcements
In-app/email/SMS-capable notification orchestration.
Method
Endpoint
Auth / Role
Purpose
GET
/api/v1/notifications
Authenticated
List notifications
GET
/api/v1/notifications/unread-count
Authenticated
Unread count
POST
/api/v1/notifications/{notificationId}/read
Authenticated
Mark read
POST
/api/v1/notifications/read-all
Authenticated
Mark all read
GET
/api/v1/me/notification-preferences
Authenticated
Preferences
PUT
/api/v1/me/notification-preferences
Authenticated
Update preferences
GET
/api/v1/announcements
Public/Authenticated
Announcements
POST
/api/v1/admin/announcements
Admin/Staff
Create announcement
GET
/api/v1/admin/announcements/{announcementId}
Admin/Staff
Get announcement
PATCH
/api/v1/admin/announcements/{announcementId}
Admin/Staff
Update
POST
/api/v1/admin/announcements/{announcementId}/publish
Admin/Staff
Publish


27. Phase 8 — M27 Support / Helpdesk
Support ticket lifecycle.
Method
Endpoint
Auth / Role
Purpose
POST
/api/v1/tickets
Authenticated
Create ticket
GET
/api/v1/tickets
Owner/Support/Admin
List tickets
GET
/api/v1/tickets/{ticketId}
Owner/Support/Admin
Get ticket
POST
/api/v1/tickets/{ticketId}/messages
Owner/Support
Add message
POST
/api/v1/tickets/{ticketId}/assign
Support/Admin
Assign
POST
/api/v1/tickets/{ticketId}/status
Support/Admin
Change status
POST
/api/v1/tickets/{ticketId}/close
Owner/Support/Admin
Close
GET
/api/v1/admin/tickets
Support/Admin
Support queue


28. Phase 9 — M28 Administration & Configuration
Operational control plane for the platform.
Method
Endpoint
Auth / Role
Purpose
GET
/api/v1/admin/users
Admin
List/search users
GET
/api/v1/admin/users/{userId}
Admin
Get user
PATCH
/api/v1/admin/users/{userId}
Admin
Update user state
POST
/api/v1/admin/users/{userId}/suspend
Admin
Suspend
POST
/api/v1/admin/users/{userId}/activate
Admin
Activate
GET
/api/v1/admin/settings
Admin
System settings
PUT
/api/v1/admin/settings/{key}
Admin
Update setting
GET
/api/v1/admin/feature-flags
Admin
Feature flags
PUT
/api/v1/admin/feature-flags/{key}
Admin
Update flag
GET
/api/v1/admin/content/statistics
Admin
Content statistics
GET
/api/v1/admin/learning/statistics
Admin
Learning statistics
GET
/api/v1/admin/commerce/statistics
Admin
Commerce statistics
GET
/api/v1/admin/virtual-classes/statistics
Admin
Class statistics


29. Phase 9 — M29 Reporting & Business Intelligence
Management and operational reporting.
Method
Endpoint
Auth / Role
Purpose
GET
/api/v1/reports/learners
Admin/Authorized
Learner report
GET
/api/v1/reports/teachers
Admin/Authorized
Teacher report
GET
/api/v1/reports/content
Admin/Authorized
Content report
GET
/api/v1/reports/assessments
Admin/Authorized
Assessment report
GET
/api/v1/reports/subscriptions
Admin/Finance
Subscription report
GET
/api/v1/reports/payments
Admin/Finance
Revenue/payment report
GET
/api/v1/reports/classes
Admin/Authorized
Class report
GET
/api/v1/reports/attendance
Admin/Authorized
Attendance report
GET
/api/v1/reports/learning-progress
Admin/Authorized
Progress report
POST
/api/v1/reports/export
Admin/Authorized
Start export job
GET
/api/v1/reports/exports/{jobId}
Admin/Authorized
Export status


30. Phase 9 — M30 Audit, Compliance & Data Governance
Audit trail and user data-rights workflows.
Method
Endpoint
Auth / Role
Purpose
GET
/api/v1/audit-logs
Admin/Auditor
Search audit logs
GET
/api/v1/audit-logs/{auditId}
Admin/Auditor
Get audit event
GET
/api/v1/resources/{resourceId}/audit
Admin/Reviewer
Content audit
GET
/api/v1/teachers/{teacherId}/audit
Admin/Reviewer
Teacher audit
GET
/api/v1/payments/{paymentId}/audit
Finance/Admin
Payment audit
POST
/api/v1/me/data-export
Authenticated
Request data export
GET
/api/v1/me/data-export/{jobId}
Authenticated
Export status
POST
/api/v1/me/deletion-request
Authenticated
Request deletion
GET
/api/v1/me/deletion-request
Authenticated
Deletion status
GET
/api/v1/admin/data-requests
Privacy/Admin
Data request queue
POST
/api/v1/admin/data-requests/{requestId}/approve
Authorized
Approve request
POST
/api/v1/admin/data-requests/{requestId}/reject
Authorized
Reject request


31. Phase 9 — M31 Security, Safeguarding & Abuse
Trust/safety controls for a platform serving minors.
Method
Endpoint
Auth / Role
Purpose
POST
/api/v1/reports
Authenticated
Report user/content/behavior
GET
/api/v1/my-reports
Authenticated
Own submitted reports where permitted
POST
/api/v1/users/{userId}/block
Authenticated
Block user where applicable
DELETE
/api/v1/users/{userId}/block
Authenticated
Unblock
GET
/api/v1/admin/reports
Trust/Admin
Review reports
GET
/api/v1/admin/reports/{reportId}
Trust/Admin
Get report
POST
/api/v1/admin/reports/{reportId}/assign
Trust/Admin
Assign case
POST
/api/v1/admin/reports/{reportId}/resolve
Trust/Admin
Resolve case
POST
/api/v1/admin/users/{userId}/restrict
Trust/Admin
Restrict account
POST
/api/v1/admin/users/{userId}/unrestrict
Trust/Admin
Remove restriction
GET
/api/v1/admin/security/events
Security/Admin
Security events


32. Cross-Cutting Integration Endpoints
Method
Endpoint
Auth / Role
Purpose
POST
/api/v1/webhooks/{provider}/{eventType}
Provider-authenticated
Generic verified webhook intake where justified
GET
/api/v1/integrations/status
Admin
Integration health
POST
/api/v1/search/reindex
Admin/System
Start search reindex
GET
/api/v1/search/reindex/{jobId}
Admin/System
Reindex status
GET
/api/v1/notifications/templates
Admin
Notification templates
POST
/api/v1/notifications/test
Admin
Test delivery


33. Endpoint Coverage by Frontend Surface
Frontend Surface
Primary API Modules
Public Website
M03, M09–M14, M18, M21, M26
Authentication
M03–M04
Learner Portal
M06, M09–M24, M26–M27
Guardian Portal (if approved)
M06, M17–M24, M26–M27
Teacher Arena
M07–M08, M09–M13, M21–M27
Academic Reviewer
M08, M12, M16, M25
Content Manager
M09–M14, M25
Admin Portal
M04–M05, M08–M33
Management Dashboard
M28–M31
Future Mobile
Same versioned REST API


34. Critical End-to-End API Flows
34.1 Learner Registration → Subscription → Content
POST /api/v1/auth/register
POST /api/v1/auth/verify-email
GET /api/v1/auth/me
GET /api/v1/plans
POST /api/v1/subscriptions
POST /api/v1/payments or /api/v1/payments/mpesa/stk-push
POST /api/v1/payments/webhooks/mpesa
GET /api/v1/me/entitlements
GET /api/v1/search/resources
GET /api/v1/resources/{resourceId}
POST /api/v1/resources/{resourceId}/playback-session or /download-session
34.2 Teacher Application → Verification → Teaching
POST /api/v1/teacher-applications
POST /api/v1/teacher-applications/{id}/documents
POST /api/v1/teacher-applications/{id}/submit
GET /api/v1/admin/teacher-applications
POST /api/v1/admin/teacher-applications/{id}/assign
POST /api/v1/admin/teacher-applications/{id}/approve
Teacher profile/subjects/availability APIs
Class creation and scheduling APIs
34.3 Content → Review → Publication → Learner
POST /api/v1/resources
POST /api/v1/media/upload-sessions
POST /api/v1/media/upload-sessions/{id}/complete
PUT /api/v1/resources/{id}/classification
PUT /api/v1/resources/{id}/access
POST /api/v1/resources/{id}/submit-review
GET /api/v1/content-review/queue
POST /api/v1/resources/{id}/approve
POST /api/v1/resources/{id}/publish
GET /api/v1/search/resources
GET /api/v1/resources/{id}/access
POST /api/v1/resources/{id}/playback-session
34.4 Virtual Class Booking → Payment → Session → Attendance
GET /api/v1/classes/{classId}/availability
POST /api/v1/bookings
POST /api/v1/payments or payment abstraction flow
POST /api/v1/payments/webhooks/mpesa
POST /api/v1/bookings/{bookingId}/confirm
GET /api/v1/me/bookings
POST /api/v1/sessions/{sessionId}/join
POST /api/v1/sessions/{sessionId}/start
POST /api/v1/sessions/{sessionId}/attendance
POST /api/v1/sessions/{sessionId}/end
POST /api/v1/sessions/{sessionId}/feedback
35. Minimum Role Model
Role
Typical Scope
Anonymous
Public catalog, public profiles, pricing
Learner
Own profile, entitled learning, assessments, progress, bookings and payments
Guardian
Linked learner information and approved purchasing/progress
Teacher Applicant
Own application and onboarding documents
Verified Teacher
Teaching profile, resources, classes, sessions and assigned learners
Academic Reviewer
Teacher/content academic verification
Content Manager
Content lifecycle and academic catalog operations
Support Agent
Support workflows
Finance/Admin
Payments, subscriptions, reconciliation
Platform Admin
Users, roles, settings and platform administration
Super Admin
Restricted platform-wide authority
Security/Privacy
Security, safeguarding and data-rights functions


36. Security Requirements for the API
Apply server-side RBAC and object-level authorization to every protected resource.
Rate-limit login, password reset, payment, webhook-adjacent and abuse-report endpoints.
Verify M-PESA/provider webhook authenticity and make processing idempotent.
Never expose storage credentials or permanent premium-content URLs.
Use short-lived playback/download authorization.
Protect teacher certification/identity documents with dedicated authorization.
Audit financial, verification, publishing, permission and administrative state changes.
Use correlation IDs across HTTP, RabbitMQ and Hangfire.
Validate uploaded files and restrict executable/unsafe content.
Minimize personal data returned by list endpoints.
Apply safeguarding restrictions to communication and user-reporting features.
37. Recommended Endpoint Implementation Order
API middleware, OpenAPI, validation, Problem Details, correlation IDs and error handling.
Authentication and current-user APIs.
RBAC and authorization policies.
Learner, guardian, teacher and staff APIs.
Teacher onboarding and verification.
Curriculum, grades, subjects and topics.
Content, media, review and publishing.
Search and discovery.
Learner workspace.
Assessments and progress.
Subscriptions and entitlements.
Payments, M-PESA callbacks and financial records.
Virtual classes, scheduling and bookings.
Live classroom and attendance.
Teacher collaboration, notifications and support.
Administration, reporting, audit and safeguarding.
Mobile client consumption and future/advanced APIs.
38. Definition of Done for an Endpoint
Business purpose and acceptance criteria approved.
Request/response schema documented in OpenAPI.
Authentication and authorization policy defined.
Validation and error cases defined.
Database/domain behavior implemented.
Idempotency/concurrency behavior defined where relevant.
Audit/event behavior defined where relevant.
Unit and integration tests implemented.
Critical frontend/end-to-end flow tested.
Logging, metrics and correlation ID included.
Security review completed for sensitive endpoints.
Documentation and API client updated.
39. What Must Be Finalized Before Full Implementation
Exact JSON request/response DTOs and OpenAPI component schemas.
Final role-permission matrix.
Final database ERD and module ownership boundaries.
Exact authentication/OIDC implementation.
M-PESA provider contract, credentials and webhook verification.
Video infrastructure and HLS/DASH service contract.
Content licensing, copyright and download/watermark policy.
Guardian-account rules.
Subscription renewal, cancellation and refund rules.
Safeguarding and escalation procedures.
Rate limits, quotas and retention policies.
40. Recommended Next Technical Artifacts
Formal OpenAPI 3.x specification derived from this endpoint inventory.
Shared schema catalogue: User, Role, Permission, Pagination, ProblemDetails, Resource, Subscription, Payment, Booking, Assessment and AuditEvent.
PostgreSQL ERD/domain model.
ASP.NET Core modular project structure and dependency rules.
Complete authorization policy matrix.
Generated TypeScript API client strategy for Next.js.
RabbitMQ event contract catalogue.
Hangfire job catalogue with retry/idempotency rules.
M-PESA abstraction and callback specification.
Media upload/processing/playback architecture.
Automated API contract and integration test plan.
Implementation backlog mapped one-to-one to these modules and endpoints.
41. Final Engineering Position
This endpoint inventory is the initial API map for Phronesis. The backend should remain a modular monolith initially, while the endpoint and domain boundaries are designed so high-load modules can later be extracted without redesigning the entire frontend.
The frontend should consume these APIs and never duplicate authoritative business rules. Subscription entitlement, premium-content access, teacher verification, booking eligibility, payment state, assessment rules and safeguarding must be enforced by ASP.NET Core.
The next implementation step is not to immediately code every endpoint. First freeze the shared conventions, domain model, authorization matrix and OpenAPI schemas. Then implement each module in the phase order defined here, with tests and frontend integration at every completed boundary.
42. Validation / Sign-Off
Role
Name
Comments / Approval
Date
Phronesis Management






Academic Lead






Product / Project Lead






Senior Software Engineer / Architect






Backend Lead






Frontend Lead






QA Lead








