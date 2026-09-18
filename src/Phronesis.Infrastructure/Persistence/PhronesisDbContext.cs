using Microsoft.EntityFrameworkCore;
using Phronesis.Application.Common.Interfaces;
using Phronesis.Domain.Common;
using Phronesis.Domain.Identity;
using System.Reflection;

namespace Phronesis.Infrastructure.Persistence;

public class PhronesisDbContext : DbContext, IApplicationDbContext
{
    public PhronesisDbContext(DbContextOptions<PhronesisDbContext> options) : base(options)
    {
    }

    public DbSet<User> Users => Set<User>();
    public DbSet<Role> Roles => Set<Role>();
    public DbSet<Permission> Permissions => Set<Permission>();
    public DbSet<UserSession> UserSessions => Set<UserSession>();
    public DbSet<UserRole> UserRoles => Set<UserRole>();
    public DbSet<RolePermission> RolePermissions => Set<RolePermission>();
    public DbSet<Phronesis.Domain.Organization.StaffProfile> StaffProfiles => Set<Phronesis.Domain.Organization.StaffProfile>();
    public DbSet<Phronesis.Domain.Academic.GradeLevel> GradeLevels => Set<Phronesis.Domain.Academic.GradeLevel>();
    public DbSet<Phronesis.Domain.Users.LearnerProfile> LearnerProfiles => Set<Phronesis.Domain.Users.LearnerProfile>();
    public DbSet<Phronesis.Domain.Users.GuardianProfile> GuardianProfiles => Set<Phronesis.Domain.Users.GuardianProfile>();
    public DbSet<Phronesis.Domain.Users.LearnerGuardian> LearnerGuardians => Set<Phronesis.Domain.Users.LearnerGuardian>();
    public DbSet<Phronesis.Domain.Users.TeacherProfile> TeacherProfiles => Set<Phronesis.Domain.Users.TeacherProfile>();
    public DbSet<Phronesis.Domain.Users.TeacherApplication> TeacherApplications => Set<Phronesis.Domain.Users.TeacherApplication>();
    public DbSet<Phronesis.Domain.Users.TeacherDocument> TeacherDocuments => Set<Phronesis.Domain.Users.TeacherDocument>();

    public DbSet<Phronesis.Domain.Academic.Curriculum> Curricula => Set<Phronesis.Domain.Academic.Curriculum>();
    public DbSet<Phronesis.Domain.Academic.EducationLevel> EducationLevels => Set<Phronesis.Domain.Academic.EducationLevel>();
    public DbSet<Phronesis.Domain.Academic.Subject> Subjects => Set<Phronesis.Domain.Academic.Subject>();
    public DbSet<Phronesis.Domain.Academic.Strand> Strands => Set<Phronesis.Domain.Academic.Strand>();
    public DbSet<Phronesis.Domain.Academic.SubStrand> SubStrands => Set<Phronesis.Domain.Academic.SubStrand>();
    public DbSet<Phronesis.Domain.Academic.LearningObjective> LearningObjectives => Set<Phronesis.Domain.Academic.LearningObjective>();

    public DbSet<Phronesis.Domain.Academic.GradeSubject> GradeSubjects => Set<Phronesis.Domain.Academic.GradeSubject>();
    public DbSet<Phronesis.Domain.Academic.SubStrandPrerequisite> SubStrandPrerequisites => Set<Phronesis.Domain.Academic.SubStrandPrerequisite>();
    public DbSet<Phronesis.Domain.Users.TeacherCompetence> TeacherCompetences => Set<Phronesis.Domain.Users.TeacherCompetence>();

    public DbSet<Phronesis.Domain.Content.EducationalContent> EducationalContents => Set<Phronesis.Domain.Content.EducationalContent>();
    public DbSet<Phronesis.Domain.Content.ContentTag> ContentTags => Set<Phronesis.Domain.Content.ContentTag>();
    public DbSet<Phronesis.Domain.Content.ContentReview> ContentReviews => Set<Phronesis.Domain.Content.ContentReview>();
    public DbSet<Phronesis.Domain.Content.ContentAttachment> ContentAttachments => Set<Phronesis.Domain.Content.ContentAttachment>();
    public DbSet<Phronesis.Domain.Content.SavedContent> SavedContents => Set<Phronesis.Domain.Content.SavedContent>();

    public DbSet<Phronesis.Domain.Learning.LearnerEnrollment> LearnerEnrollments => Set<Phronesis.Domain.Learning.LearnerEnrollment>();
    public DbSet<Phronesis.Domain.Learning.ContentEngagement> ContentEngagements => Set<Phronesis.Domain.Learning.ContentEngagement>();

    public DbSet<Phronesis.Domain.Learning.Assessment> Assessments => Set<Phronesis.Domain.Learning.Assessment>();
    public DbSet<Phronesis.Domain.Learning.Question> Questions => Set<Phronesis.Domain.Learning.Question>();
    public DbSet<Phronesis.Domain.Learning.QuestionOption> QuestionOptions => Set<Phronesis.Domain.Learning.QuestionOption>();
    public DbSet<Phronesis.Domain.Learning.AssessmentAttempt> AssessmentAttempts => Set<Phronesis.Domain.Learning.AssessmentAttempt>();
    public DbSet<Phronesis.Domain.Learning.AttemptAnswer> AttemptAnswers => Set<Phronesis.Domain.Learning.AttemptAnswer>();

    public DbSet<Phronesis.Domain.Learning.SubjectProgress> SubjectProgresses => Set<Phronesis.Domain.Learning.SubjectProgress>();
    public DbSet<Phronesis.Domain.Learning.Certificate> Certificates => Set<Phronesis.Domain.Learning.Certificate>();

    public DbSet<Phronesis.Domain.Commerce.SubscriptionPlan> SubscriptionPlans => Set<Phronesis.Domain.Commerce.SubscriptionPlan>();
    public DbSet<Phronesis.Domain.Commerce.UserSubscription> UserSubscriptions => Set<Phronesis.Domain.Commerce.UserSubscription>();
    public DbSet<Phronesis.Domain.Commerce.PaymentTransaction> PaymentTransactions => Set<Phronesis.Domain.Commerce.PaymentTransaction>();
    public DbSet<Phronesis.Domain.Commerce.Order> Orders => Set<Phronesis.Domain.Commerce.Order>();
    public DbSet<Phronesis.Domain.Commerce.Invoice> Invoices => Set<Phronesis.Domain.Commerce.Invoice>();
    public DbSet<Phronesis.Domain.Commerce.RefundRequest> RefundRequests => Set<Phronesis.Domain.Commerce.RefundRequest>();

    public DbSet<Phronesis.Domain.Tuition.VirtualClass> VirtualClasses => Set<Phronesis.Domain.Tuition.VirtualClass>();
    public DbSet<Phronesis.Domain.Tuition.ClassEnrollment> ClassEnrollments => Set<Phronesis.Domain.Tuition.ClassEnrollment>();
    public DbSet<Phronesis.Domain.Tuition.ClassSession> ClassSessions => Set<Phronesis.Domain.Tuition.ClassSession>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
        base.OnModelCreating(modelBuilder);
    }

    public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        foreach (var entry in ChangeTracker.Entries<BaseEntity>())
        {
            switch (entry.State)
            {
                case EntityState.Added:
                    entry.Entity.CreatedAt = DateTime.UtcNow;
                    break;
                case EntityState.Modified:
                    entry.Entity.UpdatedAt = DateTime.UtcNow;
                    break;
            }
        }
        return base.SaveChangesAsync(cancellationToken);
    }
}
