using Phronesis.Domain.Academic;
using Phronesis.Domain.Common;
using Phronesis.Domain.Identity;

namespace Phronesis.Domain.Users;

public class LearnerProfile : BaseEntity
{
    public Guid UserId { get; private set; }
    public Guid GradeLevelId { get; private set; }
    public string RegistrationNumber { get; private set; }
    public DateTime DateOfBirth { get; private set; }
    public string? SchoolName { get; private set; }
    public bool IsActive { get; private set; }

    public User User { get; private set; } = null!;
    public GradeLevel GradeLevel { get; private set; } = null!;

    private readonly List<LearnerGuardian> _guardians = new();
    public IReadOnlyCollection<LearnerGuardian> Guardians => _guardians.AsReadOnly();

    private LearnerProfile() { }

    public LearnerProfile(Guid userId, Guid gradeLevelId, DateTime dateOfBirth, string? schoolName)
    {
        UserId = userId;
        GradeLevelId = gradeLevelId;
        // Generate a random 6-character alphanumeric registration number
        RegistrationNumber = "PHR-L-" + Guid.NewGuid().ToString("N").Substring(0, 6).ToUpper();
        DateOfBirth = dateOfBirth;
        SchoolName = schoolName;
        IsActive = true;
    }

    public void Deactivate() => IsActive = false;
    public void Activate() => IsActive = true;
}
