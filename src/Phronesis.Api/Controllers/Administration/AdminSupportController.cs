using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Phronesis.Application.Common.Interfaces;
using Phronesis.Domain.Support;
using System.ComponentModel.DataAnnotations;

namespace Phronesis.Api.Controllers.Administration;

[ApiController]
[Route("api/v1/admin/support")]
[Authorize(Roles = "Admin")]
public class AdminSupportController : ControllerBase
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUserService _currentUserService;

    public AdminSupportController(IApplicationDbContext context, ICurrentUserService currentUserService)
    {
        _context = context;
        _currentUserService = currentUserService;
    }

    [HttpGet("tickets")]
    public async Task<IActionResult> GetTickets()
    {
        var tickets = await _context.SupportTickets
            .Include(t => t.User)
            .Include(t => t.AssignedAgent)
            .OrderByDescending(t => t.CreatedAt)
            .Select(t => new
            {
                t.Id,
                t.Category,
                t.Subject,
                t.Description,
                t.Status,
                t.Priority,
                t.CreatedAt,
                User = new { t.User.FirstName, t.User.LastName, t.User.Email },
                AssignedAgent = t.AssignedAgent != null ? new { t.AssignedAgent.FirstName, t.AssignedAgent.LastName } : null
            })
            .ToListAsync();

        return Ok(new { data = tickets });
    }

    [HttpPost("tickets/{id}/assign")]
    public async Task<IActionResult> AssignTicket(Guid id)
    {
        var ticket = await _context.SupportTickets.FindAsync(id);
        if (ticket == null) return NotFound(new { message = "Ticket not found." });

        var adminId = Guid.Parse(_currentUserService.UserId!);
        ticket.AssignAgent(adminId);

        await _context.SaveChangesAsync(default);
        return Ok(new { message = "Ticket assigned to you." });
    }

    public class UpdateTicketStatusRequest
    {
        [Required] public string Status { get; set; } = string.Empty;
    }

    [HttpPost("tickets/{id}/status")]
    public async Task<IActionResult> UpdateTicketStatus(Guid id, [FromBody] UpdateTicketStatusRequest req)
    {
        var ticket = await _context.SupportTickets.FindAsync(id);
        if (ticket == null) return NotFound(new { message = "Ticket not found." });

        ticket.UpdateStatus(req.Status);
        
        await _context.SaveChangesAsync(default);
        return Ok(new { message = "Ticket status updated." });
    }
}
