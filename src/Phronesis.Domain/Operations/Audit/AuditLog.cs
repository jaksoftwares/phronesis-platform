using Phronesis.Domain.Common;

namespace Phronesis.Domain.Operations.Audit;

public class AuditLog : BaseEntity
{
    public string? UserId { get; private set; } // Nullable for system-level actions
    public string Action { get; private set; }
    public string EntityName { get; private set; }
    public string EntityId { get; private set; }
    public string? OldValues { get; private set; } // JSON
    public string? NewValues { get; private set; } // JSON
    public string? IpAddress { get; private set; }
    public DateTime Timestamp { get; private set; }

    private AuditLog() { }

    public AuditLog(string? userId, string action, string entityName, string entityId, string? oldValues, string? newValues, string? ipAddress)
    {
        UserId = userId;
        Action = action;
        EntityName = entityName;
        EntityId = entityId;
        OldValues = oldValues;
        NewValues = newValues;
        IpAddress = ipAddress;
        Timestamp = DateTime.UtcNow;
    }
}
