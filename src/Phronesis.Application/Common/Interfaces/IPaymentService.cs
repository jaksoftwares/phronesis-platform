using Phronesis.Domain.Commerce;

namespace Phronesis.Application.Common.Interfaces;

public class PaymentRequest
{
    public Guid UserId { get; set; }
    public decimal Amount { get; set; }
    public string Currency { get; set; } = "USD";
    public PaymentProvider Provider { get; set; }
    public PaymentReferenceType ReferenceType { get; set; }
    public string ReferenceId { get; set; } = string.Empty;
}

public class PaymentResult
{
    public bool Success { get; set; }
    public Guid TransactionId { get; set; }
    public string? ProviderTransactionId { get; set; }
    public string? ClientSecret { get; set; } // For Stripe
    public string? ErrorMessage { get; set; }
}

public interface IPaymentService
{
    Task<PaymentResult> InitiatePaymentAsync(PaymentRequest request, CancellationToken cancellationToken = default);
    Task<bool> VerifyPaymentAsync(Guid transactionId, CancellationToken cancellationToken = default);
}

public class ProviderResponse
{
    public bool Success { get; set; }
    public string? ProviderTransactionId { get; set; }
    public string? ClientSecret { get; set; }
    public string? ErrorMessage { get; set; }
}

public interface IPaymentProvider
{
    PaymentProvider ProviderType { get; }
    Task<ProviderResponse> ProcessAsync(decimal amount, string currency, string reference, CancellationToken cancellationToken = default);
}
