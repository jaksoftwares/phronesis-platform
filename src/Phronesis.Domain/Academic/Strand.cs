using Phronesis.Domain.Common;

namespace Phronesis.Domain.Academic;

public class Strand : BaseEntity
{
    public Guid SubjectId { get; private set; }
    public string Name { get; private set; }
    public int SortOrder { get; private set; }
    public bool IsActive { get; private set; }

    public Subject Subject { get; private set; } = null!;

    private Strand() { }

    public Strand(Guid subjectId, string name, int sortOrder)
    {
        SubjectId = subjectId;
        Name = name;
        SortOrder = sortOrder;
        IsActive = true;
    }

    public void Deactivate() => IsActive = false;
    public void Activate() => IsActive = true;
}
