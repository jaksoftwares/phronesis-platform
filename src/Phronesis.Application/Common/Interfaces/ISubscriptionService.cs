namespace Phronesis.Application.Common.Interfaces;

public interface ISubscriptionService
{
    Task<bool> HasActivePremiumSubscriptionAsync(Guid userId, CancellationToken cancellationToken = default);
}
