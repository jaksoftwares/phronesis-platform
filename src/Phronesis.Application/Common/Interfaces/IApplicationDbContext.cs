using Microsoft.EntityFrameworkCore;
using Phronesis.Domain.Identity;

namespace Phronesis.Application.Common.Interfaces;

public interface IApplicationDbContext
{
    DbSet<User> Users { get; }
    DbSet<Role> Roles { get; }
    DbSet<Permission> Permissions { get; }
    DbSet<UserSession> UserSessions { get; }

    Task<int> SaveChangesAsync(CancellationToken cancellationToken);
}
