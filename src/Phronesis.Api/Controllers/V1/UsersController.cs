using Microsoft.AspNetCore.Mvc;
using Phronesis.Application.Common.Interfaces;
using Phronesis.Domain.Identity;
using Phronesis.Infrastructure.Authentication;
using Microsoft.EntityFrameworkCore;

namespace Phronesis.Api.Controllers.V1;

[ApiController]
[Route("api/v1/[controller]")]
public class UsersController : ControllerBase
{
    private readonly IApplicationDbContext _context;

    public UsersController(IApplicationDbContext context)
    {
        _context = context;
    }

    [HttpPost("{userId}/roles")]
    [HasPermission(Permissions.Users.AssignRoles)]
    public async Task<IActionResult> AssignRoleToUser(Guid userId, [FromBody] AssignRoleRequest request)
    {
        var user = await _context.Users.FindAsync(new object[] { userId });
        var role = await _context.Roles.FindAsync(new object[] { request.RoleId });

        if (user == null || role == null)
            return NotFound("User or Role not found");

        var userRole = new UserRole(user.Id, role.Id);
        _context.UserRoles.Add(userRole);
        await _context.SaveChangesAsync(CancellationToken.None);

        return Ok();
    }
}

public record AssignRoleRequest(Guid RoleId);
