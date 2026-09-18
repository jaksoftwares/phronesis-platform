using Phronesis.Domain.Common;
using Phronesis.Domain.Identity;

namespace Phronesis.Domain.Support;

public class SupportTicket : BaseEntity
{
    public Guid UserId { get; private set; }
    public Guid? AssignedAgentId { get; private set; }
    public string Category { get; private set; }
    public string Subject { get; private set; }
    public string Description { get; private set; }
    public string Status { get; private set; }
    public string Priority { get; private set; }

    public User User { get; private set; } = null!;
    public User? AssignedAgent { get; private set; }

    private readonly List<TicketMessage> _messages = new();
    public IReadOnlyCollection<TicketMessage> Messages => _messages.AsReadOnly();

    private SupportTicket() { }

    public SupportTicket(Guid userId, string category, string subject, string description, string priority)
    {
        UserId = userId;
        Category = category;
        Subject = subject;
        Description = description;
        Priority = priority;
        Status = "Open";
    }

    public void AssignAgent(Guid agentId)
    {
        AssignedAgentId = agentId;
        if (Status == "Open")
        {
            Status = "InProgress";
        }
    }

    public void UpdateStatus(string status)
    {
        Status = status;
    }

    public void AddMessage(TicketMessage message)
    {
        _messages.Add(message);
    }
}
