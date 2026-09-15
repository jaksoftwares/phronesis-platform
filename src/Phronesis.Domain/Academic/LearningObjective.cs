using Phronesis.Domain.Common;

namespace Phronesis.Domain.Academic;

public class LearningObjective : BaseEntity
{
    public Guid SubStrandId { get; private set; }
    public string Description { get; private set; }
    public int SortOrder { get; private set; }
    public bool IsActive { get; private set; }

    public SubStrand SubStrand { get; private set; } = null!;

    private LearningObjective() { }

    public LearningObjective(Guid subStrandId, string description, int sortOrder)
    {
        SubStrandId = subStrandId;
        Description = description;
        SortOrder = sortOrder;
        IsActive = true;
    }

    public void Deactivate() => IsActive = false;
    public void Activate() => IsActive = true;
}
