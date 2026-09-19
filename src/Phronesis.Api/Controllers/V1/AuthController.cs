using Microsoft.AspNetCore.Mvc;
using Phronesis.Application.Authentication;
using Phronesis.Application.Authentication.DTOs;
using Phronesis.Shared.Responses;

namespace Phronesis.Api.Controllers.V1;

[ApiController]
[Route("api/v1/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;

    public AuthController(IAuthService authService)
    {
        _authService = authService;
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterRequest request, CancellationToken cancellationToken)
    {
        await _authService.RegisterAsync(request, cancellationToken);
        return Ok(ApiResponse.Ok("User registered successfully."));
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginRequest request, CancellationToken cancellationToken)
    {
        var deviceInfo = Request.Headers["User-Agent"].ToString();
        var ipAddress = HttpContext.Connection.RemoteIpAddress?.ToString();

        var response = await _authService.LoginAsync(request, deviceInfo, ipAddress, cancellationToken);

        SetRefreshTokenCookie(response.RefreshToken);

        return Ok(ApiResponse<AuthResponse>.Ok(new AuthResponse 
        { 
            AccessToken = response.AccessToken, 
            MustChangePassword = response.MustChangePassword 
        }, "Login successful."));
    }

    [HttpPost("refresh")]
    public async Task<IActionResult> Refresh(CancellationToken cancellationToken)
    {
        var refreshToken = Request.Cookies["refreshToken"];
        if (string.IsNullOrEmpty(refreshToken))
        {
            return Unauthorized(ApiResponse.Failure("Refresh token is missing."));
        }

        var deviceInfo = Request.Headers["User-Agent"].ToString();
        var ipAddress = HttpContext.Connection.RemoteIpAddress?.ToString();

        var response = await _authService.RefreshTokenAsync(refreshToken, deviceInfo, ipAddress, cancellationToken);

        SetRefreshTokenCookie(response.RefreshToken);

        return Ok(ApiResponse<AuthResponse>.Ok(new AuthResponse 
        { 
            AccessToken = response.AccessToken, 
            MustChangePassword = response.MustChangePassword 
        }, "Token refreshed."));
    }
    
    [HttpPost("logout")]
    public async Task<IActionResult> Logout(CancellationToken cancellationToken)
    {
        var refreshToken = Request.Cookies["refreshToken"];
        if (!string.IsNullOrEmpty(refreshToken))
        {
            await _authService.RevokeTokenAsync(refreshToken, cancellationToken);
        }

        Response.Cookies.Delete("refreshToken");
        return Ok(ApiResponse.Ok("Logged out successfully."));
    }

    private void SetRefreshTokenCookie(string token)
    {
        var cookieOptions = new CookieOptions
        {
            HttpOnly = true,
            Secure = true, // Should be true in production, using HTTPS
            SameSite = SameSiteMode.Strict,
            Expires = DateTime.UtcNow.AddDays(7)
        };
        Response.Cookies.Append("refreshToken", token, cookieOptions);
    }

    [HttpPost("verify-email")]
    public async Task<IActionResult> VerifyEmail([FromBody] VerifyEmailRequest request, CancellationToken cancellationToken)
    {
        var role = await _authService.VerifyEmailAsync(request, cancellationToken);
        return Ok(ApiResponse<object>.Ok(new { Role = role }, "Email verified successfully."));
    }

    [HttpPost("resend-email-verification")]
    public async Task<IActionResult> ResendEmailVerification([FromBody] ResendEmailVerificationRequest request, CancellationToken cancellationToken)
    {
        await _authService.ResendEmailVerificationAsync(request, cancellationToken);
        return Ok(ApiResponse.Ok("If the email is valid, a verification link has been sent."));
    }

    [HttpPost("request-password-reset")]
    public async Task<IActionResult> RequestPasswordReset([FromBody] RequestPasswordResetRequest request, CancellationToken cancellationToken)
    {
        await _authService.RequestPasswordResetAsync(request, cancellationToken);
        return Ok(ApiResponse.Ok("If the email is valid, a password reset link has been sent."));
    }

    [HttpPost("reset-password")]
    public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordRequest request, CancellationToken cancellationToken)
    {
        var role = await _authService.ResetPasswordAsync(request, cancellationToken);
        return Ok(ApiResponse<object>.Ok(new { Role = role }, "Password reset successfully."));
    }

    [HttpPost("change-password")]
    public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordRequest request, CancellationToken cancellationToken)
    {
        await _authService.ChangePasswordAsync(request, cancellationToken);
        return Ok(ApiResponse.Ok("Password changed successfully."));
    }

    [HttpGet("me")]
    [Microsoft.AspNetCore.Authorization.Authorize]
    public async Task<IActionResult> Me(CancellationToken cancellationToken)
    {
        var email = User.FindFirst(System.Security.Claims.ClaimTypes.Email)?.Value;
        if (string.IsNullOrEmpty(email)) return Unauthorized();

        var profile = await _authService.GetMeAsync(email, cancellationToken);
        if (profile == null) return NotFound();

        return Ok(ApiResponse<object>.Ok(profile, "Profile retrieved successfully."));
    }

    public record TwoFactorCodeRequest(string Code);

    [HttpPost("2fa/enable")]
    [Microsoft.AspNetCore.Authorization.Authorize]
    public async Task<IActionResult> EnableTwoFactor(CancellationToken cancellationToken)
    {
        var userIdString = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
        if (!Guid.TryParse(userIdString, out Guid userId)) return Unauthorized();

        var secret = await _authService.GenerateTwoFactorSecretAsync(userId, cancellationToken);
        return Ok(ApiResponse<object>.Ok(new { Secret = secret }, "2FA enabled. Store this secret safely."));
    }

    [HttpPost("2fa/verify")]
    [Microsoft.AspNetCore.Authorization.Authorize]
    public async Task<IActionResult> VerifyTwoFactor([FromBody] TwoFactorCodeRequest request, CancellationToken cancellationToken)
    {
        var userIdString = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
        if (!Guid.TryParse(userIdString, out Guid userId)) return Unauthorized();

        var isValid = await _authService.VerifyTwoFactorAsync(userId, request.Code, cancellationToken);
        if (!isValid) return BadRequest(ApiResponse.Failure("Invalid 2FA code."));

        return Ok(ApiResponse.Ok("2FA verified successfully."));
    }

    [HttpGet("sessions")]
    public IActionResult Sessions() => StatusCode(501);

    [HttpDelete("sessions/{sessionId}")]
    public IActionResult RevokeSession(string sessionId) => StatusCode(501);

    [HttpPost("sessions/revoke-all")]
    public IActionResult RevokeAllSessions() => StatusCode(501);

    [HttpPost("phone/send-verification")]
    public IActionResult SendPhoneVerification() => StatusCode(501);

    [HttpPost("phone/verify")]
    public IActionResult VerifyPhone() => StatusCode(501);
}
