using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Phronesis.Application.Common.Interfaces;
using Phronesis.Domain.Commerce;

namespace Phronesis.Api.Controllers.V1;

[ApiController]
[Route("api/v1/[controller]")]
[Authorize]
public class CommerceController : ControllerBase
{
    private readonly IApplicationDbContext _context;

    public CommerceController(IApplicationDbContext context)
    {
        _context = context;
    }

    [HttpGet("plans")]
    [AllowAnonymous]
    public async Task<IActionResult> GetActivePlans(CancellationToken cancellationToken)
    {
        var plans = await _context.SubscriptionPlans
            .Where(p => p.IsActive)
            .Select(p => new
            {
                p.Id,
                p.Name,
                p.Description,
                p.PlanCode,
                p.Price,
                p.Currency,
                Interval = p.Interval.ToString(),
                p.FeaturesJson
            })
            .ToListAsync(cancellationToken);

        return Ok(plans);
    }

    [HttpGet("subscriptions/my")]
    public async Task<IActionResult> GetMySubscriptions(CancellationToken cancellationToken)
    {
        var userIdString = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
        if (!Guid.TryParse(userIdString, out var userId))
            return Unauthorized();

        var subscriptions = await _context.UserSubscriptions
            .Include(us => us.Plan)
            .Where(us => us.UserId == userId)
            .OrderByDescending(us => us.CurrentPeriodEnd)
            .Select(us => new
            {
                us.Id,
                PlanName = us.Plan.Name,
                Status = us.Status.ToString(),
                us.CurrentPeriodStart,
                us.CurrentPeriodEnd,
                us.CancelAtPeriodEnd
            })
            .ToListAsync(cancellationToken);

        return Ok(subscriptions);
    }

    [HttpPost("subscriptions/mock-provision")]
    public async Task<IActionResult> MockProvisionSubscription([FromBody] MockProvisionRequest request, CancellationToken cancellationToken)
    {
        // Security check - ideally this is for dev only
        var userIdString = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
        if (!Guid.TryParse(userIdString, out var userId))
            return Unauthorized();

        var plan = await _context.SubscriptionPlans.FirstOrDefaultAsync(p => p.PlanCode == request.PlanCode, cancellationToken);
        if (plan == null)
            return NotFound("Plan not found.");

        // Expire old active ones just in case
        var activeSubs = await _context.UserSubscriptions
            .Where(us => us.UserId == userId && us.Status == SubscriptionStatus.Active)
            .ToListAsync(cancellationToken);

        foreach (var sub in activeSubs)
        {
            sub.Expire();
        }

        var newSubscription = new UserSubscription(
            userId,
            plan.Id,
            DateTime.UtcNow,
            plan.Interval == BillingInterval.Monthly ? DateTime.UtcNow.AddMonths(1) : DateTime.UtcNow.AddYears(1)
        );

        _context.UserSubscriptions.Add(newSubscription);
        await _context.SaveChangesAsync(cancellationToken);

        return Ok(new
        {
            Message = "Successfully provisioned mock subscription.",
            SubscriptionId = newSubscription.Id,
            CurrentPeriodEnd = newSubscription.CurrentPeriodEnd
        });
    }
}

public class MockProvisionRequest
{
    public string PlanCode { get; set; } = string.Empty;
in/finance/reconciliation/run")]
    public IActionResult RunReconciliation() => StatusCode(501);
in/finance/reconciliation/run")]
    public IActionResult RunReconciliation() => StatusCode(501);
}



// {
//     [HttpGet("plans")]
//     public IActionResult ActivePlans() => StatusCode(501);

//     [HttpPost("plans")]
//     public IActionResult CreatePlan() => StatusCode(501);

//     [HttpGet("plans/{planId}")]
//     public IActionResult GetPlan(string planId) => StatusCode(501);

//     [HttpPatch("plans/{planId}")]
//     public IActionResult UpdatePlan(string planId) => StatusCode(501);

//     [HttpPost("plans/{planId}/activate")]
//     public IActionResult ActivatePlan(string planId) => StatusCode(501);

//     [HttpPost("plans/{planId}/deactivate")]
//     public IActionResult DeactivatePlan(string planId) => StatusCode(501);

//     [HttpGet("subscriptions")]
//     public IActionResult ListSubscriptions() => StatusCode(501);

//     [HttpPost("subscriptions")]
//     public IActionResult PurchaseSubscription() => StatusCode(501);

//     [HttpGet("subscriptions/{subscriptionId}")]
//     public IActionResult GetSubscription(string subscriptionId) => StatusCode(501);

//     [HttpPost("subscriptions/{subscriptionId}/cancel")]
//     public IActionResult CancelSubscription(string subscriptionId) => StatusCode(501);

//     [HttpPost("subscriptions/{subscriptionId}/renew")]
//     public IActionResult RenewSubscription(string subscriptionId) => StatusCode(501);

//     [HttpGet("learners/{learnerId}/entitlements")]
//     public IActionResult LearnerEntitlements(string learnerId) => StatusCode(501);

//     [HttpGet("teachers/{teacherId}/entitlements")]
//     public IActionResult TeacherEntitlements(string teacherId) => StatusCode(501);

//     [HttpGet("payment-methods")]
//     public IActionResult AvailablePaymentMethods() => StatusCode(501);

//     [HttpPost("payments")]
//     public IActionResult InitiatePayment() => StatusCode(501);

//     [HttpGet("payments/{paymentId}")]
//     public IActionResult PaymentStatus(string paymentId) => StatusCode(501);

//     [HttpPost("payments/{paymentId}/retry")]
//     public IActionResult RetryFailedPayment(string paymentId) => StatusCode(501);

//     [HttpPost("payments/mpesa/stk-push")]
//     public IActionResult InitiateMpesaStk() => StatusCode(501);

//     [HttpPost("payments/webhooks/mpesa")]
//     public IActionResult MpesaCallback() => StatusCode(501);

//     [HttpPost("payments/webhooks/{provider}")]
//     public IActionResult ProviderCallback(string provider) => StatusCode(501);

//     [HttpGet("payments/{paymentId}/receipt")]
//     public IActionResult PaymentReceipt(string paymentId) => StatusCode(501);

//     [HttpGet("orders")]
//     public IActionResult ListOrders() => StatusCode(501);

//     [HttpPost("orders")]
//     public IActionResult CreateOrder() => StatusCode(501);

//     [HttpGet("orders/{orderId}")]
//     public IActionResult GetOrder(string orderId) => StatusCode(501);

//     [HttpGet("invoices")]
//     public IActionResult ListInvoices() => StatusCode(501);

//     [HttpGet("invoices/{invoiceId}")]
//     public IActionResult GetInvoice(string invoiceId) => StatusCode(501);

//     [HttpGet("invoices/{invoiceId}/download")]
//     public IActionResult DownloadInvoice(string invoiceId) => StatusCode(501);

//     [HttpPost("refunds")]
//     public IActionResult ProcessRefund() => StatusCode(501);

//     [HttpGet("refunds/{refundId}")]
//     public IActionResult GetRefund(string refundId) => StatusCode(501);

//     [HttpGet("admin/finance/reconciliation")]
//     public IActionResult ReconciliationView() => StatusCode(501);

//     [HttpPost("admin/finance/reconciliation/run")]
//     public IActionResult RunReconciliation() => StatusCode(501);
// }
