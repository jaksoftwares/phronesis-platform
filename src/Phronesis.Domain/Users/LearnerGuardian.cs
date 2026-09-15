namespace Phronesis.Domain.Users;

public class LearnerGuardian
{
    public Guid LearnerProfileId { get; private set; }
    public Guid GuardianProfileId { get; private set; }
    public RelationshipType RelationshipType { get; private set; }
    public bool CanViewProgress { get; private set; }
    public bool IsPrimaryPayer { get; private set; }
    public bool IsActive { get; private set; }

    public LearnerProfile LearnerProfile { get; private set; } = null!;
    public GuardianProfile GuardianProfile { get; private set; } = null!;

    private LearnerGuardian() { }

    public LearnerGuardian(Guid learnerProfileId, Guid guardianProfileId, RelationshipType relationshipType, bool canViewProgress, bool isPrimaryPayer)
    {
        LearnerProfileId = learnerProfileId;
        GuardianProfileId = guardianProfileId;
        RelationshipType = relationshipType;
        CanViewProgress = canViewProgress;
        IsPrimaryPayer = isPrimaryPayer;
        IsActive = true;
    }
}
