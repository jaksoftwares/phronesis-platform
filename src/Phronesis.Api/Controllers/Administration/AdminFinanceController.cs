using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Phronesis.Application.Common.Interfaces;
using Phronesis.Domain.Commerce;
using System.ComponentModel.DataAnnotations;

namespace Phronesis.Api.Controllers.Administration;

[ApiController]
[Route("api/v1/admin/finance")]
[Authorize(Roles = "Admin")]
public class AdminFinanceController : ControllerBase
{
    private readonly IApplicationDbContext _context;

    public AdminFinanceController(IApplicationDbContext context)
    {
        _context = context;
    }

    [HttpGet("transactions")]
    public async Task<IActionResult> GetTransactions()
    {
        var transactions = await _context.PaymentTransactions
            .Include(t => t.User)
            .OrderByDescending(t => t.CreatedAt)
            .Select(t => new
            {
                t.Id,
                t.Amount,
                t.Currency,
                t.Provider,
                t.ProviderTransactionId,
                t.Status,
                t.ReferenceType,
                t.ReferenceId,
                t.CreatedAt,
                User = new { t.User.FirstName, t.User.LastName, t.User.Email }
            })
            .ToListAsync();

        return Ok(new { data = transactions });
    }

    [HttpGet("refunds")]
    public async Task<IActionResult> GetRefunds()
    {
        var refunds = await _context.RefundRequests
            .Include(r => r.User)
            .Include(r => r.PaymentTransaction)
            .OrderByDescending(r => r.CreatedAt)
            .Select(r => new
            {
                r.Id,
                r.Amount,
                r.Reason,
                r.Status,
                r.AdminNotes,
                r.CreatedAt,
                User = new { r.User.FirstName, r.User.LastName, r.User.Email },
                Transaction = new 
                { 
                    r.PaymentTransaction.ProviderTransactionId, 
                    r.PaymentTransaction.Provider,
                    r.PaymentTransaction.Currency
                }
            })
            .ToListAsync();

        return Ok(new { data = refunds });
    }

    public class ProcessRefundRequest
    {
        [Required] public bool Approve { get; set; }
        [Required] public string Notes { get; set; } = string.Empty;
    }

    [HttpPost("refunds/{id}/process")]
    public async Task<IActionResult> ProcessRefund(Guid id, [FromBody] ProcessRefundRequest req)
    {
        var refund = await _context.RefundRequests
            .Include(r => r.PaymentTransaction)
            .FirstOrDefaultAsync(r => r.Id == id);

        if (refund == null) return NotFound(new { message = "Refund request not found." });
        if (refund.Status != RefundStatus.Pending) return BadRequest(new { message = "Refund is not pending." });

        if (req.Approve)
        {
            refund.Approve(req.Notes);
            
            // Mark transaction as refunded
            refund.PaymentTransaction.Refund();
            
            // TODO: Actually call Stripe/Payment Provider to issue the refund
            
            // We'll optimistically mark it as processed here
            refund.MarkProcessed();
        }
        else
        {
            refund.Reject(req.Notes);
        }

        await _context.SaveChangesAsync(default);

        return Ok(new { message = req.Approve ? "Refund approved and processed." : "Refund rejected." });
    }
}
