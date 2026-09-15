using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using Phronesis.Application.Common.Interfaces;
using System.Security.Claims;
using System.IdentityModel.Tokens.Jwt;

namespace Phronesis.Infrastructure.Authentication;

public class PermissionAuthorizationHandler : AuthorizationHandler<PermissionRequirement>
{
    private readonly IApplicationDbContext _context;

    public PermissionAuthorizationHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    protected override async Task HandleRequirementAsync(AuthorizationHandlerContext context, PermissionRequirement requirement)
    {
        var userIdString = context.User.Claims.FirstOrDefault(c => c.Type == JwtRegisteredClaimNames.Sub)?.Value 
                        ?? context.User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;

        if (!Guid.TryParse(userIdString, out Guid userId))
        {
            return;
        }

        var hasPermission = await _context.UserRoles
            .AsNoTracking()
            .Where(ur => ur.UserId == userId)
            .Join(_context.RolePermissions, 
                  ur => ur.RoleId, 
                  rp => rp.RoleId, 
                  (ur, rp) => rp.Permission)
            .AnyAsync(p => p.Name == requirement.Permission);

        if (hasPermission)
        {
            context.Succeed(requirement);
        }
    }
}
