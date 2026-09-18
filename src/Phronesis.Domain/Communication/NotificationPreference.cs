using Phronesis.Domain.Common;
using Phronesis.Domain.Identity;

namespace Phronesis.Domain.Communication;

public class NotificationPreference : BaseEntity
{
    public Guid UserId { get; private set; }
    public string NotificationType { get; private set; }
    public bool EmailEnabled { get; private set; }
    public bool InAppEnabled { get; private set; }

    public User User { get; private set; } = null!;

    private NotificationPreference() { }

    public NotificationPreference(Guid userId, string notificationType, bool emailEnabled = true, bool inAppEnabled = true)
    {
        UserId = userId;
        NotificationType = notificationType;
        EmailEnabled = emailEnabled;
        InAppEnabled = inAppEnabled;
    }

    public void UpdatePreferences(bool emailEnabled, bool inAppEnabled)
    {
        EmailEnabled = emailEnabled;
        InAppEnabled = inAppEnabled;
    }
}
