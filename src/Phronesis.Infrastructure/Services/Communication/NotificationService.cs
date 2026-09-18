using Microsoft.EntityFrameworkCore;
using Phronesis.Application.Common.Interfaces;
using Phronesis.Domain.Communication;
using Phronesis.Domain.Identity;

namespace Phronesis.Infrastructure.Services.Communication;

public class NotificationService : INotificationService
{
    private readonly IApplicationDbContext _context;
    private readonly IEmailService _emailService;

    public NotificationService(IApplicationDbContext context, IEmailService emailService)
    {
        _context = context;
        _emailService = emailService;
    }

    public async Task<Notification> SendNotificationAsync(Guid userId, string title, string message, string type, Guid? referenceId = null, string? referenceType = null, CancellationToken cancellationToken = default)
    {
        // 1. Check Preferences
        var preference = await _context.NotificationPreferences
            .AsNoTracking()
            .FirstOrDefaultAsync(np => np.UserId == userId && np.NotificationType == type, cancellationToken);

        bool inAppEnabled = preference?.InAppEnabled ?? true;
        bool emailEnabled = preference?.EmailEnabled ?? true;

        var notification = new Notification(userId, title, message, type, referenceId, referenceType);

        if (inAppEnabled)
        {
            _context.Notifications.Add(notification);
            await _context.SaveChangesAsync(cancellationToken);
        }

        if (emailEnabled)
        {
            var user = await _context.Users.AsNoTracking().FirstOrDefaultAsync(u => u.Id == userId, cancellationToken);
            if (user != null)
            {
                // Fire and forget, or await. We await it here for simplicity, but in production this should be enqueued.
                await _emailService.SendEmailAsync(user.Email, title, message);
            }
        }

        return notification;
    }

    public async Task<IEnumerable<Notification>> GetUserNotificationsAsync(Guid userId, bool unreadOnly = false, CancellationToken cancellationToken = default)
    {
        var query = _context.Notifications
            .AsNoTracking()
            .Where(n => n.UserId == userId);

        if (unreadOnly)
        {
            query = query.Where(n => !n.IsRead);
        }

        return await query.OrderByDescending(n => n.CreatedAt).ToListAsync(cancellationToken);
    }

    public async Task MarkAsReadAsync(Guid notificationId, Guid userId, CancellationToken cancellationToken = default)
    {
        var notification = await _context.Notifications.FindAsync(new object[] { notificationId }, cancellationToken);
        if (notification == null || notification.UserId != userId)
            throw new ArgumentException("Notification not found.");

        notification.MarkAsRead();
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task MarkAllAsReadAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        var unreadNotifications = await _context.Notifications
            .Where(n => n.UserId == userId && !n.IsRead)
            .ToListAsync(cancellationToken);

        foreach (var notification in unreadNotifications)
        {
            notification.MarkAsRead();
        }

        if (unreadNotifications.Any())
        {
            await _context.SaveChangesAsync(cancellationToken);
        }
    }

    public async Task<IEnumerable<NotificationPreference>> GetUserPreferencesAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        return await _context.NotificationPreferences
            .AsNoTracking()
            .Where(np => np.UserId == userId)
            .ToListAsync(cancellationToken);
    }

    public async Task UpdatePreferenceAsync(Guid userId, string notificationType, bool emailEnabled, bool inAppEnabled, CancellationToken cancellationToken = default)
    {
        var preference = await _context.NotificationPreferences
            .FirstOrDefaultAsync(np => np.UserId == userId && np.NotificationType == notificationType, cancellationToken);

        if (preference == null)
        {
            preference = new NotificationPreference(userId, notificationType, emailEnabled, inAppEnabled);
            _context.NotificationPreferences.Add(preference);
        }
        else
        {
            preference.UpdatePreferences(emailEnabled, inAppEnabled);
        }

        await _context.SaveChangesAsync(cancellationToken);
    }
}
