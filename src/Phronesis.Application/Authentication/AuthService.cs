using Microsoft.EntityFrameworkCore;
using Phronesis.Application.Authentication.DTOs;
using Phronesis.Application.Common.Interfaces;
using Phronesis.Domain.Common.Exceptions;
using Phronesis.Domain.Identity;

namespace Phronesis.Application.Authentication;

public class AuthService : IAuthService
{
    private readonly IApplicationDbContext _context;
    private readonly IPasswordHasher _passwordHasher;
    private readonly IJwtProvider _jwtProvider;

    public AuthService(IApplicationDbContext context, IPasswordHasher passwordHasher, IJwtProvider jwtProvider)
    {
        _context = context;
        _passwordHasher = passwordHasher;
        _jwtProvider = jwtProvider;
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
        
        // In a real scenario, we might assign a default 'Learner' role here.

        _context.Users.Add(user);
        await _context.SaveChangesAsync(cancellationToken);

        // We can optionally auto-login the user or return empty.
        // Returning empty for now so they must explicitly log in.
        return new AuthResponse();
    }

    public async Task<AuthResponse> LoginAsync(LoginRequest request, string? deviceInfo, string? ipAddress, CancellationToken cancellationToken = default)
    {
        var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == request.Email, cancellationToken);
        if (user == null || !_passwordHasher.Verify(request.Password, user.PasswordHash))
        {
            throw new DomainException("Invalid email or password.");
        }

        if (!user.IsActive)
        {
            throw new DomainException("User account is inactive.");
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
}
