using Phronesis.Domain.Common;

namespace Phronesis.Domain.Users;

public enum LinkRequestStatus
{
    Pending = 0,
    Accepted = 1,
    Declined = 2
}

public class LearnerGuardianLinkRequest : BaseEntity
{
    public Guid LearnerProfileId { get; private set; }
    public Guid GuardianProfileId { get; private set; }
    public RelationshipType RelationshipType { get; private set; }
    public LinkRequestStatus Status { get; private set; }
    public DateTime? ResolvedAt { get; private set; }

    public LearnerProfile LearnerProfile { get; private set; } = null!;
    public GuardianProfile GuardianProfile { get; private set; } = null!;

    private LearnerGuardianLinkRequest() { }

    public LearnerGuardianLinkRequest(Guid learnerProfileId, Guid guardianProfileId, RelationshipType relationshipType)
    {
        LearnerProfileId = learnerProfileId;
        GuardianProfileId = guardianProfileId;
        RelationshipType = relationshipType;
        Status = LinkRequestStatus.Pending;
        CreatedAt = DateTime.UtcNow;
    }

    public void Accept()
    {
        Status = LinkRequestStatus.Accepted;
        ResolvedAt = DateTime.UtcNow;
    }

    public void Decline()
    {
        Status = LinkRequestStatus.Declined;
        ResolvedAt = DateTime.UtcNow;
    }
}
