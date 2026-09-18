using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Phronesis.Application.Common.Interfaces;
using Phronesis.Shared.Responses;

namespace Phronesis.Api.Controllers.V1;

[ApiController]
[Route("api/v1/reporting")]
[Authorize]
public class ReportingController : ControllerBase
{
    private readonly IReportingService _reportingService;

    public ReportingController(IReportingService reportingService)
    {
        _reportingService = reportingService;
    }

    [HttpGet("admin/dashboard")]
    [Authorize(Policy = "RequireAdmin")]
    public async Task<IActionResult> GetAdminDashboard(CancellationToken cancellationToken)
    {
        try
        {
            var stats = await _reportingService.GetAdminStatsAsync(cancellationToken);
            return Ok(ApiResponse<object>.Ok(stats));
        }
        catch (Exception ex)
        {
            return BadRequest(new { Message = ex.Message });
        }
    }

    [HttpGet("teacher/dashboard")]
    [Authorize(Policy = "RequireTeacher")]
    public async Task<IActionResult> GetTeacherDashboard(CancellationToken cancellationToken)
    {
        try
        {
            var teacherId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(teacherId)) return Unauthorized();

            var stats = await _reportingService.GetTeacherStatsAsync(teacherId, cancellationToken);
            return Ok(ApiResponse<object>.Ok(stats));
        }
        catch (Exception ex)
        {
            return BadRequest(new { Message = ex.Message });
        }
    }

    [HttpGet("learner/dashboard")]
    // [Authorize(Policy = "RequireLearner")] // Can specify if strict role needed
    public async Task<IActionResult> GetLearnerDashboard(CancellationToken cancellationToken)
    {
        try
        {
            var learnerId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(learnerId)) return Unauthorized();

            var stats = await _reportingService.GetLearnerStatsAsync(learnerId, cancellationToken);
            return Ok(ApiResponse<object>.Ok(stats));
        }
        catch (Exception ex)
        {
            return BadRequest(new { Message = ex.Message });
        }
    }
}
