using Phronesis.Domain.Academic;
using Phronesis.Domain.Common;
using Phronesis.Domain.Identity;

namespace Phronesis.Domain.Learning;

public class LearnerEnrollment : BaseEntity
{
    public Guid LearnerId { get; private set; }
    public Guid SubjectId { get; private set; }
    public DateTime EnrolledAt { get; private set; }

    public User Learner { get; private set; } = null!;
    public Subject Subject { get; private set; } = null!;

    private LearnerEnrollment() { }

    public LearnerEnrollment(Guid learnerId, Guid subjectId)
    {
        LearnerId = learnerId;
        SubjectId = subjectId;
        EnrolledAt = DateTime.UtcNow;
    }
}
