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

        return Ok(ApiResponse<AuthResponse>.Ok(new AuthResponse { AccessToken = response.AccessToken }, "Login successful."));
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

        return Ok(ApiResponse<AuthResponse>.Ok(new AuthResponse { AccessToken = response.AccessToken }, "Token refreshed."));
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
    public IActionResult VerifyEmail() => StatusCode(501);

    [HttpPost("resend-email-verification")]
    public IActionResult ResendEmailVerification() => StatusCode(501);

    [HttpPost("request-password-reset")]
    public IActionResult RequestPasswordReset() => StatusCode(501);

    [HttpPost("reset-password")]
    public IActionResult ResetPassword() => StatusCode(501);

    [HttpPost("change-password")]
    public IActionResult ChangePassword() => StatusCode(501);

    [HttpGet("me")]
    public IActionResult Me() => StatusCode(501);

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
