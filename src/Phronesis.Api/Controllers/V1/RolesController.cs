using Microsoft.AspNetCore.Mvc;
using Phronesis.Application.Common.Interfaces;
using Phronesis.Domain.Identity;
using Phronesis.Infrastructure.Authentication;
using Microsoft.EntityFrameworkCore;

namespace Phronesis.Api.Controllers.V1;

[ApiController]
[Route("api/v1/[controller]")]
public class RolesController : ControllerBase
{
    private readonly IApplicationDbContext _context;

    public RolesController(IApplicationDbContext context)
    {
        _context = context;
    }

    [HttpPost]
    [HasPermission(Permissions.Roles.Create)]
    public async Task<IActionResult> CreateRole([FromBody] CreateRoleRequest request)
    {
        var role = new Role(request.Name, request.Description);
        _context.Roles.Add(role);
        await _context.SaveChangesAsync(CancellationToken.None);
        return Ok(new { role.Id, role.Name, role.Description });
    }

    [HttpPost("{roleId}/permissions")]
    [HasPermission(Permissions.Roles.Edit)]
    public async Task<IActionResult> AssignPermissionToRole(Guid roleId, [FromBody] AssignPermissionRequest request)
    {
        var role = await _context.Roles.FindAsync(new object[] { roleId });
        var permission = await _context.Permissions.FindAsync(new object[] { request.PermissionId });

        if (role == null || permission == null)
            return NotFound("Role or Permission not found");

        var rolePermission = new RolePermission(role.Id, permission.Id);
        _context.RolePermissions.Add(rolePermission);
        await _context.SaveChangesAsync(CancellationToken.None);

        return Ok();
    }
}

public record CreateRoleRequest(string Name, string Description);
public record AssignPermissionRequest(Guid PermissionId);
