using Phronesis.Domain.Academic;
using Phronesis.Domain.Common;

namespace Phronesis.Domain.Users;

public class TeacherCompetence : BaseEntity
{
    public Guid TeacherProfileId { get; private set; }
    public Guid SubjectId { get; private set; }
    public Guid GradeLevelId { get; private set; }
    public bool IsVerified { get; private set; }

    public TeacherProfile TeacherProfile { get; private set; } = null!;
    public Subject Subject { get; private set; } = null!;
    public GradeLevel GradeLevel { get; private set; } = null!;

    private TeacherCompetence() { }

    public TeacherCompetence(Guid teacherProfileId, Guid subjectId, Guid gradeLevelId)
    {
        TeacherProfileId = teacherProfileId;
        SubjectId = subjectId;
        GradeLevelId = gradeLevelId;
        IsVerified = true; // For MVP, assigning assumes verification
    }
    
    public void Revoke() => IsVerified = false;
    public void Verify() => IsVerified = true;
}
