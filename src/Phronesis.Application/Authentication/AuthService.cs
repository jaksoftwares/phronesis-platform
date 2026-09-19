using Microsoft.EntityFrameworkCore;
using Phronesis.Application.Authentication.DTOs;
using Phronesis.Application.Common.Interfaces;
using Phronesis.Domain.Common.Exceptions;
using Phronesis.Domain.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace Phronesis.Application.Authentication;

public class AuthService : IAuthService
{
    private readonly IApplicationDbContext _context;
    private readonly IPasswordHasher _passwordHasher;
    private readonly IJwtProvider _jwtProvider;
    private readonly IEmailService _emailService;
    private readonly IConfiguration _configuration;
    private readonly ILogger<AuthService> _logger;

    public AuthService(
        IApplicationDbContext context, 
        IPasswordHasher passwordHasher, 
        IJwtProvider jwtProvider, 
        IEmailService emailService, 
        IConfiguration configuration,
        ILogger<AuthService> logger)
    {
        _context = context;
        _passwordHasher = passwordHasher;
        _jwtProvider = jwtProvider;
        _emailService = emailService;
        _configuration = configuration;
        _logger = logger;
    }

    public async Task<AuthResponse> RegisterAsync(RegisterRequest request, CancellationToken cancellationToken = default)
    {
        var existingUser = await _context.Users.FirstOrDefaultAsync(u => u.Email == request.Email, cancellationToken);
        if (existingUser != null)
        {
            throw new DomainException("Email is already registered.");
        }

        var passwordHash = _passwordHasher.Hash(request.Password);
        var user = new User(request.Email, passwordHash, request.FirstName, request.LastName);
        
        var token = Guid.NewGuid().ToString("N");
        user.SetEmailVerificationToken(token, DateTime.UtcNow.AddHours(1));

        _context.Users.Add(user);
        await _context.SaveChangesAsync(cancellationToken);

        var frontendUrl = _configuration["FrontendUrl"] ?? "http://localhost:3000";
        var verificationLink = $"{frontendUrl}/shared/verify-email?token={token}&email={System.Web.HttpUtility.UrlEncode(user.Email)}";
        
        _logger.LogInformation("================================================");
        _logger.LogInformation("DEV ALERT: REGISTRATION EMAIL VERIFICATION LINK");
        _logger.LogInformation("Link: {VerificationLink}", verificationLink);
        _logger.LogInformation("================================================");

        await _emailService.SendEmailVerificationEmailAsync(user.Email, verificationLink, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);

        // We can optionally auto-login the user or return empty.
        // Returning empty for now so they must explicitly log in.
        return new AuthResponse();
    }

    public async Task<AuthResponse> LoginAsync(LoginRequest request, string? deviceInfo, string? ipAddress, CancellationToken cancellationToken = default)
    {
        var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == request.Email, cancellationToken);
        
        if (user == null)
        {
            throw new DomainException("Invalid email or password.");
        }

        if (user.LockoutEnd.HasValue && user.LockoutEnd > DateTime.UtcNow)
        {
            throw new DomainException($"Account locked. Try again after {user.LockoutEnd.Value.ToLocalTime():t}.");
        }

        if (!_passwordHasher.Verify(request.Password, user.PasswordHash))
        {
            user.RecordFailedLogin();
            await _context.SaveChangesAsync(cancellationToken);
            throw new DomainException("Invalid email or password.");
        }

        user.ResetFailedLogin();

        if (!user.IsActive)
        {
            throw new DomainException("User account is inactive.");
        }

        if (!user.EmailConfirmed)
        {
            throw new DomainException("Email address must be verified before you can log in.");
        }

        var roles = Array.Empty<string>(); // Future: Fetch roles from UserRole mapping
        var accessToken = _jwtProvider.GenerateAccessToken(user, roles);
        var refreshToken = _jwtProvider.GenerateRefreshToken();

        var session = new UserSession(user.Id, refreshToken, DateTime.UtcNow.AddDays(7), deviceInfo, ipAddress);
        _context.UserSessions.Add(session);
        await _context.SaveChangesAsync(cancellationToken);

