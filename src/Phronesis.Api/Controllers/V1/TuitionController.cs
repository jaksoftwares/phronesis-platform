using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Phronesis.Application.Common.Interfaces;

namespace Phronesis.Api.Controllers.V1;

[ApiController]
[Route("api/v1/[controller]")]
[Authorize]
public class TuitionController : ControllerBase
{
    private readonly ITuitionService _tuitionService;

    public TuitionController(ITuitionService tuitionService)
    {
        _tuitionService = tuitionService;
    }

    [HttpPost("classes")]
    [Authorize(Policy = "RequireAdmin")] // Could also be "RequireTeacher" based on role config
    public async Task<IActionResult> CreateClass([FromBody] CreateClassRequest request, CancellationToken cancellationToken)
    {
        var result = await _tuitionService.CreateClassAsync(request, cancellationToken);
        return CreatedAtAction(nameof(GetClass), new { id = result.Id }, result);
    }

    [HttpGet("classes/{id}")]
    public IActionResult GetClass(Guid id)
    {
        // Placeholder for GET individual class (often done via MediatR queries, simple 200 OK here)
        return Ok(new { Id = id, Message = "Class details will be wired here" });
    }

    [HttpPost("classes/{classId}/enroll")]
    public async Task<IActionResult> EnrollInClass(Guid classId, CancellationToken cancellationToken)
    {
        var userIdString = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
        if (!Guid.TryParse(userIdString, out var learnerId))
            return Unauthorized();

        try
        {
            var success = await _tuitionService.EnrollLearnerAsync(learnerId, classId, cancellationToken);
            return Ok(new { Success = success, Message = "Successfully enrolled in class." });
        }
        catch (UnauthorizedAccessException ex)
        {
            return StatusCode(403, new { Message = ex.Message });
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { Message = ex.Message });
        }
        catch (ArgumentException ex)
        {
            return NotFound(new { Message = ex.Message });
        }
    }

    [HttpGet("schedule")]
    public async Task<IActionResult> GetSchedule([FromQuery] DateTime? startDate, [FromQuery] DateTime? endDate, CancellationToken cancellationToken)
    {
        var userIdString = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
        if (!Guid.TryParse(userIdString, out var userId))
            return Unauthorized();

        var start = startDate ?? DateTime.UtcNow;
        var end = endDate ?? start.AddDays(7);

        // Check if user is a teacher or learner to route the query
        var isTeacher = User.IsInRole("Teacher"); // Assuming standard ClaimTypes.Role integration

        IEnumerable<Phronesis.Domain.Tuition.ClassSession> sessions;
        if (isTeacher)
        {
            sessions = await _tuitionService.GetTeacherScheduleAsync(userId, start, end, cancellationToken);
        }
        else
        {
            sessions = await _tuitionService.GetLearnerScheduleAsync(userId, start, end, cancellationToken);
        }

        var result = sessions.Select(s => new
        {
            s.Id,
            s.Title,
            s.VirtualClassId,
            ClassName = s.VirtualClass.Name,
            s.StartTime,
            s.EndTime,
            Status = s.Status.ToString(),
            s.MeetingLink
        });

        return Ok(result);
    }
}
