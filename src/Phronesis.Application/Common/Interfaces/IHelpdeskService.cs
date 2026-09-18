using Phronesis.Domain.Support;

namespace Phronesis.Application.Common.Interfaces;

public interface IHelpdeskService
{
    Task<SupportTicket> CreateTicketAsync(Guid userId, string category, string subject, string description, string priority, CancellationToken cancellationToken = default);
    Task<IEnumerable<SupportTicket>> GetUserTicketsAsync(Guid userId, CancellationToken cancellationToken = default);
    Task<IEnumerable<SupportTicket>> GetAllOpenTicketsAsync(CancellationToken cancellationToken = default);
    Task<SupportTicket?> GetTicketByIdAsync(Guid ticketId, Guid? userId = null, CancellationToken cancellationToken = default);
    Task<TicketMessage> AddMessageAsync(Guid ticketId, Guid senderId, string content, CancellationToken cancellationToken = default);
    Task AssignTicketAsync(Guid ticketId, Guid agentId, CancellationToken cancellationToken = default);
    Task UpdateTicketStatusAsync(Guid ticketId, string status, CancellationToken cancellationToken = default);
}
