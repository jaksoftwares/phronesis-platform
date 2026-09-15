using Phronesis.Domain.Identity;

namespace Phronesis.Application.Authentication;

public interface IJwtProvider
{
    string GenerateAccessToken(User user, IEnumerable<string> roles);
    string GenerateRefreshToken();
}
