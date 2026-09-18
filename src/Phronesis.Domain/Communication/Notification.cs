using Phronesis.Domain.Common;
using Phronesis.Domain.Identity;

namespace Phronesis.Domain.Communication;

public class Notification : BaseEntity
{
    public Guid UserId { get; private set; }
    public string Title { get; private set; }
    public string Message { get; private set; }
    public string Type { get; private set; }
    public bool IsRead { get; private set; }
    public Guid? ReferenceId { get; private set; }
    public string? ReferenceType { get; private set; }
    public DateTime CreatedAt { get; private set; }

    public User User { get; private set; } = null!;

    private Notification() { }

    public Notification(Guid userId, string title, string message, string type, Guid? referenceId = null, string? referenceType = null)
    {
        UserId = userId;
        Title = title;
        Message = message;
        Type = type;
        IsRead = false;
        ReferenceId = referenceId;
        ReferenceType = referenceType;
        CreatedAt = DateTime.UtcNow;
    }

    public void MarkAsRead()
    {
        IsRead = true;
    }
}
