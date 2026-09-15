using Phronesis.Domain.Common;

namespace Phronesis.Domain.Academic;

public class EducationLevel : BaseEntity
{
    public Guid CurriculumId { get; private set; }
    public string Name { get; private set; }
    public string Description { get; private set; }
    public int SortOrder { get; private set; }
    public bool IsActive { get; private set; }

    public Curriculum Curriculum { get; private set; } = null!;

    private EducationLevel() { }

    public EducationLevel(Guid curriculumId, string name, string description, int sortOrder)
    {
        CurriculumId = curriculumId;
        Name = name;
        Description = description;
        SortOrder = sortOrder;
        IsActive = true;
    }

    public void Deactivate() => IsActive = false;
    public void Activate() => IsActive = true;
}
