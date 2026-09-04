PHRONESIS HOMESCHOOL
SYSTEM PROBLEM DEFINITION & DISCOVERY
Genesis of Knowledge
“Cream for Sprouting Minds”

Document Item
Details
Organization
Phronesis Homeschool
Location
Nairobi, Kenya
Email
phronesishomeschool@gmail.com
Telephone
0723 376 024 / 0113 093 087
Primary Audience
Learners, parents/guardians, teachers, administrators and Phronesis staff
Initial Market
Kenya
Long-Term Market
Global
Curriculum Focus
Kenyan Competency-Based Curriculum (CBC), with future extensibility
Document Purpose
Define the problem, business need, proposed solution boundary, requirements, actors, workflows, constraints, risks and questions requiring validation before detailed system design.
Status
Discovery / Requirements Definition – Draft for Client Validation



1. Executive Summary	4
2. Organization Context	4
2.1 Organization Profile	4
2.2 Core Values	4
2.3 Vision and Strategic Objectives	5
3. Problem Definition	5
3.1 Core Problem	5
3.2 Problem Statement – Formal Definition	5
3.3 The Problem in Practical Terms	6
4. Target Users and Stakeholders	6
5. Target Learner Segments	7
6. Proposed Solution Concept	7
6.1 Core Platform Pillars	7
7. Academic Content Model	8
7.1 Proposed Hierarchy	8
7.2 Resource Categories	8
7.3 Subject and Curriculum Coverage	9
8. Learner Portal – Problem and Capability Definition	9
9. Teacher Arena – Onboarding, Verification and Teaching	10
9.1 Proposed Teacher Onboarding Workflow	10
9.2 Teacher Eligibility Principles	10
9.3 Teacher Workspace	10
10. Virtual Classes and Tuition	11
11. Subscription and Commercial Model	11
11.1 Learners Arena – Initial Stated Plans	11
11.2 Teachers Arena – Initial Stated Plans	12
11.3 Entitlement Model	12
12. Content Protection, Integrity and Copyright	12
13. Content Management and Quality Assurance	13
14. Administration and Organizational Workspace	14
15. Collaboration and Communication	14
16. Mobile Education Programme	15
17. High-Level Functional Requirements	15
18. High-Level Non-Functional Requirements	17
19. Key Business Rules to Define	17
20. Proposed End-to-End User Journeys	18
20.1 Learner Journey	18
20.2 Teacher Journey	18
20.3 Content Journey	18
21. Scope Definition	19
21.1 Proposed Core MVP Scope	19
21.2 Recommended Later-Phase Capabilities	19
22. Risks and Challenges	20
23. Key Assumptions	21
24. Critical Questions Requiring Client Validation	21
25. Success Measures	22
26. Recommended System Architecture Domains	22
27. Data Entities – Initial Conceptual Model	23
28. Security, Privacy and Safeguarding Considerations	24
29. Recommended Delivery Strategy	25
30. Overall Problem-Solution Alignment	25
31. Conclusion	25
32. Document Approval / Validation	26

1. Executive Summary
Phronesis Homeschool intends to establish a comprehensive digital education platform that connects learners with structured, reliable and high-quality home-schooling resources and professional virtual teaching services. The platform is intended to serve learners undertaking the Kenyan CBC curriculum, initially focusing on Junior School (Grades 7–9) and Senior School (Grades 10–12), while being architected for expansion across Kenya and eventually into international markets.
The central problem is not simply the absence of educational content. The deeper problem is the fragmentation of learning resources, revision materials, qualified home-schooling support, teacher availability, virtual tuition, learner progress workflows and quality assurance. Learners and their parents/guardians may struggle to identify trustworthy resources and competent teachers, while teachers require a controlled environment in which they can teach, contribute resources, collaborate and demonstrate their capabilities.
The proposed platform therefore needs to function as an integrated digital learning ecosystem rather than a simple content website. It should provide learner workspaces, teacher workspaces, administrative controls, structured subject and topic libraries, premium learning resources, virtual-class booking and delivery, teacher onboarding and verification, subscriptions, resource contribution workflows, collaboration, communication, reporting and quality assurance.
This document converts the meeting notes and organization information supplied by Phronesis Homeschool into a coherent problem definition that can be used as the foundation for requirements analysis, UX/UI design, technical architecture, costing, project planning and subsequent software development.
2. Organization Context
2.1 Organization Profile
Phronesis Homeschool is positioned as an education organization committed to accessible, high-quality, Christ-centered education that combines academic excellence with practical wisdom, competence and character development.
2.2 Core Values
Inspiring Greatness, Changing Lives.
Building Knowledge, Developing Character.
Preparing Leaders.





