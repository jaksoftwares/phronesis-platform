using Phronesis.Domain.Common;

namespace Phronesis.Domain.Academic;

public class SubStrand : BaseEntity
{
    public Guid StrandId { get; private set; }
    public string Name { get; private set; }
    public int SortOrder { get; private set; }
    public bool IsActive { get; private set; }

    public Strand Strand { get; private set; } = null!;

    private SubStrand() { }

    public SubStrand(Guid strandId, string name, int sortOrder)
    {
        StrandId = strandId;
        Name = name;
        SortOrder = sortOrder;
        IsActive = true;
    }

    public void Deactivate() => IsActive = false;
    public void Activate() => IsActive = true;
}
