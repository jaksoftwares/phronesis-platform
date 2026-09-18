using Phronesis.Application.Common.Interfaces;
using Phronesis.Domain.Commerce;

namespace Phronesis.Infrastructure.Services.Payments;

public class MockPaymentProvider : IPaymentProvider
{
    public PaymentProvider ProviderType => PaymentProvider.Mock;

    public Task<ProviderResponse> ProcessAsync(decimal amount, string currency, string reference, CancellationToken cancellationToken = default)
    {
        // Simulate a delay
        Thread.Sleep(500);

        // Always succeed for mock provider
        return Task.FromResult(new ProviderResponse
        {
            Success = true,
            ProviderTransactionId = $"MOCK_TXN_{Guid.NewGuid().ToString("N").Substring(0, 10).ToUpper()}",
            ClientSecret = "mock_client_secret_xyz"
        });
    }
}
