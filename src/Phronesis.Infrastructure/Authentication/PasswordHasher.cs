using Phronesis.Application.Authentication;

namespace Phronesis.Infrastructure.Authentication;

public class PasswordHasher : IPasswordHasher
{
    public string Hash(string password)
    {
        return BCrypt.Net.BCrypt.EnhancedHashPassword(password, 13);
    }

    public bool Verify(string password, string passwordHash)
    {
        return BCrypt.Net.BCrypt.EnhancedVerify(password, passwordHash);
    }
}
