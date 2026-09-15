using Microsoft.EntityFrameworkCore;
using Phronesis.Application.Common.Interfaces;

namespace Phronesis.Application.Authorization;

public class PermissionService : IPermissionService
{
    private readonly IApplicationDbContext _context;

    public PermissionService(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<HashSet<string>> GetPermissionsAsync(Guid userId)
    {
        var permissions = await _context.UserRoles
            .AsNoTracking()
            .Where(ur => ur.UserId == userId)
            .Join(_context.RolePermissions,
                ur => ur.RoleId,
                rp => rp.RoleId,
                (ur, rp) => rp.Permission)
            .Select(p => p.Name)
            .ToListAsync();

        return permissions.ToHashSet();
    }
}
