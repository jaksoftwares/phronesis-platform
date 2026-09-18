using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Phronesis.Application.Common.Interfaces;

namespace Phronesis.Api.Controllers.V1;

[ApiController]
[Route("api/v1/[controller]")]
[Authorize]
public class AccountingController : ControllerBase
{
    private readonly IApplicationDbContext _context;

    public AccountingController(IApplicationDbContext context)
    {
        _context = context;
    }

    [HttpGet("invoices/my")]
    public async Task<IActionResult> GetMyInvoices(CancellationToken cancellationToken)
    {
        var userIdString = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
        if (!Guid.TryParse(userIdString, out var userId))
            return Unauthorized();

        var invoices = await _context.Invoices
            .Include(i => i.Order)
            .Where(i => i.Order.UserId == userId)
            .OrderByDescending(i => i.IssueDate)
            .Select(i => new
            {
                i.Id,
                i.InvoiceNumber,
                i.Order.TotalAmount,
                i.Order.Currency,
                i.IssueDate,
                Status = i.Status.ToString()
            })
            .ToListAsync(cancellationToken);

        return Ok(invoices);
    }

    [HttpGet("admin/reconciliation")]
    [Authorize(Policy = "RequireAdmin")]
    public async Task<IActionResult> GetReconciliationReport(CancellationToken cancellationToken)
    {
        // Admin view to balance books
        var report = await _context.PaymentTransactions
            .Include(pt => pt.User)
            .OrderByDescending(pt => pt.CreatedAt)
            .Take(100)
            .Select(pt => new
            {
                pt.Id,
                pt.ProviderTransactionId,
                pt.Amount,
                pt.Currency,
                Status = pt.Status.ToString(),
                Provider = pt.Provider.ToString(),
                UserEmail = pt.User.Email,
                pt.CreatedAt
            })
            .ToListAsync(cancellationToken);

        return Ok(new { TotalTransactions = report.Count, Report = report });
    }
}
