using Phronesis.Domain.Common;

namespace Phronesis.Domain.Academic;

public class GradeLevel : BaseEntity
{
    public string Name { get; private set; }
    public string Description { get; private set; }
    public int SortOrder { get; private set; }
    public bool IsActive { get; private set; }

    private GradeLevel() { }

    public GradeLevel(string name, string description, int sortOrder)
    {
        Name = name;
        Description = description;
        SortOrder = sortOrder;
        IsActive = true;
    }

    public void Deactivate() => IsActive = false;
    public void Activate() => IsActive = true;
}
