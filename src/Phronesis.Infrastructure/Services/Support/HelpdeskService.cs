using Microsoft.EntityFrameworkCore;
using Phronesis.Application.Common.Interfaces;
using Phronesis.Domain.Support;

namespace Phronesis.Infrastructure.Services.Support;

public class HelpdeskService : IHelpdeskService
{
    private readonly IApplicationDbContext _context;

    public HelpdeskService(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<SupportTicket> CreateTicketAsync(Guid userId, string category, string subject, string description, string priority, CancellationToken cancellationToken = default)
    {
        var ticket = new SupportTicket(userId, category, subject, description, priority);
        _context.SupportTickets.Add(ticket);
        await _context.SaveChangesAsync(cancellationToken);

        return ticket;
    }

    public async Task<IEnumerable<SupportTicket>> GetUserTicketsAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        return await _context.SupportTickets
            .AsNoTracking()
            .Where(t => t.UserId == userId)
            .OrderByDescending(t => t.Id)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<SupportTicket>> GetAllOpenTicketsAsync(CancellationToken cancellationToken = default)
    {
        return await _context.SupportTickets
            .AsNoTracking()
            .Where(t => t.Status == "Open" || t.Status == "InProgress")
            .OrderByDescending(t => t.Id)
            .ToListAsync(cancellationToken);
    }

    public async Task<SupportTicket?> GetTicketByIdAsync(Guid ticketId, Guid? userId = null, CancellationToken cancellationToken = default)
    {
        var query = _context.SupportTickets
            .AsNoTracking()
            .Include(t => t.Messages)
                .ThenInclude(m => m.Sender)
            .Include(t => t.AssignedAgent)
            .Where(t => t.Id == ticketId);

        if (userId.HasValue)
        {
            // Simple check: Only allow if the user is the creator (in a real system, you'd also allow Support agents)
            // For now, we enforce this restriction if a userId is provided.
            query = query.Where(t => t.UserId == userId.Value);
        }

        return await query.FirstOrDefaultAsync(cancellationToken);
    }

    public async Task<TicketMessage> AddMessageAsync(Guid ticketId, Guid senderId, string content, CancellationToken cancellationToken = default)
    {
        var ticket = await _context.SupportTickets.FindAsync(new object[] { ticketId }, cancellationToken);
        if (ticket == null)
            throw new ArgumentException("Ticket not found.");

        var message = new TicketMessage(ticketId, senderId, content);
        ticket.AddMessage(message);
        
        await _context.SaveChangesAsync(cancellationToken);
        return message;
    }

    public async Task AssignTicketAsync(Guid ticketId, Guid agentId, CancellationToken cancellationToken = default)
    {
        var ticket = await _context.SupportTickets.FindAsync(new object[] { ticketId }, cancellationToken);
        if (ticket == null)
            throw new ArgumentException("Ticket not found.");

        ticket.AssignAgent(agentId);
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task UpdateTicketStatusAsync(Guid ticketId, string status, CancellationToken cancellationToken = default)
    {
        var ticket = await _context.SupportTickets.FindAsync(new object[] { ticketId }, cancellationToken);
        if (ticket == null)
            throw new ArgumentException("Ticket not found.");

        ticket.UpdateStatus(status);
        await _context.SaveChangesAsync(cancellationToken);
    }
}
