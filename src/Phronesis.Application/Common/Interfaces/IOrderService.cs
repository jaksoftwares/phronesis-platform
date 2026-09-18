using Phronesis.Domain.Commerce;

namespace Phronesis.Application.Common.Interfaces;

public interface IOrderService
{
    Task<Order> CreateOrderFromTransactionAsync(PaymentTransaction transaction, CancellationToken cancellationToken = default);
}
