using Phronesis.Application.Authentication.DTOs;

namespace Phronesis.Application.Authentication;

public interface IAuthService
{
    Task<AuthResponse> RegisterAsync(RegisterRequest request, CancellationToken cancellationToken = default);
    Task<AuthResponse> LoginAsync(LoginRequest request, string? deviceInfo, string? ipAddress, CancellationToken cancellationToken = default);
    Task<AuthResponse> RefreshTokenAsync(string refreshToken, string? deviceInfo, string? ipAddress, CancellationToken cancellationToken = default);
    Task RevokeTokenAsync(string refreshToken, CancellationToken cancellationToken = default);
    Task ChangePasswordAsync(ChangePasswordRequest request, CancellationToken cancellationToken = default);

    Task RequestPasswordResetAsync(RequestPasswordResetRequest request, CancellationToken cancellationToken = default);
    Task<string> ResetPasswordAsync(ResetPasswordRequest request, CancellationToken cancellationToken = default);

    Task<string> VerifyEmailAsync(VerifyEmailRequest request, CancellationToken cancellationToken = default);
    Task ResendEmailVerificationAsync(ResendEmailVerificationRequest request, CancellationToken cancellationToken = default);

    // Two-Factor Authentication
    Task<string> GenerateTwoFactorSecretAsync(Guid userId, CancellationToken cancellationToken = default);
    Task<bool> VerifyTwoFactorAsync(Guid userId, string code, CancellationToken cancellationToken = default);
    Task<bool> ValidateTwoFactorCodeAsync(Guid userId, string code, CancellationToken cancellationToken = default);

    Task<object?> GetMeAsync(string email, CancellationToken cancellationToken = default);
}