2.3 Vision and Strategic Objectives
Objective
Interpretation for the Digital Platform
Accessibility
Make learning resources and educational support accessible to learners across Kenya, including learners in remote areas, and eventually to international users.
Competence
Provide current, structured and trustworthy learning materials that improve learner knowledge, confidence and practical competence.
Digital Learning
Deliver virtual classes, digital resources, video lessons, animations and mobile-accessible learning experiences.
Teacher Empowerment
Provide qualified teachers with opportunities to teach, contribute resources, collaborate and grow professionally within a controlled platform.
Structured Learning
Organize curriculum content progressively by grade, subject, strand/topic and resource type so learners can study independently and systematically.


3. Problem Definition
3.1 Core Problem
Learners following the Kenyan CBC curriculum can face difficulty obtaining a single, trusted and structured environment where they can access comprehensive learning resources, revision materials, topical exercises, examinations, professional home-schooling support and live virtual tuition. Educational resources may exist in many disconnected forms and channels, while access to qualified and verified teachers can be difficult to organize consistently.
At the same time, Phronesis Homeschool needs a reliable digital operating environment through which it can manage learners, teachers, educational content, subscriptions, virtual classes, bookings, resource approvals, quality assurance and organizational operations at scale.
Without an integrated platform, the organization risks fragmented service delivery, inconsistent content organization, difficulty verifying teachers, inefficient booking and communication processes, weak control over premium content, limited visibility into learner engagement and challenges in scaling beyond a largely manual or channel-dependent model.
3.2 Problem Statement – Formal Definition
Phronesis Homeschool requires a secure, scalable and structured digital education platform that enables learners to discover, subscribe to and consume curriculum-aligned learning resources and virtual tuition; enables Phronesis to onboard, verify, manage and support qualified teachers; and enables administrators to control content, subscriptions, classes, users, quality assurance and platform operations. The system must preserve educational quality, protect premium content, support controlled access and provide an experience that can scale from Kenya to international markets.
3.3 The Problem in Practical Terms
Learners need a dependable place to obtain comprehensive and organized learning materials instead of searching across disconnected sources.
Learners need structured content by grade, subject, topic and learning objective to support independent revision.
Learners and parents/guardians need access to professional and trustworthy home-schooling services.
Phronesis needs a controlled mechanism for virtual-class booking, scheduling, payment/subscription validation and participation.
Phronesis needs a rigorous teacher onboarding and verification process to reduce the risk of unqualified or unsuitable teachers.
Teachers need a dedicated workspace for teaching, accessing approved materials, contributing resources and collaborating.
Phronesis needs content approval and quality-control workflows before resources become publicly or commercially available.
Premium resources need access controls and safeguards against unauthorized downloading, copying and redistribution.
Administrators need visibility and control over users, content, teachers, classes, subscriptions, payments, reports and platform activity.
The organization needs an architecture that supports future mobile applications, increased traffic, additional curriculum areas and international expansion.
4. Target Users and Stakeholders
Actor / Stakeholder
Primary Needs
Key Platform Interaction
Learner
Learn, revise, practice, attend classes and track learning activity.
Register, subscribe, browse content, study, attempt exercises/exams, book/attend virtual classes.
Parent / Guardian
Understand and support learner access and purchases.
Create/manage learner access where applicable, make payments, view subscriptions and selected progress information.
Teacher
Teach professionally and access approved teaching resources.
Apply, verify identity/certification, subscribe/onboard, view assignments/classes, teach, contribute resources, collaborate.
Teacher Reviewer / Academic Reviewer
Maintain educational quality.
Review teacher applications and educational resources, provide approval/rejection feedback.
Content Manager
Organize and maintain learning materials.
Create/edit/classify resources, manage metadata, submit content for approval.
Administrator
Operate and control the platform.
Manage users, teachers, content, subscriptions, classes, payments, roles, reports and system settings.
Phronesis Management
Monitor performance and strategic growth.
Dashboards, analytics, reports, operational oversight and quality indicators.
Customer Support / Staff
Resolve learner, teacher and payment issues.
Support workflows, tickets/queries, account assistance and communication.
System / Security Administrator
Maintain platform security and availability.
Access controls, audit logs, configuration, monitoring and security operations.


5. Target Learner Segments
Segment
Approx. Age
Grades
Platform Need
Junior School
12–15
Grades 7–9
CBC-aligned resources, revision, topical practice, examinations and virtual tuition.
Senior School
15–18
Grades 10–12
Deeper subject resources, revision, examination preparation, virtual tuition and pathway-oriented learning support.


