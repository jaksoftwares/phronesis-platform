using Microsoft.EntityFrameworkCore;
using Phronesis.Application.Common.Interfaces;
using Phronesis.Domain.Commerce;

namespace Phronesis.Infrastructure.Services.Payments;

public class PaymentService : IPaymentService
{
    private readonly IApplicationDbContext _context;
    private readonly IEnumerable<IPaymentProvider> _providers;
    private readonly IOrderService _orderService;

    public PaymentService(IApplicationDbContext context, IEnumerable<IPaymentProvider> providers, IOrderService orderService)
    {
        _context = context;
        _providers = providers;
        _orderService = orderService;
    }

    public async Task<PaymentResult> InitiatePaymentAsync(PaymentRequest request, CancellationToken cancellationToken = default)
    {
        // 1. Log transaction as pending
        var transaction = new PaymentTransaction(
            request.UserId,
            request.Amount,
            request.Currency,
            request.Provider,
            request.ReferenceType,
            request.ReferenceId
        );

        _context.PaymentTransactions.Add(transaction);
        await _context.SaveChangesAsync(cancellationToken);

        // 2. Find correct provider
        var provider = _providers.FirstOrDefault(p => p.ProviderType == request.Provider);
        if (provider == null)
        {
            transaction.MarkAsFailed("Payment provider not supported.");
            await _context.SaveChangesAsync(cancellationToken);
            return new PaymentResult { Success = false, ErrorMessage = "Payment provider not supported." };
        }

        // 3. Process via provider
        var response = await provider.ProcessAsync(request.Amount, request.Currency, transaction.Id.ToString(), cancellationToken);

        // 4. Update transaction state
        if (response.Success && response.ProviderTransactionId != null)
        {
            transaction.MarkAsSuccessful(response.ProviderTransactionId);

            // M20: Automatically generate Order and Invoice
            await _orderService.CreateOrderFromTransactionAsync(transaction, cancellationToken);

            // M18/M19: Provision subscription on successful mock payment
            if (transaction.ReferenceType == PaymentReferenceType.SubscriptionPlan)
            {
                await ProvisionSubscriptionAsync(transaction.UserId, Guid.Parse(transaction.ReferenceId), cancellationToken);
            }
        }
        else
        {
            transaction.MarkAsFailed(response.ErrorMessage ?? "Unknown provider error.");
        }

        await _context.SaveChangesAsync(cancellationToken);

        return new PaymentResult
        {
            Success = response.Success,
            TransactionId = transaction.Id,
            ProviderTransactionId = response.ProviderTransactionId,
            ClientSecret = response.ClientSecret,
            ErrorMessage = response.ErrorMessage
        };
    }

    public async Task<bool> VerifyPaymentAsync(Guid transactionId, CancellationToken cancellationToken = default)
    {
        var transaction = await _context.PaymentTransactions.FindAsync(new object[] { transactionId }, cancellationToken);
        return transaction != null && transaction.Status == PaymentStatus.Successful;
    }

    private async Task ProvisionSubscriptionAsync(Guid userId, Guid planId, CancellationToken cancellationToken)
    {
        var plan = await _context.SubscriptionPlans.FindAsync(new object[] { planId }, cancellationToken);
        if (plan == null) return;

        // Expire existing
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
    }
}
