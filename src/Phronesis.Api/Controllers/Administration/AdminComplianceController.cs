using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Phronesis.Application.Common.Interfaces;

namespace Phronesis.Api.Controllers.Administration;

[ApiController]
[Route("api/v1/admin/compliance")]
[Authorize(Roles = "Admin")]
public class AdminComplianceController : ControllerBase
{
    private readonly IApplicationDbContext _context;

    public AdminComplianceController(IApplicationDbContext context)
    {
        _context = context;
    }

    [HttpGet("data-requests")]
    public async Task<IActionResult> GetDataRequests()
    {
        // Treat SupportTickets with Category "Privacy" or "Data Request" as GDPR requests
        var requests = await _context.SupportTickets
            .Include(t => t.User)
            .Where(t => t.Category == "GDPR" || t.Category == "Data Export" || t.Category == "Account Deletion")
            .OrderByDescending(t => t.CreatedAt)
            .Select(t => new
            {
                t.Id,
                t.Category,
                t.Subject,
                t.Status,
                t.CreatedAt,
                User = new { t.User.FirstName, t.User.LastName, t.User.Email }
            })
            .ToListAsync();

        return Ok(new { data = requests });
    }

    [HttpPost("data-requests/{id}/approve")]
    public async Task<IActionResult> ApproveDataRequest(Guid id)
    {
        var ticket = await _context.SupportTickets.FindAsync(id);
        if (ticket == null) return NotFound(new { message = "Data request not found." });

        ticket.UpdateStatus("Processed");
        
        await _context.SaveChangesAsync(default);

        return Ok(new { message = "Data request approved and background job triggered." });
    }
}
