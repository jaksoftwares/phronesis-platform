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

    Task<int> SaveChangesAsync(CancellationToken cancellationToken);
}
