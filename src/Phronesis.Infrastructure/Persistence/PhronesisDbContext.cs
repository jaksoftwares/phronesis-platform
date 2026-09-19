using Microsoft.EntityFrameworkCore;
using Phronesis.Application.Common.Interfaces;
using Phronesis.Domain.Common;
using Phronesis.Domain.Identity;
using System.Reflection;

namespace Phronesis.Infrastructure.Persistence;

public class PhronesisDbContext : DbContext, IApplicationDbContext
{
    private readonly ICurrentUserService? _currentUserService;

    public PhronesisDbContext(
        DbContextOptions<PhronesisDbContext> options, 
        ICurrentUserService? currentUserService = null) : base(options)
    {
        _currentUserService = currentUserService;
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
    public DbSet<Phronesis.Domain.Users.LearnerGuardianLinkRequest> LearnerGuardianLinkRequests => Set<Phronesis.Domain.Users.LearnerGuardianLinkRequest>();
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
    public DbSet<Phronesis.Domain.Tuition.ClassAttendance> ClassAttendances => Set<Phronesis.Domain.Tuition.ClassAttendance>();
    public DbSet<Phronesis.Domain.Tuition.SessionFeedback> SessionFeedbacks => Set<Phronesis.Domain.Tuition.SessionFeedback>();
    public DbSet<Phronesis.Domain.Tuition.TeacherAvailability> TeacherAvailabilities => Set<Phronesis.Domain.Tuition.TeacherAvailability>();
    public DbSet<Phronesis.Domain.Tuition.BookingRequest> BookingRequests => Set<Phronesis.Domain.Tuition.BookingRequest>();

    public DbSet<Phronesis.Domain.Collaboration.ClassResource> ClassResources => Set<Phronesis.Domain.Collaboration.ClassResource>();
    public DbSet<Phronesis.Domain.Collaboration.ClassDiscussion> ClassDiscussions => Set<Phronesis.Domain.Collaboration.ClassDiscussion>();
    public DbSet<Phronesis.Domain.Collaboration.DiscussionReply> DiscussionReplies => Set<Phronesis.Domain.Collaboration.DiscussionReply>();

    public DbSet<Phronesis.Domain.Communication.Notification> Notifications => Set<Phronesis.Domain.Communication.Notification>();
    public DbSet<Phronesis.Domain.Communication.NotificationPreference> NotificationPreferences => Set<Phronesis.Domain.Communication.NotificationPreference>();

    public DbSet<Phronesis.Domain.Support.SupportTicket> SupportTickets => Set<Phronesis.Domain.Support.SupportTicket>();
    public DbSet<Phronesis.Domain.Support.TicketMessage> TicketMessages => Set<Phronesis.Domain.Support.TicketMessage>();

    public DbSet<Phronesis.Domain.Operations.SystemSetting> SystemSettings => Set<Phronesis.Domain.Operations.SystemSetting>();
    public DbSet<Phronesis.Domain.Operations.FeatureFlag> FeatureFlags => Set<Phronesis.Domain.Operations.FeatureFlag>();

    public DbSet<Phronesis.Domain.Operations.Audit.AuditLog> AuditLogs => Set<Phronesis.Domain.Operations.Audit.AuditLog>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
        
        modelBuilder.Entity<Phronesis.Domain.Users.LearnerGuardianLinkRequest>()
            .HasOne(r => r.LearnerProfile)
            .WithMany()
            .HasForeignKey(r => r.LearnerProfileId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Phronesis.Domain.Users.LearnerGuardianLinkRequest>()
            .HasOne(r => r.GuardianProfile)
            .WithMany()
            .HasForeignKey(r => r.GuardianProfileId)
            .OnDelete(DeleteBehavior.Cascade);

        base.OnModelCreating(modelBuilder);
    }

    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        var userId = _currentUserService?.UserId;
        var ipAddress = _currentUserService?.IpAddress;

        var auditEntries = new List<Phronesis.Domain.Operations.Audit.AuditLog>();

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

        foreach (var entry in ChangeTracker.Entries())
        {
            if (entry.Entity is Phronesis.Domain.Operations.Audit.AuditLog || entry.State == EntityState.Detached || entry.State == EntityState.Unchanged)
                continue;

            var entityName = entry.Entity.GetType().Name;
            var action = entry.State.ToString();
            
            // Try to get EntityId (Id property usually exists on BaseEntity or directly)
            var idProperty = entry.Properties.FirstOrDefault(p => p.Metadata.IsPrimaryKey());
            var entityId = idProperty?.CurrentValue?.ToString() ?? "Unknown";

            string? oldValues = null;
            string? newValues = null;

            if (entry.State == EntityState.Added)
            {
                newValues = System.Text.Json.JsonSerializer.Serialize(entry.CurrentValues.ToObject());
            }
            else if (entry.State == EntityState.Deleted)
            {
                oldValues = System.Text.Json.JsonSerializer.Serialize(entry.OriginalValues.ToObject());
            }
            else if (entry.State == EntityState.Modified)
            {
                oldValues = System.Text.Json.JsonSerializer.Serialize(entry.OriginalValues.ToObject());
                newValues = System.Text.Json.JsonSerializer.Serialize(entry.CurrentValues.ToObject());
            }

            auditEntries.Add(new Phronesis.Domain.Operations.Audit.AuditLog(
                userId: userId,
                action: action,
                entityName: entityName,
                entityId: entityId,
                oldValues: oldValues,
                newValues: newValues,
                ipAddress: ipAddress
            ));
        }

        if (auditEntries.Any())
        {
            await AuditLogs.AddRangeAsync(auditEntries, cancellationToken);
        }

        return await base.SaveChangesAsync(cancellationToken);
    }
}
