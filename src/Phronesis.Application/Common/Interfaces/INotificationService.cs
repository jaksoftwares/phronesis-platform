using Phronesis.Domain.Communication;

namespace Phronesis.Application.Common.Interfaces;

public interface INotificationService
{
    Task<Notification> SendNotificationAsync(Guid userId, string title, string message, string type, Guid? referenceId = null, string? referenceType = null, CancellationToken cancellationToken = default);
    Task<IEnumerable<Notification>> GetUserNotificationsAsync(Guid userId, bool unreadOnly = false, CancellationToken cancellationToken = default);
    Task MarkAsReadAsync(Guid notificationId, Guid userId, CancellationToken cancellationToken = default);
    Task MarkAllAsReadAsync(Guid userId, CancellationToken cancellationToken = default);
    
    Task<IEnumerable<NotificationPreference>> GetUserPreferencesAsync(Guid userId, CancellationToken cancellationToken = default);
    Task UpdatePreferenceAsync(Guid userId, string notificationType, bool emailEnabled, bool inAppEnabled, CancellationToken cancellationToken = default);
}
