using Phronesis.Domain.Common;

namespace Phronesis.Domain.Academic;

public class GradeSubject : BaseEntity
{
    public Guid GradeLevelId { get; private set; }
    public Guid SubjectId { get; private set; }
    public bool IsCore { get; private set; }
    public int PeriodsPerWeek { get; private set; }

    public GradeLevel GradeLevel { get; private set; } = null!;
    public Subject Subject { get; private set; } = null!;

    private GradeSubject() { }

    public GradeSubject(Guid gradeLevelId, Guid subjectId, bool isCore, int periodsPerWeek)
    {
        GradeLevelId = gradeLevelId;
        SubjectId = subjectId;
        IsCore = isCore;
        PeriodsPerWeek = periodsPerWeek;
    }

    public void Update(bool isCore, int periodsPerWeek)
    {
        IsCore = isCore;
        PeriodsPerWeek = periodsPerWeek;
    }
}
