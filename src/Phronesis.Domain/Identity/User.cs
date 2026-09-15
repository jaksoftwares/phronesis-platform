using Phronesis.Domain.Common;

namespace Phronesis.Domain.Identity;

public class User : BaseEntity
{
    public string Email { get; private set; }
    public string PasswordHash { get; private set; }
    public string FirstName { get; private set; }
    public string LastName { get; private set; }
    public bool IsActive { get; private set; }
    public bool MustChangePassword { get; private set; }

    public Phronesis.Domain.Organization.StaffProfile? StaffProfile { get; private set; }

    private readonly List<UserRole> _userRoles = new();
    public IReadOnlyCollection<UserRole> UserRoles => _userRoles.AsReadOnly();

    // Required by EF Core
    private User() { }

    public User(string email, string passwordHash, string firstName, string lastName)
    {
        Email = email;
        PasswordHash = passwordHash;
        FirstName = firstName;
        LastName = lastName;
        IsActive = true;
        MustChangePassword = false;
    }

    public void RequirePasswordChange() => MustChangePassword = true;
    public void PasswordChanged() => MustChangePassword = false;
    public void UpdatePasswordHash(string newHash) => PasswordHash = newHash;

    public void Deactivate()
    {
        IsActive = false;
    }

    public void Activate()
    {
        IsActive = true;
    }
}
