using Microsoft.AspNetCore.Mvc;

namespace Phronesis.Api.Controllers.V1;

[ApiController]
[Route("api/v1/[controller]")]
public class SystemController : ControllerBase
{
    [HttpGet("/health")]
    public IActionResult Health() => Ok(new { status = "healthy", timestamp = DateTime.UtcNow });

    [HttpGet("/health/ready")]
    public IActionResult Ready() => Ok(new { status = "ready", timestamp = DateTime.UtcNow });

    [HttpGet("/api/v1/meta")]
    public IActionResult Meta() => Ok(new { name = "Phronesis Platform API", version = "1.0.0" });

    [HttpGet("/api/v1/config/public")]
    public IActionResult PublicConfig() => StatusCode(501);

    [HttpGet("/api/v1/system/status")]
    public IActionResult SystemStatus() => StatusCode(501);

    [HttpGet("/api/v1/jobs/{jobId}")]
    public IActionResult GetJob(string jobId) => StatusCode(501);

    [HttpPost("/api/v1/jobs/{jobId}/cancel")]
    public IActionResult CancelJob(string jobId) => StatusCode(501);
}
