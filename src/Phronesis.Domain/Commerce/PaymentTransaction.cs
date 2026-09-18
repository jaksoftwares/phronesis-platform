using Phronesis.Domain.Common;
using Phronesis.Domain.Identity;

namespace Phronesis.Domain.Commerce;

public enum PaymentProvider
{
    Mock,
    Stripe,
    MPesa
}

public enum PaymentStatus
{
    Pending,
    Successful,
    Failed,
    Refunded
}

public enum PaymentReferenceType
{
    SubscriptionPlan,
    OneOffClass,
    Other
}

public class PaymentTransaction : BaseEntity
{
    public Guid UserId { get; private set; }
    public decimal Amount { get; private set; }
    public string Currency { get; private set; }
    
    public PaymentProvider Provider { get; private set; }
    public string? ProviderTransactionId { get; private set; }
    
    public PaymentStatus Status { get; private set; }
    
    public PaymentReferenceType ReferenceType { get; private set; }
    public string ReferenceId { get; private set; }
    
    public string? ErrorMessage { get; private set; }

    public User User { get; private set; } = null!;

    private PaymentTransaction() { } // EF Core

    public PaymentTransaction(
        Guid userId, 
        decimal amount, 
        string currency, 
        PaymentProvider provider, 
        PaymentReferenceType referenceType, 
        string referenceId)
    {
        UserId = userId;
        Amount = amount;
        Currency = currency;
        Provider = provider;
        Status = PaymentStatus.Pending;
        ReferenceType = referenceType;
        ReferenceId = referenceId;
    }

    public void MarkAsSuccessful(string providerTransactionId)
    {
        if (Status != PaymentStatus.Pending)
            throw new InvalidOperationException("Only pending transactions can be marked as successful.");

        Status = PaymentStatus.Successful;
        ProviderTransactionId = providerTransactionId;
    }

    public void MarkAsFailed(string errorMessage)
    {
        if (Status != PaymentStatus.Pending)
            throw new InvalidOperationException("Only pending transactions can be marked as failed.");

        Status = PaymentStatus.Failed;
        ErrorMessage = errorMessage;
    }

    public void Refund()
    {
        if (Status != PaymentStatus.Successful)
            throw new InvalidOperationException("Only successful transactions can be refunded.");

        Status = PaymentStatus.Refunded;
    }
}
