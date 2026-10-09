using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Phronesis.Application.Common.Interfaces;
using Phronesis.Domain.Communication;
using System.ComponentModel.DataAnnotations;

namespace Phronesis.Api.Controllers.Administration;

[ApiController]
[Route("api/v1/admin/notifications")]
[Authorize(Roles = "Admin")]
public class AdminNotificationsController : ControllerBase
{
    private readonly IApplicationDbContext _context;

    public AdminNotificationsController(IApplicationDbContext context)
    {
        _context = context;
    }

    public class BroadcastRequest
    {
        [Required] public string Title { get; set; } = string.Empty;
        [Required] public string Message { get; set; } = string.Empty;
        [Required] public string TargetAudience { get; set; } = "All"; // All, Teachers, Learners, Guardians
    }

    [HttpPost("broadcast")]
    public async Task<IActionResult> Broadcast([FromBody] BroadcastRequest req)
    {
        var usersQuery = _context.Users.Where(u => u.IsActive);

        if (req.TargetAudience != "All")
        {
            usersQuery = usersQuery.Where(u => u.UserRoles.Any(ur => ur.Role.Name == req.TargetAudience));
        }

        var targetUsers = await usersQuery.Select(u => u.Id).ToListAsync();

        var notifications = targetUsers.Select(userId => 
            new Notification(userId, req.Title, req.Message, "SystemBroadcast")
        ).ToList();

        await _context.Notifications.AddRangeAsync(notifications);
        await _context.SaveChangesAsync(default);

        return Ok(new { message = $"Broadcast sent to {notifications.Count} users." });
    }

    [HttpGet("history")]
    public async Task<IActionResult> GetBroadcastHistory()
    {
        // For simplicity, we just return all SystemBroadcast notifications, grouped by Title/Message and CreatedAt.
        // A better design would be a separate Broadcast entity.
        var broadcasts = await _context.Notifications
            .Where(n => n.Type == "SystemBroadcast")
            .GroupBy(n => new { n.Title, n.Message, CreatedAt = n.CreatedAt.Date })
            .Select(g => new
            {
                g.Key.Title,
                g.Key.Message,
                g.Key.CreatedAt,
                RecipientCount = g.Count()
            })
            .OrderByDescending(b => b.CreatedAt)
            .ToListAsync();

        return Ok(new { data = broadcasts });
    }
}
