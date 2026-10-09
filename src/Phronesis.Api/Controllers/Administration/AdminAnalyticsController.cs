using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Phronesis.Application.Common.Interfaces;

namespace Phronesis.Api.Controllers.Administration;

[ApiController]
[Route("api/v1/admin/analytics")]
[Authorize(Roles = "Admin")]
public class AdminAnalyticsController : ControllerBase
{
    private readonly IApplicationDbContext _context;

    public AdminAnalyticsController(IApplicationDbContext context)
    {
        _context = context;
    }

    [HttpGet("overview")]
    public async Task<IActionResult> GetOverview()
    {
        var totalUsers = await _context.Users.CountAsync(u => u.IsActive);
        
        var totalRevenue = await _context.PaymentTransactions
            .Where(t => t.Status == Domain.Commerce.PaymentStatus.Successful)
            .SumAsync(t => t.Amount);

        var activeClasses = await _context.VirtualClasses.CountAsync(c => c.IsActive);

        var activeSubscriptions = await _context.UserSubscriptions
            .CountAsync(s => s.Status == Domain.Commerce.SubscriptionStatus.Active);

        var pendingReports = await _context.SupportTickets
            .CountAsync(t => t.Status == "Open" || t.Status == "Pending");

        return Ok(new
        {
            data = new
            {
                TotalUsers = totalUsers,
                TotalRevenue = totalRevenue,
                ActiveClasses = activeClasses,
                ActiveSubscriptions = activeSubscriptions,
                PendingReports = pendingReports
            }
        });
    }
}
