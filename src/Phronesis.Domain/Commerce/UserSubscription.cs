using Phronesis.Domain.Common;
using Phronesis.Domain.Identity;

namespace Phronesis.Domain.Commerce;

public enum SubscriptionStatus
{
    Active,
    PastDue,
    Canceled,
    Expired
}

public class UserSubscription : BaseEntity
{
    public Guid UserId { get; private set; }
    public Guid SubscriptionPlanId { get; private set; }
    
    public SubscriptionStatus Status { get; private set; }
    public DateTime CurrentPeriodStart { get; private set; }
    public DateTime CurrentPeriodEnd { get; private set; }
    public bool CancelAtPeriodEnd { get; private set; }

    public User User { get; private set; } = null!;
    public SubscriptionPlan Plan { get; private set; } = null!;

    private UserSubscription() { }

    public UserSubscription(Guid userId, Guid subscriptionPlanId, DateTime periodStart, DateTime periodEnd)
    {
        UserId = userId;
        SubscriptionPlanId = subscriptionPlanId;
        Status = SubscriptionStatus.Active;
        CurrentPeriodStart = periodStart;
        CurrentPeriodEnd = periodEnd;
        CancelAtPeriodEnd = false;
    }

    public void Renew(DateTime newPeriodEnd)
    {
        Status = SubscriptionStatus.Active;
        CurrentPeriodStart = CurrentPeriodEnd; // Start of new period is end of old period
        CurrentPeriodEnd = newPeriodEnd;
        CancelAtPeriodEnd = false;
    }

    public void Cancel(bool immediately = false)
    {
        if (immediately)
        {
            Status = SubscriptionStatus.Canceled;
            CurrentPeriodEnd = DateTime.UtcNow;
        }
        else
        {
            CancelAtPeriodEnd = true;
        }
    }

    public void Expire()
    {
        Status = SubscriptionStatus.Expired;
    }
}
