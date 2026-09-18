using Phronesis.Domain.Common;
using Phronesis.Domain.Identity;

namespace Phronesis.Domain.Support;

public class TicketMessage : BaseEntity
{
    public Guid SupportTicketId { get; private set; }
    public Guid SenderId { get; private set; }
    public string Content { get; private set; }
    public DateTime SentAt { get; private set; }

    public SupportTicket SupportTicket { get; private set; } = null!;
    public User Sender { get; private set; } = null!;

    private TicketMessage() { }

    public TicketMessage(Guid supportTicketId, Guid senderId, string content)
    {
        SupportTicketId = supportTicketId;
        SenderId = senderId;
        Content = content;
        SentAt = DateTime.UtcNow;
    }
}
