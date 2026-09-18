using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Phronesis.Application.Common.Interfaces;
using Phronesis.Shared.Responses;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;

namespace Phronesis.Api.Controllers.V1;

[ApiController]
[Route("api/v1")]
[Authorize]
public class LearningController : ControllerBase
{
    private readonly IApplicationDbContext _context;

    public LearningController(IApplicationDbContext context)
    {
        _context = context;
    }

    [HttpGet("me/dashboard")]
    public async Task<IActionResult> Dashboard(CancellationToken cancellationToken)
    {
        var email = User.FindFirst(ClaimTypes.Email)?.Value;
        if (string.IsNullOrEmpty(email)) return Unauthorized();

        var user = await _context.Users
            .AsNoTracking()
            .FirstOrDefaultAsync(u => u.Email == email, cancellationToken);
            
        if (user == null) return NotFound("User not found.");

        var learnerProfile = await _context.LearnerProfiles
            .AsNoTracking()
            .FirstOrDefaultAsync(lp => lp.UserId == user.Id, cancellationToken);
            
        if (learnerProfile == null) return NotFound("Learner profile not found.");

        // Calculate learning streak from ContentEngagements (Mock logic using real data if available)
        var recentEngagements = await _context.ContentEngagements
            .Where(e => e.LearnerId == user.Id)
            .OrderByDescending(e => e.LastAccessedAt)
            .Take(10)
            .ToListAsync(cancellationToken);

        var activeEnrollments = await _context.LearnerEnrollments
            .CountAsync(e => e.LearnerId == user.Id, cancellationToken);

        var dashboardData = new 
        {
            User = new { user.FirstName, user.LastName },
            Learner = new { learnerProfile.GradeLevelId, learnerProfile.SchoolName },
            LearningStreak = recentEngagements.Any() ? 1 : 0, // Simplified streak logic
            ActiveEnrollmentsCount = activeEnrollments,
            TotalPoints = 120 // Placeholder for gamification
        };

        return Ok(ApiResponse<object>.Ok(dashboardData));
    }

    [HttpGet("me/subjects")]
    public IActionResult MySubjects() => StatusCode(501);

    [HttpGet("me/topics")]
    public IActionResult RelevantTopics() => StatusCode(501);

    [HttpGet("me/learning/continue")]
    public async Task<IActionResult> ContinueLearning(CancellationToken cancellationToken)
    {
        var email = User.FindFirst(ClaimTypes.Email)?.Value;
        if (string.IsNullOrEmpty(email)) return Unauthorized();

        var user = await _context.Users.AsNoTracking().FirstOrDefaultAsync(u => u.Email == email, cancellationToken);
        if (user == null) return NotFound();
        
        var learnerProfile = await _context.LearnerProfiles.AsNoTracking().FirstOrDefaultAsync(lp => lp.UserId == user.Id, cancellationToken);
        if (learnerProfile == null) return NotFound();

        // Get the most recent engagement that is not 100% complete
        var lastEngagement = await _context.ContentEngagements
            .Include(e => e.EducationalContent)
            .Where(e => e.LearnerId == user.Id && !e.IsCompleted)
            .OrderByDescending(e => e.LastAccessedAt)
            .Select(e => new 
            {
                e.Id,
                ContentId = e.EducationalContentId,
                Title = e.EducationalContent.Title,
                ContentType = e.EducationalContent.ContentType.ToString(),
                CompletionPercentage = e.IsCompleted ? 100 : 0,
                LastEngagedAt = e.LastAccessedAt
            })
            .FirstOrDefaultAsync(cancellationToken);

        return Ok(ApiResponse<object>.Ok(lastEngagement));
    }

    [HttpGet("me/bookmarks")]
    public IActionResult Bookmarks() => StatusCode(501);

    [HttpPost("me/bookmarks")]
    public IActionResult BookmarkResource() => StatusCode(501);

    [HttpDelete("me/bookmarks/{resourceId}")]
    public IActionResult RemoveBookmark(string resourceId) => StatusCode(501);

    [HttpGet("me/recent-resources")]
    public async Task<IActionResult> RecentResources(CancellationToken cancellationToken)
    {
        var email = User.FindFirst(ClaimTypes.Email)?.Value;
        if (string.IsNullOrEmpty(email)) return Unauthorized();

        var user = await _context.Users.AsNoTracking().FirstOrDefaultAsync(u => u.Email == email, cancellationToken);
        if (user == null) return NotFound();
        
        var learnerProfile = await _context.LearnerProfiles.AsNoTracking().FirstOrDefaultAsync(lp => lp.UserId == user.Id, cancellationToken);
        if (learnerProfile == null) return NotFound();

        var recentEngagements = await _context.ContentEngagements
            .Include(e => e.EducationalContent)
            .Where(e => e.LearnerId == user.Id)
            .OrderByDescending(e => e.LastAccessedAt)
            .Take(3)
            .Select(e => new 
            {
                e.Id,
                ContentId = e.EducationalContentId,
                Title = e.EducationalContent.Title,
                ContentType = e.EducationalContent.ContentType.ToString(),
                CompletionPercentage = e.IsCompleted ? 100 : 0,
                LastEngagedAt = e.LastAccessedAt
            })
            .ToListAsync(cancellationToken);

        return Ok(ApiResponse<object>.Ok(recentEngagements));
    }

