using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Phronesis.Application.Common.Interfaces;
using Phronesis.Domain.Users;
using System.ComponentModel.DataAnnotations;

namespace Phronesis.Api.Controllers.Teachers;

[ApiController]
[Route("api/v1/admin/teacher-applications")]
[Authorize(Roles = "Admin")]
public class AdminTeacherApplicationsController : ControllerBase
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUserService _currentUserService;

    public AdminTeacherApplicationsController(IApplicationDbContext context, ICurrentUserService currentUserService)
    {
        _context = context;
        _currentUserService = currentUserService;
    }

    [HttpGet]
    public async Task<IActionResult> GetApplications([FromQuery] ApplicationStatus? status)
    {
        var query = _context.TeacherApplications
            .Include(a => a.TeacherProfile)
            .ThenInclude(p => p.User)
            .AsQueryable();

        if (status.HasValue)
        {
            query = query.Where(a => a.Status == status.Value);
        }

        var apps = await query
            .OrderByDescending(a => a.SubmittedAt ?? a.CreatedAt)
            .Select(a => new
            {
                a.Id,
                a.Status,
                a.SubmittedAt,
                a.InterviewDate,
                Teacher = new { a.TeacherProfile.User.FirstName, a.TeacherProfile.User.LastName, a.TeacherProfile.User.Email }
            })
            .ToListAsync();

        return Ok(new { data = apps });
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetApplication(Guid id)
    {
        var app = await _context.TeacherApplications
            .Include(a => a.TeacherProfile).ThenInclude(p => p.User)
            .Include(a => a.Documents)
            .FirstOrDefaultAsync(a => a.Id == id);

        if (app == null) return NotFound();

        var dto = new
        {
            app.Id,
            app.Status,
            app.ReviewerId,
            app.AdminNotes,
            app.InterviewDate,
            app.InterviewLink,
            app.InterviewNotes,
            app.SubmittedAt,
            app.ReviewedAt,
            app.PolicyAccepted,
            TeacherProfile = new
            {
                app.TeacherProfile.UserId,
                app.TeacherProfile.Bio,
                app.TeacherProfile.Qualifications,
                app.TeacherProfile.ExperienceYears,
                app.TeacherProfile.TeachingSkills,
                app.TeacherProfile.VerificationState,
                app.TeacherProfile.IsActive,
                User = new
                {
                    app.TeacherProfile.User.Id,
                    app.TeacherProfile.User.Email,
                    app.TeacherProfile.User.FirstName,
                    app.TeacherProfile.User.LastName
                }
            },
            Documents = app.Documents.Select(d => new
            {
                d.Id,
                DocumentType = d.DocumentType.ToString(),
                d.FileUri,
                d.VerificationStatus,
                d.CreatedAt
            })
        };

        return Ok(new { data = dto });
    }

    [HttpPost("{id}/start-review")]
    public async Task<IActionResult> StartReview(Guid id)
    {
        var app = await _context.TeacherApplications.FindAsync(id);
        if (app == null) return NotFound();

        if (app.Status != ApplicationStatus.Submitted)
            return BadRequest(new { message = "Application is not in Submitted state." });

        app.AssignReviewer(Guid.Parse(_currentUserService.UserId!));

        await _context.SaveChangesAsync(default);
        return Ok(new { message = "Review started." });
    }

    public class ScheduleInterviewRequest
    {
        [Required] public DateTime Date { get; set; }
        public string? MeetingLink { get; set; }
    }

    [HttpPost("{id}/schedule-interview")]
    public async Task<IActionResult> ScheduleInterview(Guid id, [FromBody] ScheduleInterviewRequest req)
    {
        var app = await _context.TeacherApplications.FindAsync(id);
        if (app == null) return NotFound();

        app.ScheduleInterview(req.Date, req.MeetingLink ?? string.Empty);
        await _context.SaveChangesAsync(default);

        // TODO: Send Email Notification to Teacher here
        
        return Ok(new { message = "Interview scheduled." });
    }

    public class AdminActionRequest
    {
        public string? Notes { get; set; }
    }

    [HttpPost("{id}/approve")]
    public async Task<IActionResult> Approve(Guid id, [FromBody] AdminActionRequest req)
    {
        var app = await _context.TeacherApplications
            .Include(a => a.TeacherProfile)
            .FirstOrDefaultAsync(a => a.Id == id);

        if (app == null) return NotFound();

        app.Approve(req.Notes);
        
        // Approve profile
        app.TeacherProfile.SetVerificationState(TeacherVerificationState.Verified);

        await _context.SaveChangesAsync(default);
        return Ok(new { message = "Application approved." });
    }

    [HttpPost("{id}/reject")]
    public async Task<IActionResult> Reject(Guid id, [FromBody] AdminActionRequest req)
    {
        var app = await _context.TeacherApplications
            .Include(a => a.TeacherProfile)
            .FirstOrDefaultAsync(a => a.Id == id);

        if (app == null) return NotFound();

        if (string.IsNullOrWhiteSpace(req.Notes))
            return BadRequest(new { message = "Rejection reason (Notes) is required." });

        app.Reject(req.Notes);
        
        await _context.SaveChangesAsync(default);
        return Ok(new { message = "Application rejected." });
    }
}

