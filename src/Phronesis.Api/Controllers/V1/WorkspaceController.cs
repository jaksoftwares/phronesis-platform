using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Phronesis.Application.Common.Interfaces;
using Phronesis.Domain.Learning;
using Phronesis.Shared.Responses;

namespace Phronesis.Api.Controllers.V1;

[ApiController]
[Route("api/v1/workspace")]
public class WorkspaceController : ControllerBase
{
    private readonly IApplicationDbContext _context;
    private readonly ISubscriptionService _subscriptionService;

    public WorkspaceController(IApplicationDbContext context, ISubscriptionService subscriptionService)
    {
        _context = context;
        _subscriptionService = subscriptionService;
    }

    [HttpPost("enrollments/{subjectId}")]
    public async Task<IActionResult> Enroll(Guid subjectId, CancellationToken cancellationToken)
    {
        var learnerIdStr = HttpContext.User.FindFirst("sub")?.Value;
        if (!Guid.TryParse(learnerIdStr, out var learnerId)) return Unauthorized();

        // Check if already enrolled
        var isEnrolled = await _context.LearnerEnrollments
            .AnyAsync(le => le.LearnerId == learnerId && le.SubjectId == subjectId, cancellationToken);
            
        if (isEnrolled) return BadRequest(ApiResponse.Failure("Already enrolled in this subject."));

        var enrollment = new LearnerEnrollment(learnerId, subjectId);
        _context.LearnerEnrollments.Add(enrollment);
        await _context.SaveChangesAsync(cancellationToken);

        return Ok(ApiResponse.Ok("Enrolled successfully."));
    }

    [HttpPost("engagement/{contentId}")]
    public async Task<IActionResult> RecordEngagement(Guid contentId, [FromBody] EngagementRequest request, CancellationToken cancellationToken)
    {
        var learnerIdStr = HttpContext.User.FindFirst("sub")?.Value;
        if (!Guid.TryParse(learnerIdStr, out var learnerId)) return Unauthorized();

        var engagement = await _context.ContentEngagements
            .FirstOrDefaultAsync(ce => ce.LearnerId == learnerId && ce.EducationalContentId == contentId, cancellationToken);

        if (engagement == null)
        {
            engagement = new ContentEngagement(learnerId, contentId);
            _context.ContentEngagements.Add(engagement);
        }

        engagement.RecordHeartbeat(request.TimeSpentDeltaSeconds, request.IsCompleted);
        await _context.SaveChangesAsync(cancellationToken);

        return Ok(ApiResponse.Ok("Engagement recorded."));
    }

    [HttpGet("dashboard")]
    public async Task<IActionResult> GetDashboard(CancellationToken cancellationToken)
    {
        var learnerIdStr = HttpContext.User.FindFirst("sub")?.Value;
        if (!Guid.TryParse(learnerIdStr, out var learnerId)) return Unauthorized();

        // 1. My Subjects (Enrollments)
        var enrollments = await _context.LearnerEnrollments
            .Include(le => le.Subject)
            .Where(le => le.LearnerId == learnerId)
            .Select(le => new { le.SubjectId, le.Subject.Name, le.EnrolledAt })
            .ToListAsync(cancellationToken);

        // 2. Continue Learning (Recent incomplete engagements)
        var continueLearning = await _context.ContentEngagements
            .Include(ce => ce.EducationalContent)
                .ThenInclude(ec => ec.Attachments.Where(a => a.IsPrimary))
            .Where(ce => ce.LearnerId == learnerId && !ce.IsCompleted)
            .OrderByDescending(ce => ce.LastAccessedAt)
            .Take(3)
            .Select(ce => new 
            {
                ce.EducationalContentId,
                ce.EducationalContent.Title,
                ce.EducationalContent.ContentType,
                PrimaryAttachmentUrl = ce.EducationalContent.Attachments.FirstOrDefault() != null ? ce.EducationalContent.Attachments.First().FileUri : null,
                ce.TimeSpentSeconds,
                ce.LastAccessedAt
            })
            .ToListAsync(cancellationToken);

        // 3. Recent Activity (All recent engagements)
        var recentActivity = await _context.ContentEngagements
            .Include(ce => ce.EducationalContent)
            .Where(ce => ce.LearnerId == learnerId)
            .OrderByDescending(ce => ce.LastAccessedAt)
            .Take(5)
            .Select(ce => new 
            {
                ce.EducationalContentId,
                ce.EducationalContent.Title,
                ce.EducationalContent.ContentType,
                ce.TimeSpentSeconds,
                ce.IsCompleted,
                ce.LastAccessedAt
            })
            .ToListAsync(cancellationToken);

        // 4. Bookmarks (From M14)
        var bookmarks = await _context.SavedContents
            .Include(sc => sc.EducationalContent)
            .Where(sc => sc.UserId == learnerId)
            .OrderByDescending(sc => sc.CreatedAt)
            .Take(3)
            .Select(sc => new 
            {
                sc.EducationalContentId,
                sc.EducationalContent.Title,
                sc.EducationalContent.ContentType
            })
            .ToListAsync(cancellationToken);

        return Ok(ApiResponse<object>.Ok(new
        {
            Enrollments = enrollments,
            ContinueLearning = continueLearning,
            RecentActivity = recentActivity,
            Bookmarks = bookmarks
        }, "Dashboard fetched successfully."));
    }

    [HttpGet("content-access/{contentId}")]
    public async Task<IActionResult> GetContentAccess(Guid contentId, CancellationToken cancellationToken)
    {
        var learnerIdStr = HttpContext.User.FindFirst("sub")?.Value;
        if (!Guid.TryParse(learnerIdStr, out var learnerId)) return Unauthorized();

        var content = await _context.EducationalContents
            .Include(c => c.Attachments)
            .FirstOrDefaultAsync(c => c.Id == contentId, cancellationToken);

        if (content == null || content.Status != Domain.Content.ContentStatus.Published)
            return NotFound(ApiResponse.Failure("Content not found or not published."));

        // SUBSCRIPTION GATING
        if (content.IsPremium)
        {
            bool hasActiveSubscription = await _subscriptionService.HasActivePremiumSubscriptionAsync(learnerId, cancellationToken);
            
            if (!hasActiveSubscription)
            {
                return StatusCode(403, ApiResponse.Failure("Payment Required. This is premium content. Please upgrade your subscription to access this material."));
            }
        }

        // Return secure URLs / payloads
        return Ok(ApiResponse<object>.Ok(new
        {
            content.Id,
            content.Title,
            content.Description,
            content.ContentType,
            content.Version,
            Attachments = content.Attachments.Select(a => new { a.Id, a.FileName, a.FileUri, a.MimeType, a.SizeInBytes, a.IsPrimary })
        }, "Access granted."));
    }
}

public class EngagementRequest
{
    public long TimeSpentDeltaSeconds { get; set; }
    public bool IsCompleted { get; set; }
}