Because the platform may serve minors, learner accounts, communication, content, privacy, parental/guardian involvement, teacher interaction and safety controls should be treated as first-class requirements during detailed analysis and legal review.
6. Proposed Solution Concept
The proposed solution is a multi-role digital learning ecosystem consisting of a learner-facing learning environment, a teacher-facing teaching environment, an administrative/operations environment and a structured content-management and virtual-class infrastructure.
6.1 Core Platform Pillars
Structured CBC Learning Library
Learner Portal / Learner Workspace
Teacher Portal / Teacher Workspace
Teacher Onboarding and Verification
Virtual Classes and Video Lessons
Subscriptions and Payments
Topical Revision and Assessments
Premium Content Protection and Controlled Access
Content Contribution, Review and Approval
Teacher and Learner Collaboration
Administration, Reporting and Quality Assurance
Mobile Education Capability and Future Mobile Applications
7. Academic Content Model
The platform should not treat educational materials as a flat collection of files. Content should be represented through a structured academic hierarchy that allows resources to be discovered, searched, filtered, reused and associated with specific learning areas.
7.1 Proposed Hierarchy
Curriculum → Education Level → Grade/Form → Subject → Strand/Area → Topic/Subtopic → Resource Type → Resource Item
Each resource should support metadata such as title, description, grade, subject, topic, author/creator, academic year where applicable, version, status, access level and review/approval state.
The model should support resources that apply to multiple topics or grades where appropriate without unnecessary duplication.
7.2 Resource Categories
Category
Examples / Purpose
Notes
Updated class notes and topic summaries.
Past Papers
Past examination/revision papers, including materials from 2023 onward where Phronesis has appropriate rights to use them.
Topical Questions
Topic-specific practice questions for reinforcement and mastery.
Exercises
Structured learner exercises and assignments.
Examinations / Assessments
Tests, mock exams and topic/subject assessments.
Answer Guides / Marking Schemes
Controlled-access solutions and marking guidance where permitted.
Videos
Recorded topic-specific explanations and lessons.
Animations
Teaching animations for selected subjects and topics.
Virtual-Class Materials
Slides, worksheets, handouts and supporting resources.
Teacher Resources
Teaching guides, lesson resources and approved classroom materials.
Premium Content
Commercial or restricted resources available only to eligible subscribers/users.


7.3 Subject and Curriculum Coverage
The supplied notes identify core CBC subjects and additional subject/language areas. The final curriculum taxonomy should be confirmed against the exact subjects Phronesis intends to offer at each grade. The notes reference Mathematics, English, Kiswahili, Integrated Science, Social Studies, CRE, Pre-Technical Studies, French, German, Mandarin, Hindu/Hindi, Geography, Biology, Chemistry and Physics, among others. These should not be assumed to apply identically to every grade.
8. Learner Portal – Problem and Capability Definition
The learner portal is intended to become the learner's primary digital workspace. It should reduce the effort required to find appropriate learning materials and participate in Phronesis services.
Account registration and secure authentication.
Learner profile and grade/class configuration.
Subject dashboard.
Structured topic navigation.
Search and filtering across permitted resources.
Access to free and subscribed resources according to entitlements.
Reading/viewing of notes, papers, exercises, videos and animations.
Topical practice and assessments.
Virtual-class discovery, booking and attendance.
Personal learning workspace / dashboard.
History of accessed learning resources and completed activities.
Notifications and class reminders.
Subscription status and payment history.
Support/help functionality.



