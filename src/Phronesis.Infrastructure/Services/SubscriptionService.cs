using Microsoft.EntityFrameworkCore;
using Phronesis.Application.Common.Interfaces;
using Phronesis.Domain.Commerce;

namespace Phronesis.Infrastructure.Services;

public class SubscriptionService : ISubscriptionService
{
    private readonly IApplicationDbContext _context;

    public SubscriptionService(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<bool> HasActivePremiumSubscriptionAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        return await _context.UserSubscriptions
            .AnyAsync(us => us.UserId == userId 
                         && us.Status == SubscriptionStatus.Active 
                         && us.CurrentPeriodEnd > DateTime.UtcNow, 
                cancellationToken);
    }
}
