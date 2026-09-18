using Phronesis.Application.Common.Interfaces;
using Phronesis.Domain.Commerce;

namespace Phronesis.Infrastructure.Services.Commerce;

public class OrderService : IOrderService
{
    private readonly IApplicationDbContext _context;

    public OrderService(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Order> CreateOrderFromTransactionAsync(PaymentTransaction transaction, CancellationToken cancellationToken = default)
    {
        if (transaction.Status != PaymentStatus.Successful)
            throw new InvalidOperationException("Can only create an order for a successful payment.");

        var order = new Order(transaction.UserId, transaction.Id, transaction.Amount, transaction.Currency);
        _context.Orders.Add(order);
        
        var invoice = new Invoice(order.Id);
        _context.Invoices.Add(invoice);

        await _context.SaveChangesAsync(cancellationToken);

        return order;
    }
}
