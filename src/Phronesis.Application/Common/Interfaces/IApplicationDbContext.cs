using Microsoft.EntityFrameworkCore;
using Phronesis.Domain.Identity;

namespace Phronesis.Application.Common.Interfaces;

public interface IApplicationDbContext
{
    DbSet<User> Users { get; }
    DbSet<Role> Roles { get; }
    DbSet<Permission> Permissions { get; }
    DbSet<UserSession> UserSessions { get; }
    DbSet<UserRole> UserRoles { get; }
    DbSet<RolePermission> RolePermissions { get; }
    DbSet<Phronesis.Domain.Organization.StaffProfile> StaffProfiles { get; }
    DbSet<Phronesis.Domain.Academic.GradeLevel> GradeLevels { get; }
    DbSet<Phronesis.Domain.Users.LearnerProfile> LearnerProfiles { get; }
    DbSet<Phronesis.Domain.Users.GuardianProfile> GuardianProfiles { get; }
    DbSet<Phronesis.Domain.Users.LearnerGuardian> LearnerGuardians { get; }
    DbSet<Phronesis.Domain.Users.TeacherProfile> TeacherProfiles { get; }
    DbSet<Phronesis.Domain.Users.TeacherApplication> TeacherApplications { get; }
    DbSet<Phronesis.Domain.Users.TeacherDocument> TeacherDocuments { get; }

    DbSet<Phronesis.Domain.Academic.Curriculum> Curricula { get; }
    DbSet<Phronesis.Domain.Academic.EducationLevel> EducationLevels { get; }
    DbSet<Phronesis.Domain.Academic.Subject> Subjects { get; }
    DbSet<Phronesis.Domain.Academic.Strand> Strands { get; }
    DbSet<Phronesis.Domain.Academic.SubStrand> SubStrands { get; }
    DbSet<Phronesis.Domain.Academic.LearningObjective> LearningObjectives { get; }

    DbSet<Phronesis.Domain.Academic.GradeSubject> GradeSubjects { get; }
    DbSet<Phronesis.Domain.Academic.SubStrandPrerequisite> SubStrandPrerequisites { get; }
    DbSet<Phronesis.Domain.Users.TeacherCompetence> TeacherCompetences { get; }

    DbSet<Phronesis.Domain.Content.EducationalContent> EducationalContents { get; }
    DbSet<Phronesis.Domain.Content.ContentTag> ContentTags { get; }
    DbSet<Phronesis.Domain.Content.ContentReview> ContentReviews { get; }
    DbSet<Phronesis.Domain.Content.ContentAttachment> ContentAttachments { get; }
    DbSet<Phronesis.Domain.Content.SavedContent> SavedContents { get; }

    DbSet<Phronesis.Domain.Learning.LearnerEnrollment> LearnerEnrollments { get; }
    DbSet<Phronesis.Domain.Learning.ContentEngagement> ContentEngagements { get; }

    DbSet<Phronesis.Domain.Learning.Assessment> Assessments { get; }
    DbSet<Phronesis.Domain.Learning.Question> Questions { get; }
    DbSet<Phronesis.Domain.Learning.QuestionOption> QuestionOptions { get; }
    DbSet<Phronesis.Domain.Learning.AssessmentAttempt> AssessmentAttempts { get; }
    DbSet<Phronesis.Domain.Learning.AttemptAnswer> AttemptAnswers { get; }

    DbSet<Phronesis.Domain.Learning.SubjectProgress> SubjectProgresses { get; }
    DbSet<Phronesis.Domain.Learning.Certificate> Certificates { get; }

    DbSet<Phronesis.Domain.Commerce.SubscriptionPlan> SubscriptionPlans { get; }
    DbSet<Phronesis.Domain.Commerce.UserSubscription> UserSubscriptions { get; }
    DbSet<Phronesis.Domain.Commerce.PaymentTransaction> PaymentTransactions { get; }
    DbSet<Phronesis.Domain.Commerce.Order> Orders { get; }
    DbSet<Phronesis.Domain.Commerce.Invoice> Invoices { get; }
    DbSet<Phronesis.Domain.Commerce.RefundRequest> RefundRequests { get; }

    DbSet<Phronesis.Domain.Tuition.VirtualClass> VirtualClasses { get; }
    DbSet<Phronesis.Domain.Tuition.ClassEnrollment> ClassEnrollments { get; }
    DbSet<Phronesis.Domain.Tuition.ClassSession> ClassSessions { get; }

    Task<int> SaveChangesAsync(CancellationToken cancellationToken);
}
