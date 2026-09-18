using Phronesis.Domain.Common;

namespace Phronesis.Domain.Commerce;

public enum InvoiceStatus
{
    Draft,
    Unpaid,
    Paid,
    Void
}

public class Invoice : BaseEntity
{
    public string InvoiceNumber { get; private set; }
    public Guid OrderId { get; private set; }
    
    public DateTime IssueDate { get; private set; }
    public DateTime? DueDate { get; private set; }
    
    public InvoiceStatus Status { get; private set; }
    public string? PdfUri { get; private set; } // For future document generation

    public Order Order { get; private set; } = null!;

    private Invoice() { } // EF Core

    public Invoice(Guid orderId)
    {
        InvoiceNumber = $"INV-{DateTime.UtcNow.Year}-{Guid.NewGuid().ToString("N").Substring(0, 8).ToUpper()}";
        OrderId = orderId;
        IssueDate = DateTime.UtcNow;
        Status = InvoiceStatus.Paid; // Since we generate upon successful payment in MVP
    }

    public void VoidInvoice()
    {
        Status = InvoiceStatus.Void;
    }
}
