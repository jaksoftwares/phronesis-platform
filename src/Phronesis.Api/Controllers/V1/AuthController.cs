using Microsoft.AspNetCore.Mvc;

namespace Phronesis.Api.Controllers.V1;

[ApiController]
[Route("api/v1/[controller]")]
public class AuthController : ControllerBase
{
    [HttpPost("register")]
    public IActionResult Register() => StatusCode(501);

    [HttpPost("login")]
    public IActionResult Login() => StatusCode(501);

    [HttpPost("logout")]
    public IActionResult Logout() => StatusCode(501);

    [HttpPost("refresh")]
    public IActionResult Refresh() => StatusCode(501);

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
