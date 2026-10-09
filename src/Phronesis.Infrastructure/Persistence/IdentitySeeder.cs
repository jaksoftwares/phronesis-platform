using Microsoft.EntityFrameworkCore;
using Phronesis.Application.Authentication;
using Phronesis.Application.Common.Interfaces;
using Phronesis.Domain.Identity;

namespace Phronesis.Infrastructure.Persistence;

public static class IdentitySeeder
{
    public static async Task SeedAsync(IApplicationDbContext context, IPasswordHasher passwordHasher)
    {
        // Check if Admin role exists
        var adminRole = await context.Roles.FirstOrDefaultAsync(r => r.Name == "Admin");
        if (adminRole == null)
        {
            adminRole = new Role("Admin", "System Administrator with full access.");
            context.Roles.Add(adminRole);
            await context.SaveChangesAsync(CancellationToken.None);
        }

        // Check if Admin user exists
        var adminUser = await context.Users.FirstOrDefaultAsync(u => u.Email == "admin@phronesis.com");
        if (adminUser == null)
        {
            var hash = passwordHasher.Hash("Admin@123!");
            adminUser = new User("admin@phronesis.com", hash, "System", "Administrator");
            adminUser.ConfirmEmail(); // Admin is pre-confirmed
            
            context.Users.Add(adminUser);
            await context.SaveChangesAsync(CancellationToken.None);

            // Assign Admin role
            var userRole = new UserRole(adminUser.Id, adminRole.Id);
            context.UserRoles.Add(userRole);
            await context.SaveChangesAsync(CancellationToken.None);
        }
    }
}