9. Teacher Arena – Onboarding, Verification and Teaching
Teacher quality is a central trust mechanism for the platform. Teacher onboarding should therefore be designed as a controlled verification workflow rather than an unrestricted self-registration process.
9.1 Proposed Teacher Onboarding Workflow
Teacher creates an application/account.
Teacher submits personal/professional profile information.
Teacher selects subjects, grades and areas of competence.
Teacher submits required certification and supporting documents.
Teacher provides evidence of teaching/content-delivery capability where required.
Platform performs completeness and preliminary validation checks.
Phronesis reviewer conducts academic/professional verification.
Additional verification or interview/demo lesson may be requested.
Application is approved, rejected or returned for correction.
Approved teacher completes onboarding/payment requirements.
Teacher receives access to the Teacher Arena according to assigned permissions.
9.2 Teacher Eligibility Principles
Verified certification appropriate to the teaching role.
Demonstrated subject competence.
Proven or assessed content-delivery ability.
Computer/digital literacy sufficient for online teaching.
Commitment to Phronesis standards and professional conduct.
Acceptance of applicable platform, academic, safeguarding, confidentiality and content policies.
Successful completion of Phronesis verification and onboarding requirements.
9.3 Teacher Workspace
Assigned subjects, grades and classes.
Class/session schedule.
Virtual teaching tools and session links/workspace.
Access to approved teaching resources.
Resource contribution/submission facility.
Draft, submitted, approved and rejected resource states.
Learner/session information appropriate to the teacher's role.
Collaboration and communication tools.
Teacher profile and professional status.
Subscription/onboarding status.
Performance and teaching activity records where required.
10. Virtual Classes and Tuition
Virtual classes are a paid service and should operate as a complete workflow from discovery and booking through payment, confirmation, scheduling, attendance and post-session records.
Stage
Required Capability
Class discovery
Learner can view available subjects, topics, teachers, session descriptions and relevant time information.
Booking
Learner selects an eligible session/service and requests or makes a booking.
Payment / entitlement
System confirms payment or active subscription entitlement before access is granted.
Confirmation
Learner receives confirmation and class details.
Scheduling
Phronesis/teacher publishes or confirms the final schedule after booking according to operating rules.
Attendance
Learner and teacher access the session through the platform.
Session delivery
Approximately one-hour live interactive sessions, subject to Phronesis configuration.
Supporting resources
Teacher may provide approved notes, exercises or other session materials.
Post-session
Attendance, completion, feedback and selected learning records may be retained.


The platform should also support recorded/video lessons for specific topics where Phronesis elects to provide them.
11. Subscription and Commercial Model
11.1 Learners Arena – Initial Stated Plans
Duration
Stated Fee
KES
1 Hour
Ksh. 250
250
1 Day( say 3 sessions ) 
Ksh. 300
300
1 Week (3 sessions a day ) 
Ksh. 2,000
2,000
1 Month
Ksh. 8,000
8,000
1 Year
Ksh. 100,000
100,000


These prices are recorded from the client notes and should be treated as provisional until commercially approved.

11.2 Teachers Arena – Initial Stated Plans
Duration
Stated Fee
KES
1 Week
Ksh. 300 
300
1 Month
Ksh. 500
500
1 Year
Ksh. 1,000
1,000


Teacher onboarding fees, if separate from subscription fees, require explicit confirmation of amount, timing and refund rules.
- Notes - will be deducted from the amount after they start working - onboarding - then a fixed monthly licence fee - say 500/month)  -  Teachers onboarding fee-  then we have monthly charges that are deducted from  the total amount - this means they are paid through the platform - so , as they work their amount is calculated , platform fee, session fee, etc … then we have the total amount we are paying them in a month.

11.3 Entitlement Model
Subscription plans should create explicit entitlements rather than merely marking a user as 'paid'.
The system should know what content, classes, services or duration each plan permits.
Expired, cancelled, refunded or failed payments should affect entitlements according to defined business rules.
The platform should maintain transaction and subscription history for operational and financial reconciliation.
Payment provider integration should be designed so that payment confirmation is reliable and access is not granted solely from client-side claims.
12. Content Protection, Integrity and Copyright
Phronesis intends some content to be premium and accessible only through the platform in order to protect educational value, commercial interests and content integrity. This requirement should be addressed as a layered content-protection strategy rather than a promise that unauthorized copying can be made technically impossible.
Role- and subscription-based authorization.
Secure media/content delivery.
Controlled document viewing where technically feasible.
Reduced exposure of direct storage locations.
Watermarking or user-identifying overlays for selected premium materials where appropriate.
Access logging and audit trails.
Rate limiting and abuse detection.
Restricted administrative access to source files.
Content takedown/review mechanisms.
Clear ownership/licensing records for resources uploaded by Phronesis and teachers.
Explicit rules governing teacher-submitted third-party materials.
Copyright/licensing review before commercial distribution of external past papers or other protected materials.
Important: technical controls can reduce unauthorized redistribution but cannot guarantee that screenshots, photography or other forms of capture will never occur. The final product should combine technical controls, contractual terms, access policies and content licensing.
13. Content Management and Quality Assurance
Educational quality is a core product requirement. Content should therefore move through defined lifecycle states.
State
Meaning
Draft
Content is being created or edited and is not available to learners.
Submitted
Content has been submitted for review.
Under Review
An authorized reviewer is evaluating academic quality, accuracy, structure and compliance.
Approved
Content is authorized for publication according to its access level.
Published
Content is visible to eligible users.
Revision Required
Content requires changes before approval.
Archived
Content is retained for records but no longer actively offered.
Rejected
Content has failed review or does not meet requirements.


