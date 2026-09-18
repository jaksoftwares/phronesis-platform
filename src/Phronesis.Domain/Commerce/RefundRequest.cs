using Phronesis.Domain.Common;
using Phronesis.Domain.Identity;

namespace Phronesis.Domain.Commerce;

public enum RefundStatus
{
    Pending,
    Approved,
    Rejected,
    Processed
}

public class RefundRequest : BaseEntity
{
    public Guid PaymentTransactionId { get; private set; }
    public Guid UserId { get; private set; }
    
    public decimal Amount { get; private set; }
    public string Reason { get; private set; }
    
    public RefundStatus Status { get; private set; }
    public string? AdminNotes { get; private set; }

    public PaymentTransaction PaymentTransaction { get; private set; } = null!;
    public User User { get; private set; } = null!;

    private RefundRequest() { } // EF Core

    public RefundRequest(Guid paymentTransactionId, Guid userId, decimal amount, string reason)
    {
        PaymentTransactionId = paymentTransactionId;
        UserId = userId;
        Amount = amount;
        Reason = reason;
        Status = RefundStatus.Pending;
    }

    public void Approve(string notes)
    {
        Status = RefundStatus.Approved;
        AdminNotes = notes;
    }

    public void Reject(string notes)
    {
        Status = RefundStatus.Rejected;
        AdminNotes = notes;
    }

    public void MarkProcessed()
    {
        Status = RefundStatus.Processed;
    }
}
