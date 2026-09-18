using Phronesis.Domain.Common;
using Phronesis.Domain.Identity;

namespace Phronesis.Domain.Commerce;

public enum OrderStatus
{
    Pending,
    Completed,
    Cancelled,
    Refunded
}

public class Order : BaseEntity
{
    public string OrderNumber { get; private set; }
    public Guid UserId { get; private set; }
    public Guid PaymentTransactionId { get; private set; }
    
    public decimal TotalAmount { get; private set; }
    public string Currency { get; private set; }
    
    public OrderStatus Status { get; private set; }

    // Navigations
    public User User { get; private set; } = null!;
    public PaymentTransaction PaymentTransaction { get; private set; } = null!;
    public Invoice? Invoice { get; private set; }

    private Order() { } // EF Core

    public Order(Guid userId, Guid paymentTransactionId, decimal totalAmount, string currency)
    {
        OrderNumber = $"ORD-{DateTime.UtcNow.Year}-{Guid.NewGuid().ToString("N").Substring(0, 8).ToUpper()}";
        UserId = userId;
        PaymentTransactionId = paymentTransactionId;
        TotalAmount = totalAmount;
        Currency = currency;
        Status = OrderStatus.Completed; // In our flow, order is created after payment success
    }

    public void MarkAsRefunded()
    {
        Status = OrderStatus.Refunded;
    }
}
