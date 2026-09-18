using Microsoft.EntityFrameworkCore;
using Phronesis.Application.Common.Interfaces;
using Phronesis.Domain.Operations.Audit;

namespace Phronesis.Infrastructure.Services.Operations;

public class AuditService : IAuditService
{
    private readonly IApplicationDbContext _context;

    public AuditService(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task LogActionAsync(string? userId, string action, string entityName, string entityId, string? oldValues = null, string? newValues = null, string? ipAddress = null, CancellationToken cancellationToken = default)
    {
        var log = new AuditLog(userId, action, entityName, entityId, oldValues, newValues, ipAddress);
        _context.AuditLogs.Add(log);
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task<IEnumerable<AuditLog>> GetLogsAsync(string? entityName = null, string? userId = null, DateTime? startDate = null, DateTime? endDate = null, CancellationToken cancellationToken = default)
    {
        var query = _context.AuditLogs.AsNoTracking().AsQueryable();

        if (!string.IsNullOrEmpty(entityName))
            query = query.Where(a => a.EntityName == entityName);

        if (!string.IsNullOrEmpty(userId))
            query = query.Where(a => a.UserId == userId);

        if (startDate.HasValue)
            query = query.Where(a => a.Timestamp >= startDate.Value);

        if (endDate.HasValue)
            query = query.Where(a => a.Timestamp <= endDate.Value);

        return await query.OrderByDescending(a => a.Timestamp).ToListAsync(cancellationToken);
    }
}