        return new AuthResponse
        {
            AccessToken = accessToken,
            RefreshToken = refreshToken,
            MustChangePassword = user.MustChangePassword
        };
    }

    public async Task<AuthResponse> RefreshTokenAsync(string refreshToken, string? deviceInfo, string? ipAddress, CancellationToken cancellationToken = default)
    {
        var session = await _context.UserSessions
            .Include(s => s.User)
            .FirstOrDefaultAsync(s => s.RefreshToken == refreshToken, cancellationToken);

        if (session == null || !session.IsActive)
        {
            throw new DomainException("Invalid or expired refresh token.");
        }

        // Revoke the old token (Refresh Token Rotation)
        session.Revoke();

        var roles = Array.Empty<string>(); // Future: Fetch roles
        var newAccessToken = _jwtProvider.GenerateAccessToken(session.User, roles);
        var newRefreshToken = _jwtProvider.GenerateRefreshToken();

        var newSession = new UserSession(session.UserId, newRefreshToken, DateTime.UtcNow.AddDays(7), deviceInfo, ipAddress);
        _context.UserSessions.Add(newSession);
        
        await _context.SaveChangesAsync(cancellationToken);

        return new AuthResponse
        {
            AccessToken = newAccessToken,
            RefreshToken = newRefreshToken,
            MustChangePassword = session.User.MustChangePassword
        };
    }

    public async Task RevokeTokenAsync(string refreshToken, CancellationToken cancellationToken = default)
    {
        var session = await _context.UserSessions.FirstOrDefaultAsync(s => s.RefreshToken == refreshToken, cancellationToken);
        if (session != null && session.IsActive)
        {
            session.Revoke();
            await _context.SaveChangesAsync(cancellationToken);
        }
    }

    public async Task ChangePasswordAsync(ChangePasswordRequest request, CancellationToken cancellationToken = default)
    {
        var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == request.Email, cancellationToken);
        
        if (user == null || !_passwordHasher.Verify(request.TemporaryPassword, user.PasswordHash))
        {
            throw new DomainException("Invalid email or password.");
        }

        if (!user.MustChangePassword)
        {
            throw new DomainException("Password change is not required for this user.");
        }

        user.UpdatePasswordHash(_passwordHasher.Hash(request.NewPassword));
        user.PasswordChanged();
        
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task RequestPasswordResetAsync(RequestPasswordResetRequest request, CancellationToken cancellationToken = default)
    {
        var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == request.Email, cancellationToken);
        if (user == null || !user.IsActive)
        {
            // Do not reveal that the user does not exist
            return;
        }

        var token = Guid.NewGuid().ToString("N");
        user.SetPasswordResetToken(token, DateTime.UtcNow.AddHours(1));
        await _context.SaveChangesAsync(cancellationToken);

        var frontendUrl = _configuration["FrontendUrl"] ?? "http://localhost:3000";
        var resetLink = $"{frontendUrl}/shared/reset-password?token={token}";
        
        _logger.LogInformation("================================================");
        _logger.LogInformation("DEV ALERT: PASSWORD RESET LINK GENERATED");
        _logger.LogInformation("Link: {ResetLink}", resetLink);
        _logger.LogInformation("================================================");

        await _emailService.SendPasswordResetEmailAsync(user.Email, resetLink, cancellationToken);
    }

    public async Task<string> ResetPasswordAsync(ResetPasswordRequest request, CancellationToken cancellationToken = default)
    {
        var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == request.Email, cancellationToken);
        if (user == null || !user.IsActive)
        {
            throw new DomainException("Invalid or expired reset token.");
        }

        if (user.PasswordResetToken != request.Token || user.PasswordResetTokenExpiry < DateTime.UtcNow)
        {
            throw new DomainException("Invalid or expired reset token.");
        }

        user.UpdatePasswordHash(_passwordHasher.Hash(request.NewPassword));
        user.ClearPasswordResetToken();
        user.PasswordChanged();

        await _context.SaveChangesAsync(cancellationToken);

        var userRole = await _context.UserRoles
            .Include(ur => ur.Role)
            .FirstOrDefaultAsync(ur => ur.UserId == user.Id, cancellationToken);

        return userRole?.Role?.Name ?? "Learner";
    }

    public async Task<string> VerifyEmailAsync(VerifyEmailRequest request, CancellationToken cancellationToken = default)
    {
        var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == request.Email, cancellationToken);
        if (user == null)
        {
            throw new DomainException("Invalid verification token.");
        }

        if (user.EmailVerificationToken != request.Token || user.EmailVerificationTokenExpiryTime < DateTime.UtcNow)
        {
            throw new DomainException("Invalid or expired verification token.");
        }

        user.ConfirmEmail();
        await _context.SaveChangesAsync(cancellationToken);

        var userRole = await _context.UserRoles
            .Include(ur => ur.Role)
            .FirstOrDefaultAsync(ur => ur.UserId == user.Id, cancellationToken);

        return userRole?.Role?.Name ?? "Learner";
    }

    public async Task ResendEmailVerificationAsync(ResendEmailVerificationRequest request, CancellationToken cancellationToken = default)
    {
        var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == request.Email, cancellationToken);
        if (user == null || user.EmailConfirmed)
        {
            // Do not reveal if user does not exist or already confirmed to prevent probing
            return;
        }

        var token = Guid.NewGuid().ToString("N");
        user.SetEmailVerificationToken(token, DateTime.UtcNow.AddHours(1));
        await _context.SaveChangesAsync(cancellationToken);

        var frontendUrl = _configuration["FrontendUrl"] ?? "http://localhost:3000";
        var verificationLink = $"{frontendUrl}/shared/verify-email?token={token}&email={System.Web.HttpUtility.UrlEncode(user.Email)}";
        
        _logger.LogInformation("================================================");
        _logger.LogInformation("DEV ALERT: EMAIL VERIFICATION LINK GENERATED");
        _logger.LogInformation("Link: {VerificationLink}", verificationLink);
        _logger.LogInformation("================================================");

        await _emailService.SendEmailVerificationEmailAsync(user.Email, verificationLink, cancellationToken);
    }

    public async Task<object?> GetMeAsync(string email, CancellationToken cancellationToken = default)
    {
        var user = await _context.Users
            .AsNoTracking()
            .Select(u => new
            {
                u.Id,
                u.Email,
                u.FirstName,
                u.LastName,
                u.EmailConfirmed,
                u.IsActive,
                u.TwoFactorEnabled,
                Roles = u.UserRoles.Select(ur => ur.Role.Name).ToList()
            })
            .FirstOrDefaultAsync(u => u.Email == email, cancellationToken);
            
        return user;
    }

    public async Task<string> GenerateTwoFactorSecretAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        var user = await _context.Users.FindAsync(new object[] { userId }, cancellationToken);
        if (user == null) throw new DomainException("User not found.");

        // For MVP, we generate a 16-character base32-like string.
        // In a real app, use Otp.NET to generate standard secrets.
        var secret = Guid.NewGuid().ToString("N").Substring(0, 16).ToUpper();
        user.EnableTwoFactor(secret);
        await _context.SaveChangesAsync(cancellationToken);

        return secret;
    }

    public async Task<bool> VerifyTwoFactorAsync(Guid userId, string code, CancellationToken cancellationToken = default)
    {
        return await ValidateTwoFactorCodeAsync(userId, code, cancellationToken);
    }

    public async Task<bool> ValidateTwoFactorCodeAsync(Guid userId, string code, CancellationToken cancellationToken = default)
    {
        var user = await _context.Users.FindAsync(new object[] { userId }, cancellationToken);
        if (user == null || !user.TwoFactorEnabled) return false;

        // For MVP, we just verify the exact secret. A real app uses a TOTP algorithm (e.g. Otp.NET).
        // Since we are mocking TOTP, we will just simulate success if it matches.
        return user.TwoFactorSecret == code;
    }
}
