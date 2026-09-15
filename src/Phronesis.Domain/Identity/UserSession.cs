using Phronesis.Domain.Common;

namespace Phronesis.Domain.Identity;

public class UserSession : BaseEntity
{
    public Guid UserId { get; private set; }
    public string RefreshToken { get; private set; }
    public DateTime ExpiryDate { get; private set; }
    public bool IsRevoked { get; private set; }
    public string? DeviceInfo { get; private set; }
    public string? IpAddress { get; private set; }

    public User User { get; private set; } = null!;

    private UserSession() { }

    public UserSession(Guid userId, string refreshToken, DateTime expiryDate, string? deviceInfo, string? ipAddress)
    {
        UserId = userId;
        RefreshToken = refreshToken;
        ExpiryDate = expiryDate;
        DeviceInfo = deviceInfo;
        IpAddress = ipAddress;
        IsRevoked = false;
    }

    public void Revoke()
    {
        IsRevoked = true;
    }

    public bool IsExpired => DateTime.UtcNow >= ExpiryDate;
    public bool IsActive => !IsRevoked && !IsExpired;
}