Version control should preserve important changes to published resources.
Reviewers should be able to provide structured feedback.
Content should be tagged to academic classifications before publication.
The system should record who created, reviewed, approved, published or modified content.
Phronesis should be able to retire outdated resources and replace them with updated versions.


14. Administration and Organizational Workspace
Dashboard showing users, teachers, content, subscriptions, classes and operational activity.
User and role management.
Teacher application and verification management.
Subject, grade, topic and curriculum taxonomy management.
Content moderation and approval.
Virtual-class management and scheduling.
Subscription plan configuration.
Payment and transaction visibility.
Notifications and announcements.
Reports and analytics.
Audit logs.
Support and issue management.
Platform configuration.
Staff role separation and permission management.
15. Collaboration and Communication
The client notes indicate a need for workspaces and collaboration among learners, teachers, administrators and employees. This should be implemented with role-specific permissions and safeguarding controls.
Teacher-to-platform communication.
Teacher collaboration on educational resources.
Teacher-to-learner communication within controlled contexts.
Announcements and notifications.
Class/session messaging where required.
Administrative communication.
Support requests.
Notification channels such as in-platform notifications, email and, where approved, SMS/other messaging services.
16. Mobile Education Programme
Mobile accessibility is part of the strategic vision. The first release should therefore be responsive and mobile-friendly, while the long-term roadmap may include dedicated Android/iOS applications or other mobile delivery mechanisms.
Low-friction access on mobile devices.
Efficient content loading for constrained networks.
Video quality adaptation where supported.
Mobile-friendly assessments and revision.
Push notifications in a future native application.
Potential offline/limited-connectivity features subject to content-protection requirements.
17. High-Level Functional Requirements
ID
Area
Requirement
FR-01
Authentication & Accounts
Users shall be able to securely register, authenticate, recover accounts and manage profiles according to their roles.
FR-02
Role Management
The platform shall support role-based access for learners, guardians where applicable, teachers, reviewers, staff and administrators.
FR-03
Learner Management
Authorized staff shall be able to manage learner records and status.
FR-04
Teacher Applications
Prospective teachers shall be able to submit onboarding applications and supporting information.
FR-05
Teacher Verification
Authorized reviewers shall be able to review, approve, reject or request changes to teacher applications.
FR-06
Academic Taxonomy
Administrators shall manage grades, subjects, strands/topics and related classifications.
FR-07
Content Management
Authorized users shall create, upload, classify, edit, review, approve, publish and archive resources.
FR-08
Content Access Control
The platform shall enforce access according to user role, subscription, entitlement and resource visibility.
FR-09
Learner Learning
Learners shall be able to consume permitted resources and complete learning activities.
FR-10
Assessments
The platform shall support topical questions, exercises, tests and/or examinations according to configured assessment types.
FR-11
Virtual Classes
The platform shall support class/session creation, booking, confirmation, scheduling and learner/teacher access.
FR-12
Subscriptions
The platform shall support configurable learner and teacher subscription plans.
FR-13
Payments
The platform shall integrate with an approved payment mechanism and record payment status and transaction references.
FR-14
Notifications
The system shall notify users about relevant account, subscription, booking, class and platform events.
FR-15
Search
Users shall be able to search and filter permitted academic resources.
FR-16
Dashboards
Each major role shall have a dashboard appropriate to its responsibilities.
FR-17
Auditability
Important administrative, content, access and financial actions shall be logged.
FR-18
Reporting
Authorized users shall be able to access operational and educational reports.
FR-19
Support
The platform shall provide a mechanism for reporting issues and obtaining support.
FR-20
Scalability
The platform shall support future growth in users, content, teachers, classes and geographic coverage.





18. High-Level Non-Functional Requirements
Area
Requirement / Design Intent
Security
Strong authentication, authorization, secure data handling, least-privilege access and security monitoring.
Privacy
Personal information, learner data, teacher records and operational data should be collected and processed under appropriate privacy controls.
Child Safety
Because the target population includes minors, learner interaction and teacher access must include appropriate safeguarding controls.
Performance
Common learner actions should respond promptly under expected operating loads.
Availability
The service should be designed for dependable access, with backups, monitoring and recovery procedures.
Scalability
Architecture should support growth across Kenya and later international markets.
Usability
The platform should be simple enough for learners and teachers with varying levels of digital literacy.
Accessibility
Interfaces should consider accessible typography, contrast, navigation and assistive technologies.
Maintainability
System components should be modular, documented, testable and maintainable.
Observability
Errors, security events, system health and important business events should be monitored.
Data Integrity
Payments, subscriptions, content states, teacher verification and important records should be consistent and auditable.
Content Integrity
Published educational resources should have controlled versions and review status.
Interoperability
The platform should expose well-defined integration points for payments, video/meeting services, notifications and future applications.


