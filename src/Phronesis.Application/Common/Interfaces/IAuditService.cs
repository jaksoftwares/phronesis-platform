using Phronesis.Domain.Operations.Audit;

namespace Phronesis.Application.Common.Interfaces;

public interface IAuditService
{
    Task LogActionAsync(string? userId, string action, string entityName, string entityId, string? oldValues = null, string? newValues = null, string? ipAddress = null, CancellationToken cancellationToken = default);
    
    Task<IEnumerable<AuditLog>> GetLogsAsync(string? entityName = null, string? userId = null, DateTime? startDate = null, DateTime? endDate = null, CancellationToken cancellationToken = default);
}
