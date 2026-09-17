# Walkthrough: M11 (Educational Content Management)

We have successfully engineered **M11**, bringing the Content phase to life by allowing educators and staff to author learning materials deeply integrated with our academic taxonomy!

## 1. Content Aggregate Root
We created the core `EducationalContent` entity. This acts as the single source of truth for a piece of learning material:
- **Metadata:** It stores `Title`, `Description`, `Version`, and an `IsPremium` flag (setting up our Phase 6 monetization pipeline early).
- **State:** It uses strict Enums for `ContentType` (`Video`, `Document`, `Interactive`) and `ContentStatus` (`Draft`, `InReview`, `Published`). All content starts as `Draft`.
- **Flexible Taxonomy Tagging:** We enforced that content *must* be mapped to a `GradeLevel` and `Subject`. However, we left the granular CBC tags (`Strand`, `SubStrand`, `LearningObjective`) optional. This provides the pedagogical flexibility to upload general textbooks alongside highly specific micro-lessons.
- **Search Optimization:** We built a dedicated `ContentTag` entity to allow arbitrary string tagging (e.g., "KCPE Revision") to support future search discovery.

## 2. Bulletproof Database Configuration
- We mapped the extensive foreign keys bridging the `Content` domain to the `Academic` domain.
- **Critical Safeguard:** We applied `DeleteBehavior.SetNull` to the optional taxonomy tags (Strand, SubStrand, etc.). If a curriculum administrator deletes or restructures a SubStrand in M09, the platform will *not* cascade-delete the actual educational content. The content simply loses that tag and remains safe, ensuring years of authored material are never accidentally destroyed.

## 3. Authoring API (`ContentController`)
The API is live and secured:
- **Creation & Updating:** Authors can initialize a Draft and update its metadata/tags dynamically. The system securely extracts the author's identity directly from their JWT token (`sub` claim).
- **Security Check:** The `PUT` endpoint strictly verifies that only the original author can edit their draft content, and ensures that content cannot be manually modified once it leaves the `Draft` state (handing control over to M12's review process).

**Phase 4 is off to a phenomenal start.** The content engine is running. Next, in **M12 (Content Review & Publishing)**, we will build the robust multi-stage approval pipelines required for quality assurance!