19. Key Business Rules to Define
A learner may access only resources included in the learner's active entitlement.
A teacher should not teach on the platform until required verification and onboarding steps are complete.
Teacher permissions should depend on verification status and assigned role.
Only authorized reviewers may approve educational resources for publication.
Premium resources should not become publicly visible merely because their URLs are known.
A virtual-class booking should have a defined lifecycle: requested/booked → payment/entitlement confirmed → confirmed/scheduled → attended/completed/cancelled.
Subscription expiry must automatically affect relevant entitlements.
Refund, cancellation and failed-payment behavior must be explicitly defined.
Teacher-submitted content should remain under review until approved.
Administrative actions affecting users, payments, content or permissions should be auditable.
The platform should distinguish between user account status, teacher verification status and subscription status.
The system should support suspension/revocation where a teacher, learner account or content item violates defined rules.
20. Proposed End-to-End User Journeys
20.1 Learner Journey
Discover Phronesis Homeschool.
Create an account / obtain learner access.
Select or confirm grade and academic level.
Browse subjects and topics.
Access free resources or select a paid plan.
Complete payment where required.
Receive the relevant entitlement.
Study notes, papers, videos, animations and topical resources.
Attempt exercises/assessments.
Discover and book a virtual class.
Receive class confirmation and schedule.
Attend the live session.
Continue learning through the learner workspace.
20.2 Teacher Journey
Create a teacher application.
Submit profile, subject areas and certification.
Complete verification requirements.
Receive reviewer decision.
Complete onboarding/payment requirements.
Access Teacher Arena.
View assigned classes/resources.
Teach virtual sessions.
Submit or contribute educational resources.
Collaborate with other approved teachers where permitted.
Receive feedback and maintain professional profile/status.
20.3 Content Journey
Content is created or uploaded.
Metadata and curriculum classification are completed.
Content enters review.
Academic/quality reviewer evaluates it.
Content is approved, rejected or returned for revision.
Approved content is published under a defined access level.
Learner/teacher access is governed by entitlements and roles.
Content is periodically reviewed and updated or archived.
21. Scope Definition
21.1 Proposed Core MVP Scope
Secure authentication and role-based accounts.
Learner portal.
Teacher onboarding and verification workflow.
Teacher portal.
Admin portal.
CBC academic structure for agreed grades and subjects.
Core learning resource library.
Notes, topical questions, past papers and selected multimedia resources.
Learner subscriptions.
Teacher subscriptions/onboarding fees where approved.
Payment integration.
Virtual class booking and session management.
Basic notifications.
Content approval workflow.
Basic reporting and audit logs.
Responsive/mobile-friendly web experience.
21.2 Recommended Later-Phase Capabilities
Dedicated mobile applications.
Advanced learner analytics and personalized learning paths.
AI-assisted learning/tutoring features.
Advanced adaptive assessments.
Teacher performance analytics.
Parent/guardian dashboards with configurable visibility.
Expanded collaboration/community features.
Offline learning packages subject to licensing and protection controls.
International curriculum support.
Multi-country payment and localization.
Advanced video classroom infrastructure.
Digital certificates and professional development features for teachers.
22. Risks and Challenges
Risk
Impact
Mitigation Direction
Unverified teachers
Learner safety and education quality risk.
Strong onboarding, document verification, reviewer workflow, conduct standards and access controls.
Copyright infringement
Legal and commercial exposure.
Confirm ownership/licensing before publication; maintain rights metadata and takedown procedures.
Unauthorized content redistribution
Loss of premium content value.
Access control, secure delivery, watermarking where appropriate, monitoring and contractual controls.
Poor connectivity
Reduced access for remote learners.
Responsive design, optimized assets, adaptive media and future offline strategies.
Payment failures
Incorrect access or revenue leakage.
Server-side payment verification, transaction reconciliation and clear entitlement rules.
Low-quality uploaded resources
Reduced educational trust.
Review and approval workflow, versioning and academic reviewers.
Scale and performance
Poor learner experience as adoption grows.
Scalable architecture, caching, monitoring, load testing and capacity planning.
Minor safeguarding concerns
Potential learner safety risk.
Role boundaries, controlled communication, reporting mechanisms, safeguarding policies and appropriate verification.
Complexity creep
Budget and timeline overrun.
Phase the product, define MVP, prioritize core workflows and manage change requests.
Ambiguous curriculum taxonomy
Poor content discovery and classification.
Finalize grade/subject/topic taxonomy before bulk content migration.


