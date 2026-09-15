using Phronesis.Domain.Common;
using Phronesis.Domain.Identity;

namespace Phronesis.Domain.Users;

public class GuardianProfile : BaseEntity
{
    public Guid UserId { get; private set; }
    public string PhoneNumber { get; private set; }
    public bool IsActive { get; private set; }

    public User User { get; private set; } = null!;

    private readonly List<LearnerGuardian> _learners = new();
    public IReadOnlyCollection<LearnerGuardian> Learners => _learners.AsReadOnly();

    private GuardianProfile() { }

    public GuardianProfile(Guid userId, string phoneNumber)
    {
        UserId = userId;
        PhoneNumber = phoneNumber;
        IsActive = true;
    }

    public void Deactivate() => IsActive = false;
    public void Activate() => IsActive = true;
}
