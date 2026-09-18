using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Phronesis.Application.Common.Interfaces;
using Phronesis.Shared.Responses;

namespace Phronesis.Api.Controllers.V1;

[ApiController]
[Route("api/v1/audit")]
[Authorize(Policy = "RequireAdmin")] // Only Admins should view compliance logs
public class AuditController : ControllerBase
{
    private readonly IAuditService _auditService;

    public AuditController(IAuditService auditService)
    {
        _auditService = auditService;
    }

    [HttpGet("logs")]
    public async Task<IActionResult> GetLogs(
        [FromQuery] string? entityName, 
        [FromQuery] string? userId, 
        [FromQuery] DateTime? startDate, 
        [FromQuery] DateTime? endDate, 
        CancellationToken cancellationToken)
    {
        try
        {
            var logs = await _auditService.GetLogsAsync(entityName, userId, startDate, endDate, cancellationToken);
            return Ok(ApiResponse<object>.Ok(logs));
        }
        catch (Exception ex)
        {
            return BadRequest(new { Message = ex.Message });
        }
    }
}
