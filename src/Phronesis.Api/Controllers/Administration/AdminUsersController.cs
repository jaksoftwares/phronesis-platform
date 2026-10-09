using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Phronesis.Application.Common.Interfaces;
using Phronesis.Domain.Identity;

namespace Phronesis.Api.Controllers.Administration;

[ApiController]
[Route("api/v1/admin/users")]
[Authorize(Roles = "Admin")]
public class AdminUsersController : ControllerBase
{
    private readonly IApplicationDbContext _context;

    public AdminUsersController(IApplicationDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<IActionResult> GetUsers([FromQuery] string? role, [FromQuery] int page = 1, [FromQuery] int pageSize = 20)
    {
        var query = _context.Users.Include(u => u.UserRoles).ThenInclude(ur => ur.Role).AsQueryable();

        if (!string.IsNullOrEmpty(role))
        {
            query = query.Where(u => u.UserRoles.Any(ur => ur.Role.Name == role));
        }

        var totalItems = await query.CountAsync();
        var users = await query
            .OrderByDescending(u => u.CreatedAt)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Select(u => new
            {
                u.Id,
                u.FirstName,
                u.LastName,
                u.Email,
                u.IsActive,
                u.EmailConfirmed,
                u.CreatedAt,
                Roles = u.UserRoles.Select(ur => ur.Role.Name).ToList()
            })
            .ToListAsync();

        return Ok(new
        {
            data = users,
            meta = new
            {
                page,
                pageSize,
                totalItems,
                totalPages = (int)Math.Ceiling(totalItems / (double)pageSize)
            }
        });
    }

    [HttpPost("{id}/deactivate")]
    public async Task<IActionResult> DeactivateUser(Guid id)
    {
        var user = await _context.Users.FindAsync(id);
        if (user == null) return NotFound(new { message = "User not found" });

        user.Deactivate();
        await _context.SaveChangesAsync(default);

        return Ok(new { message = "User deactivated successfully." });
    }

    [HttpPost("{id}/activate")]
    public async Task<IActionResult> ActivateUser(Guid id)
    {
        var user = await _context.Users.FindAsync(id);
        if (user == null) return NotFound(new { message = "User not found" });

        user.Activate();
        await _context.SaveChangesAsync(default);

        return Ok(new { message = "User activated successfully." });
    }

    public class AssignRolesDto
    {
        public List<string> Roles { get; set; } = new();
    }

    [HttpPut("{id}/roles")]
    public async Task<IActionResult> AssignRoles(Guid id, [FromBody] AssignRolesDto req)
    {
        var user = await _context.Users
            .Include(u => u.UserRoles)
            .FirstOrDefaultAsync(u => u.Id == id);
            
        if (user == null) return NotFound(new { message = "User not found" });

        // Remove existing roles
        _context.UserRoles.RemoveRange(user.UserRoles);
        
        // Add new roles
        var validRoles = await _context.Roles
            .Where(r => req.Roles.Contains(r.Name))
            .ToListAsync();
            
        foreach (var role in validRoles)
        {
            user.UserRoles.Add(new UserRole(user.Id, role.Id));
        }

        await _context.SaveChangesAsync(default);

        return Ok(new { message = "User roles updated successfully." });
    }
}