23. Key Assumptions
The initial platform market is Kenya.
The first curriculum focus is CBC for Grades 7–12.
Phronesis will supply or obtain lawful rights to distribute the educational materials it publishes.
Phronesis will identify personnel responsible for academic review and teacher verification.
The subscription prices in this document are preliminary business inputs, not final contractual prices.
Virtual classes may require a third-party video infrastructure or an integrated video service unless Phronesis elects to build its own classroom technology.
Payment provider, messaging provider and video provider choices will be confirmed during technical discovery.
The platform will initially be delivered as a responsive web application, with native mobile applications considered as a later phase unless otherwise agreed.
Detailed legal, privacy, child-safeguarding and content-licensing policies will be reviewed and approved by qualified professionals.
24. Critical Questions Requiring Client Validation
What exact grades and subjects will be available at launch?
What is the final approved CBC subject/strand/topic taxonomy for each grade?
Will Phronesis serve only learners, or will parents/guardians have separate accounts?
How will learners under 18 be registered and how will guardian consent or oversight operate?
Are the stated learner subscription plans access plans, tuition plans, content plans, or combinations of these?
What exactly does a 1-hour learner subscription purchase provide?
Are virtual classes one-to-one, group-based, or both?
Can learners select teachers, or does Phronesis assign teachers?
How many learners may join a virtual session?
How are class schedules created and how far in advance are they published?
What happens when a class is cancelled, a teacher is unavailable or a learner misses a session?
Will sessions be recorded? If yes, who can access recordings and for how long?
Which payment methods will be supported initially?
What are the refund, cancellation and subscription renewal rules?
What is the exact teacher onboarding fee, if any, and is it refundable?
What documents are mandatory for teacher verification?
Who performs teacher verification and how many approval stages are required?
Will Phronesis conduct interviews, demo lessons, reference checks or other assessments?
What teacher qualifications are acceptable for each subject?
Can teachers upload content? If yes, who owns the intellectual property after upload?
What content may teachers upload, and what third-party materials are prohibited?
Which past papers from 2023 onward does Phronesis have rights to distribute?
Will premium content be view-only, downloadable, printable or a combination?
Which resources are free versus premium?
Will parents see learner progress, grades, attendance and subscriptions?
Will learners be allowed to communicate directly with teachers outside live classes?
What safeguarding and reporting process should apply to inappropriate interactions?
What analytics does management need?
What reports are required for teachers, finance, academic reviewers and management?
What languages should the platform interface support?
What are the expected launch user numbers and five-year growth assumptions?
Which features are mandatory for MVP versus future phases?
What is the preferred launch date and operating budget?
25. Success Measures
The success of the platform should be measured not only by technical completion but by whether it solves the educational and operational problems identified above.
Dimension
Example KPI / Outcome
Learner acquisition
Number of registered and active learners.
Resource usage
Resource views, completed activities and repeat learning sessions.
Learning engagement
Assessment attempts, completion rates and virtual-class attendance.
Teacher quality
Percentage of teachers successfully verified and ongoing quality indicators.
Teacher participation
Number of active teachers, sessions delivered and approved resources contributed.
Commercial performance
Subscriptions, paid classes, conversion and recurring revenue.
Service quality
Class attendance, cancellation rate, support resolution and learner feedback.
Content quality
Approval rate, revision rate, outdated-content replacement and academic review completion.
Platform reliability
Availability, error rates and successful payment/access transactions.
Geographic reach
Learner distribution across counties and later countries.
Trust & safety
Verified teacher rate, reported incidents, resolution times and access-policy compliance.


