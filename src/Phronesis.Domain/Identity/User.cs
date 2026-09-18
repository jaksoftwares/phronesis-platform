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

    // Security & Lockout
    public int FailedLoginAttempts { get; private set; }
    public DateTime? LockoutEnd { get; private set; }

    // Password Recovery
    public string? PasswordResetToken { get; private set; }
    public DateTime? PasswordResetTokenExpiry { get; private set; }

    // Two-Factor Authentication
    public bool TwoFactorEnabled { get; private set; }
    public string? TwoFactorSecret { get; private set; }

    public bool EmailConfirmed { get; private set; }
    public string? EmailVerificationToken { get; private set; }
    public DateTime? EmailVerificationTokenExpiryTime { get; private set; }

    public Phronesis.Domain.Organization.StaffProfile? StaffProfile { get; private set; }

    public ICollection<UserRole> UserRoles { get; private set; } = new List<UserRole>();
    public ICollection<UserSession> Sessions { get; private set; } = new List<UserSession>();

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
        EmailConfirmed = false;
    }

    public void RequirePasswordChange() => MustChangePassword = true;
    public void PasswordChanged() => MustChangePassword = false;

    public void UpdatePasswordHash(string newHash) => PasswordHash = newHash;

    public void SetEmailVerificationToken(string token, DateTime expiryTime)
    {
        EmailVerificationToken = token;
        EmailVerificationTokenExpiryTime = expiryTime;
    }

    public void ConfirmEmail()
    {
        EmailConfirmed = true;
        EmailVerificationToken = null;
        EmailVerificationTokenExpiryTime = null;
    }

    public void Activate()
    {
        IsActive = true;
    }

    public void Deactivate()
    {
        IsActive = false;
    }

    public void RecordFailedLogin()
    {
        FailedLoginAttempts++;
        if (FailedLoginAttempts >= 5)
        {
            LockoutEnd = DateTime.UtcNow.AddMinutes(15);
        }
    }

    public void ResetFailedLogin()
    {
        FailedLoginAttempts = 0;
        LockoutEnd = null;
    }

    public void SetPasswordResetToken(string token, DateTime expiry)
    {
        PasswordResetToken = token;
        PasswordResetTokenExpiry = expiry;
    }

    public void ClearPasswordResetToken()
    {
        PasswordResetToken = null;
        PasswordResetTokenExpiry = null;
    }

    public void EnableTwoFactor(string secret)
    {
        TwoFactorEnabled = true;
        TwoFactorSecret = secret;
    }

    public void DisableTwoFactor()
    {
        TwoFactorEnabled = false;
        TwoFactorSecret = null;
    }
}
