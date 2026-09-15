using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Phronesis.Application.Common.Interfaces;
using Phronesis.Domain.Users;
using Phronesis.Shared.Responses;

namespace Phronesis.Api.Controllers.V1;

[ApiController]
[Route("api/v1/admin/teacher-verifications")]
public class TeacherVerificationController : ControllerBase
{
    private readonly IApplicationDbContext _context;

    public TeacherVerificationController(IApplicationDbContext context)
    {
        _context = context;
    }

    [HttpGet("pending")]
    public async Task<IActionResult> GetPendingApplications(CancellationToken cancellationToken)
    {
        var apps = await _context.TeacherApplications
            .Include(a => a.TeacherProfile)
            .ThenInclude(p => p.User)
            .Where(a => a.Status == ApplicationStatus.Submitted || a.Status == ApplicationStatus.UnderReview)
            .Select(a => new
            {
                a.Id,
                a.TeacherProfile.User.FirstName,
                a.TeacherProfile.User.LastName,
                a.Status,
                a.SubmittedAt
            })
            .ToListAsync(cancellationToken);

        return Ok(ApiResponse<object>.Ok(apps, "Fetched pending applications."));
    }

    [HttpPost("{applicationId}/assign")]
    public async Task<IActionResult> AssignReviewer(Guid applicationId, CancellationToken cancellationToken)
    {
        var application = await _context.TeacherApplications
            .FirstOrDefaultAsync(a => a.Id == applicationId, cancellationToken);

        if (application == null) return NotFound();

        // Mock staff user mapping
        var reviewerIdStr = HttpContext.User.FindFirst("sub")?.Value;
        if (!Guid.TryParse(reviewerIdStr, out var reviewerId))
            return Unauthorized();

        application.AssignReviewer(reviewerId);
        await _context.SaveChangesAsync(cancellationToken);

        return Ok(ApiResponse.Ok("Reviewer assigned successfully."));
    }

    [HttpPost("{applicationId}/schedule-interview")]
    public async Task<IActionResult> ScheduleInterview(Guid applicationId, [FromBody] ScheduleInterviewRequest request, CancellationToken cancellationToken)
    {
        var application = await _context.TeacherApplications
            .FirstOrDefaultAsync(a => a.Id == applicationId, cancellationToken);

        if (application == null) return NotFound();

        application.ScheduleInterview(request.InterviewDate, request.InterviewLink);
        await _context.SaveChangesAsync(cancellationToken);

        return Ok(ApiResponse.Ok("Interview scheduled successfully."));
    }

    [HttpPost("{applicationId}/complete-interview")]
    public async Task<IActionResult> CompleteInterview(Guid applicationId, [FromBody] CompleteInterviewRequest request, CancellationToken cancellationToken)
    {
        var application = await _context.TeacherApplications
            .FirstOrDefaultAsync(a => a.Id == applicationId, cancellationToken);

        if (application == null) return NotFound();

        application.CompleteInterview(request.Notes);
        await _context.SaveChangesAsync(cancellationToken);

        return Ok(ApiResponse.Ok("Interview completed and notes logged."));
    }

    [HttpPost("{applicationId}/approve")]
    public async Task<IActionResult> ApproveApplication(Guid applicationId, [FromBody] ApproveApplicationRequest request, CancellationToken cancellationToken)
    {
        var application = await _context.TeacherApplications
            .Include(a => a.TeacherProfile)
            .FirstOrDefaultAsync(a => a.Id == applicationId, cancellationToken);

        if (application == null) return NotFound();

        application.Approve(request.Notes);
        
        // Ensure parent teacher profile state updates
        application.TeacherProfile.SetVerificationState(TeacherVerificationState.Verified);

        await _context.SaveChangesAsync(cancellationToken);

        return Ok(ApiResponse.Ok("Teacher application approved."));
    }

    [HttpPost("{applicationId}/reject")]
    public async Task<IActionResult> RejectApplication(Guid applicationId, [FromBody] RejectApplicationRequest request, CancellationToken cancellationToken)
    {
        var application = await _context.TeacherApplications
            .Include(a => a.TeacherProfile)
            .FirstOrDefaultAsync(a => a.Id == applicationId, cancellationToken);

        if (application == null) return NotFound();

        application.Reject(request.Notes);
        application.TeacherProfile.SetVerificationState(TeacherVerificationState.Rejected);

        await _context.SaveChangesAsync(cancellationToken);

        return Ok(ApiResponse.Ok("Teacher application rejected."));
    }
}

public record ScheduleInterviewRequest(DateTime InterviewDate, string InterviewLink);
public record CompleteInterviewRequest(string Notes);
public record ApproveApplicationRequest(string Notes);
public record RejectApplicationRequest(string Notes);
