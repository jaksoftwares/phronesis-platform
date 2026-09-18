using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
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

    [HttpPost("classes/{classId}/sessions")]
    [Authorize(Policy = "RequireTeacher")]
    public async Task<IActionResult> CreateSession(Guid classId, [FromBody] CreateSessionDto dto, CancellationToken cancellationToken)
    {
        try
        {
            var session = await _tuitionService.CreateSessionAsync(classId, dto.Title, dto.StartTime, dto.EndTime, cancellationToken);
            return Ok(new { Message = "Session created successfully.", SessionId = session.Id });
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { Message = ex.Message });
        }
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
            HasVideo = !string.IsNullOrEmpty(s.MeetingId)
        });

        return Ok(result);
    }

    [HttpGet("sessions/{sessionId}/join")]
    public async Task<IActionResult> JoinSession(Guid sessionId, [FromServices] Phronesis.Application.Common.Interfaces.IApplicationDbContext context, CancellationToken cancellationToken)
    {
        var userIdString = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
        if (!Guid.TryParse(userIdString, out var userId))
            return Unauthorized();

        var isTeacher = User.IsInRole("Teacher");

        var session = await context.ClassSessions
            .Include(cs => cs.VirtualClass)
                .ThenInclude(vc => vc.Enrollments)
            .FirstOrDefaultAsync(cs => cs.Id == sessionId, cancellationToken);

        if (session == null)
            return NotFound("Session not found.");

        // Verification
        if (isTeacher)
        {
            if (session.VirtualClass.TeacherId != userId)
                return Forbid("You are not the teacher for this class.");
        }
        else
        {
            if (!session.VirtualClass.Enrollments.Any(e => e.LearnerId == userId && e.Status == Phronesis.Domain.Tuition.EnrollmentStatus.Active))
                return Forbid("You are not enrolled in this class.");
        }

        // Time Check (allow joining 15 mins early)
        if (DateTime.UtcNow < session.StartTime.AddMinutes(-15))
        {
            return BadRequest(new { Message = "Too early to join. Please wait until 15 minutes before the session starts." });
        }

        if (isTeacher)
        {
            // Teacher gets the Host URL
            return Ok(new { JoinUrl = session.HostUrl, Password = session.MeetingPassword });
        }
        
        // M24: Auto-Record Join Attendance for Learner
        if (!isTeacher)
        {
            await _tuitionService.RecordJoinAsync(sessionId, userId, cancellationToken);
        }
        
        // Learner gets the standard Join URL
        return Ok(new { JoinUrl = session.MeetingLink, Password = session.MeetingPassword });
    }

    [HttpPost("sessions/{sessionId}/leave")]
    [Authorize]
    public async Task<IActionResult> LeaveSession(Guid sessionId, CancellationToken cancellationToken)
    {
        var userIdString = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
        if (!Guid.TryParse(userIdString, out var userId))
            return Unauthorized();

        var isTeacher = User.IsInRole("Teacher");
        if (!isTeacher)
        {
            await _tuitionService.RecordLeaveAsync(sessionId, userId, cancellationToken);
        }

        return Ok(new { Message = "Successfully recorded leave." });
    }

    [HttpPost("sessions/{sessionId}/feedback")]
    [Authorize(Policy = "RequireLearner")]
    public async Task<IActionResult> SubmitFeedback(Guid sessionId, [FromBody] SubmitFeedbackDto dto, CancellationToken cancellationToken)
    {
        var userIdString = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
        if (!Guid.TryParse(userIdString, out var userId))
            return Unauthorized();

        try
        {
            await _tuitionService.SubmitFeedbackAsync(
                sessionId, userId, dto.TeacherRating, dto.ContentRating, dto.TechnicalQualityRating,
                dto.WhatWentWell, dto.AreasForImprovement, dto.Complaints, cancellationToken);

            return Ok(new { Message = "Feedback submitted successfully." });
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { Message = ex.Message });
        }
    }

    [HttpPatch("sessions/{sessionId}/complete")]
    [Authorize(Policy = "RequireTeacher")]
    public async Task<IActionResult> CompleteSession(Guid sessionId, [FromBody] CompleteSessionDto dto, CancellationToken cancellationToken)
    {
        var userIdString = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
        if (!Guid.TryParse(userIdString, out var userId))
            return Unauthorized();

        try
        {
            await _tuitionService.CompleteSessionAsync(sessionId, userId, dto.RecordingUrl, dto.TeacherNotes, cancellationToken);
            return Ok(new { Message = "Session completed successfully. Post-class emails triggered." });
        }
        catch (ArgumentException ex)
        {
            return Forbid(ex.Message);
        }
    }

    [HttpGet("classes")]
    [AllowAnonymous] // Or basic auth depending on platform rules
    public IActionResult BrowseClasses([FromServices] Phronesis.Application.Common.Interfaces.IApplicationDbContext context)
    {
        // MVP: Expose public active group classes for learners to browse
        var classes = context.VirtualClasses
            .Where(vc => vc.IsActive && vc.ClassType == Phronesis.Domain.Tuition.ClassType.Group)
            .Select(vc => new
            {
                vc.Id,
                vc.Name,
                vc.Description,
                vc.MaxCapacity,
                vc.IsSubscriptionIncluded,
                vc.Price,
                EnrollmentCount = vc.Enrollments.Count
            })
            .ToList();

        return Ok(classes);
    }
}

public class CreateSessionDto
{
    public string Title { get; set; } = string.Empty;
    public DateTime StartTime { get; set; }
    public DateTime EndTime { get; set; }
}

public class CompleteSessionDto
{
    public string? RecordingUrl { get; set; }
    public string? TeacherNotes { get; set; }
}

public class SubmitFeedbackDto
{
    public int TeacherRating { get; set; }
    public int ContentRating { get; set; }
    public int TechnicalQualityRating { get; set; }
    public string? WhatWentWell { get; set; }
    public string? AreasForImprovement { get; set; }
    public string? Complaints { get; set; }
}