    [HttpPost("resources/{resourceId}/activity")]
    public IActionResult RecordActivity(string resourceId) => StatusCode(501);

    [HttpGet("me/classes/upcoming")]
    public async Task<IActionResult> UpcomingClasses(CancellationToken cancellationToken)
    {
        var email = User.FindFirst(ClaimTypes.Email)?.Value;
        if (string.IsNullOrEmpty(email)) return Unauthorized();

        var user = await _context.Users.AsNoTracking().FirstOrDefaultAsync(u => u.Email == email, cancellationToken);
        if (user == null) return NotFound();

        var learnerProfile = await _context.LearnerProfiles.AsNoTracking().FirstOrDefaultAsync(lp => lp.UserId == user.Id, cancellationToken);
        if (learnerProfile == null) return NotFound();

        // Get enrollments for this learner
        var enrollmentIds = await _context.ClassEnrollments
            .Where(ce => ce.LearnerId == user.Id && ce.Status == Phronesis.Domain.Tuition.EnrollmentStatus.Active)
            .Select(ce => ce.VirtualClassId)
            .ToListAsync(cancellationToken);

        // Fetch upcoming sessions for these classes
        var upcomingSessions = await _context.ClassSessions
            .Include(cs => cs.VirtualClass)
            .Where(cs => enrollmentIds.Contains(cs.VirtualClassId) && cs.StartTime > DateTime.UtcNow)
            .OrderBy(cs => cs.StartTime)
            .Take(5)
            .Select(cs => new 
            {
                cs.Id,
                Title = cs.Title,
                ClassName = cs.VirtualClass.Name,
                StartTime = cs.StartTime,
                EndTime = cs.EndTime,
                JoinUrl = cs.MeetingLink
            })
            .ToListAsync(cancellationToken);

        return Ok(ApiResponse<object>.Ok(upcomingSessions));
    }

    [HttpGet("me/subscriptions")]
    public IActionResult Subscriptions() => StatusCode(501);

    [HttpGet("me/entitlements")]
    public IActionResult EffectiveEntitlements() => StatusCode(501);

    [HttpGet("question-banks")]
    public IActionResult ListBanks() => StatusCode(501);

    [HttpPost("question-banks")]
    public IActionResult CreateBank() => StatusCode(501);

    [HttpGet("question-banks/{bankId}")]
    public IActionResult GetBank(string bankId) => StatusCode(501);

    [HttpPatch("question-banks/{bankId}")]
    public IActionResult UpdateBank(string bankId) => StatusCode(501);

    [HttpGet("question-banks/{bankId}/questions")]
    public IActionResult ListQuestions(string bankId) => StatusCode(501);

    [HttpPost("question-banks/{bankId}/questions")]
    public IActionResult CreateQuestion(string bankId) => StatusCode(501);

    [HttpGet("questions/{questionId}")]
    public IActionResult GetQuestion(string questionId) => StatusCode(501);

    [HttpPatch("questions/{questionId}")]
    public IActionResult UpdateQuestion(string questionId) => StatusCode(501);

    [HttpDelete("questions/{questionId}")]
    public IActionResult ArchiveQuestion(string questionId) => StatusCode(501);

    [HttpGet("assessments")]
    public IActionResult ListAssessments() => StatusCode(501);

    [HttpPost("assessments")]
    public IActionResult CreateAssessment() => StatusCode(501);

    [HttpGet("assessments/{assessmentId}")]
    public IActionResult GetAssessment(string assessmentId) => StatusCode(501);

    [HttpPatch("assessments/{assessmentId}")]
    public IActionResult UpdateAssessment(string assessmentId) => StatusCode(501);

    [HttpPost("assessments/{assessmentId}/publish")]
    public IActionResult PublishAssessment(string assessmentId) => StatusCode(501);

    [HttpPost("assessments/{assessmentId}/attempts")]
    public IActionResult StartAttempt(string assessmentId) => StatusCode(501);

    [HttpGet("attempts/{attemptId}")]
    public IActionResult GetAttempt(string attemptId) => StatusCode(501);

    [HttpPut("attempts/{attemptId}/answers/{questionId}")]
    public IActionResult SaveAnswer(string attemptId, string questionId) => StatusCode(501);

    [HttpPost("attempts/{attemptId}/submit")]
    public IActionResult SubmitAttempt(string attemptId) => StatusCode(501);

    [HttpGet("attempts/{attemptId}/result")]
    public IActionResult GetResult(string attemptId) => StatusCode(501);

    [HttpGet("me/assessment-history")]
    public IActionResult AssessmentHistory() => StatusCode(501);

    [HttpGet("learners/{learnerId}/progress")]
    public IActionResult LearnerProgress(string learnerId) => StatusCode(501);

    [HttpGet("learners/{learnerId}/progress/topics")]
    public IActionResult TopicProgress(string learnerId) => StatusCode(501);

    [HttpGet("learners/{learnerId}/performance")]
    public IActionResult Performance(string learnerId) => StatusCode(501);

    [HttpGet("learners/{learnerId}/activity")]
    public IActionResult ActivityTimeline(string learnerId) => StatusCode(501);

    [HttpGet("me/progress")]
    public IActionResult OwnProgress() => StatusCode(501);

    [HttpGet("me/performance")]
    public IActionResult OwnPerformance() => StatusCode(501);

    [HttpGet("me/activity")]
    public IActionResult OwnActivity() => StatusCode(501);
}