26. Recommended System Architecture Domains
This is a business/domain decomposition rather than a final technical architecture. It provides a clean foundation for subsequent system design.
Identity & Access Management
Learner Management
Guardian Management (if approved)
Teacher Management
Teacher Verification
Curriculum & Academic Taxonomy
Content Management
Content Review & Publishing
Learning & Assessments
Virtual Classes
Subscriptions & Entitlements
Payments & Financial Records
Notifications & Communications
Search & Discovery
Collaboration
Reporting & Analytics
Audit & Compliance
Administration & Configuration
Support / Helpdesk
Media / Video Management
27. Data Entities – Initial Conceptual Model
Entity
Purpose
User
Base identity and account information.
Learner Profile
Grade, subjects and learner-specific information.
Guardian Profile
Parent/guardian relationship and oversight where applicable.
Teacher Profile
Professional profile, subjects, grades and verification state.
Teacher Application
Onboarding submission and review information.
Certification
Teacher qualification/certification record.
Grade
Academic grade/level.
Subject
Academic subject.
Topic / Strand
Curriculum structure.
Resource
Educational content item.
Resource Version
Version history for controlled content.
Review
Academic/content review decision and feedback.
Subscription Plan
Definition of a commercial plan.
Subscription
User's active/historical plan enrollment.
Entitlement
Specific access rights granted by plan/payment.
Payment Transaction
Financial transaction record.
Virtual Class
Scheduled teaching session.
Booking
Learner's reservation for a class.
Attendance
Participation record.
Assessment
Test/exam/exercise definition.
Assessment Attempt
Learner attempt and outcome.
Notification
System communication.
Audit Log
Trace of important platform actions.


28. Security, Privacy and Safeguarding Considerations
These areas require detailed requirements and professional review before production launch. They are especially important because the service is intended to include minors, teachers, professional credentials, educational records and financial transactions.
Role-based access control and least privilege.
Secure authentication and session management.
Protection of teacher certification and identity documents.
Protection of learner information and learning records.
Controlled teacher-to-learner interactions.
Reporting and escalation mechanisms for inappropriate conduct.
Audit logging for sensitive administrative actions.
Secure payment handling and minimization of stored payment information.
Backup and disaster-recovery procedures.
Data retention and deletion rules.
Privacy notices and appropriate consent mechanisms.
Content ownership/licensing records.
Administrative separation of duties.
The final implementation should be reviewed against applicable Kenyan requirements and any other jurisdictions entered during international expansion. This document intentionally defines system needs rather than serving as legal advice.
29. Recommended Delivery Strategy
Because the requested vision is broad, a phased delivery approach is recommended. The platform should first prove the core education and revenue workflows before introducing advanced features.
Phase
Primary Focus
Phase 1 – Foundation
Accounts, roles, learner/teacher/admin portals, curriculum structure and core content management.
Phase 2 – Learning
Resource library, topical revision, assessments, multimedia and learner workspace.
Phase 3 – Commercial
Subscriptions, payments, entitlements and commercial reporting.
Phase 4 – Virtual Tuition
Teacher availability, bookings, scheduling, live sessions and class records.
Phase 5 – Quality & Scale
Advanced verification, content QA, analytics, performance and operational automation.
Phase 6 – Mobile & Expansion
Native mobile experience, internationalization and additional curricula/markets.


30. Overall Problem-Solution Alignment
Identified Problem
System Response
Scattered learning resources
Centralized, structured academic resource library.
Difficulty finding trustworthy home-schooling support
Verified teacher network and virtual tuition.
Unstructured revision
Grade → subject → topic structure, topical questions and assessments.
Limited access to professional teachers
Teacher onboarding, verification and scheduling platform.
Manual/fragmented class booking
Integrated booking, payment, confirmation and attendance workflow.
Uncontrolled educational content
Content contribution, academic review and approval workflow.
Premium content redistribution
Entitlement-based access and layered content protection.
Weak learner visibility
Learner workspace, activity records and future progress analytics.
Teacher collaboration challenges
Teacher workspace and controlled collaboration tools.
Administrative complexity
Centralized administration, dashboards, reporting and audit trails.
Limited geographic reach
Responsive, scalable, mobile-friendly digital platform.
Difficulty scaling globally
Extensible academic, commercial, localization and architecture models.


31. Conclusion
Phronesis Homeschool's proposed system should be understood as a digital education ecosystem designed to make quality home-schooling resources and professional teaching accessible, structured and scalable. The platform's differentiator is the integration of curriculum-aligned content, verified teachers, virtual tuition, controlled premium resources, learner workspaces and institutional quality assurance in one environment.
The immediate priority is to validate the business rules and unanswered questions identified in this document, particularly curriculum coverage, learner/guardian account relationships, teacher verification, virtual-class models, subscription entitlements, payment/refund rules, content licensing, premium-content behavior and safeguarding. Once those decisions are confirmed, this problem definition can be transformed into a formal Software Requirements Specification (SRS), user stories, process flows, database model, UX/UI requirements, technical architecture and phased implementation plan.
32. Document Approval / Validation
This document is intended to be reviewed jointly by Phronesis Homeschool and the system development team. It should be treated as the baseline for requirements discovery only after the client validates the assumptions, scope and open questions.
Role
Name
Signature
Date
Phronesis Homeschool Representative






Phronesis Academic / Operations Representative






Client Management Representative






System/Product Representative








