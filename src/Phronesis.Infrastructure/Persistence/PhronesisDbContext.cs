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
