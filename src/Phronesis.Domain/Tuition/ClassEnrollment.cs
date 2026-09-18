using Phronesis.Domain.Common;
using Phronesis.Domain.Identity;

namespace Phronesis.Domain.Tuition;

public enum EnrollmentStatus
{
    Active,
    Suspended,
    Dropped
}

public class ClassEnrollment : BaseEntity
{
    public Guid LearnerId { get; private set; }
    public Guid VirtualClassId { get; private set; }
    public EnrollmentStatus Status { get; private set; }
    
    // Navigations
    public User Learner { get; private set; } = null!;
    public VirtualClass VirtualClass { get; private set; } = null!;

    private ClassEnrollment() { } // EF Core

    public ClassEnrollment(Guid learnerId, Guid virtualClassId)
    {
        LearnerId = learnerId;
        VirtualClassId = virtualClassId;
        Status = EnrollmentStatus.Active;
    }

    public void Drop()
    {
        Status = EnrollmentStatus.Dropped;
    }
}
