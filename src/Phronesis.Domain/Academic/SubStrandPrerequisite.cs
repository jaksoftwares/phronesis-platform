using Phronesis.Domain.Common;

namespace Phronesis.Domain.Academic;

public class SubStrandPrerequisite : BaseEntity
{
    public Guid SubStrandId { get; private set; }
    public Guid PrerequisiteId { get; private set; }

    public SubStrand SubStrand { get; private set; } = null!;
    public SubStrand Prerequisite { get; private set; } = null!;

    private SubStrandPrerequisite() { }

    public SubStrandPrerequisite(Guid subStrandId, Guid prerequisiteId)
    {
        if (subStrandId == prerequisiteId)
            throw new ArgumentException("A substrand cannot be a prerequisite of itself.");

        SubStrandId = subStrandId;
        PrerequisiteId = prerequisiteId;
    }
}
