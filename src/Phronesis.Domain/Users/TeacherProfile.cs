using Phronesis.Domain.Common;
using Phronesis.Domain.Identity;

namespace Phronesis.Domain.Users;

public class TeacherProfile : BaseEntity
{
    public Guid UserId { get; private set; }
    public string? Bio { get; private set; }
    public string? Qualifications { get; private set; }
    public int ExperienceYears { get; private set; }
    public string? TeachingSkills { get; private set; }
    
    public TeacherVerificationState VerificationState { get; private set; }
    public bool IsActive { get; private set; }

    public User User { get; private set; } = null!;

    private TeacherProfile() { }

    public TeacherProfile(Guid userId)
    {
        UserId = userId;
        VerificationState = TeacherVerificationState.Pending;
        IsActive = true;
    }

    public void UpdateProfile(string? bio, string? qualifications, int experienceYears, string? teachingSkills)
    {
        Bio = bio;
        Qualifications = qualifications;
        ExperienceYears = experienceYears;
        TeachingSkills = teachingSkills;
        
        // If a profile is updated and they were previously rejected, move back to pending.
        // Or if they were verified, it might trigger a re-review (M08).
        if (VerificationState == TeacherVerificationState.Rejected)
        {
            VerificationState = TeacherVerificationState.Pending;
        }
    }

    public void SetVerificationState(TeacherVerificationState state)
    {
        VerificationState = state;
    }

    public void Deactivate() => IsActive = false;
    public void Activate() => IsActive = true;
}
