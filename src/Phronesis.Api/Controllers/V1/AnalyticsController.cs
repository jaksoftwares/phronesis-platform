using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Phronesis.Application.Common.Interfaces;
using Phronesis.Domain.Learning;
using Phronesis.Shared.Responses;

namespace Phronesis.Api.Controllers.V1;

[ApiController]
[Route("api/v1/analytics")]
public class AnalyticsController : ControllerBase
{
    private readonly IApplicationDbContext _context;

    public AnalyticsController(IApplicationDbContext context)
    {
        _context = context;
    }

    [HttpGet("overview")]
    public async Task<IActionResult> GetOverview(CancellationToken cancellationToken)
    {
        var learnerIdStr = HttpContext.User.FindFirst("sub")?.Value;
        if (!Guid.TryParse(learnerIdStr, out var learnerId)) return Unauthorized();

        // High-level analytics
        var totalTimeSpentSeconds = await _context.ContentEngagements
            .Where(ce => ce.LearnerId == learnerId)
            .SumAsync(ce => ce.TimeSpentSeconds, cancellationToken);

        var assessmentsTaken = await _context.AssessmentAttempts
            .Where(aa => aa.LearnerId == learnerId && aa.Status == AttemptStatus.Completed)
            .CountAsync(cancellationToken);

        var averageScore = assessmentsTaken > 0
            ? await _context.AssessmentAttempts
                .Where(aa => aa.LearnerId == learnerId && aa.Status == AttemptStatus.Completed)
                .AverageAsync(aa => aa.ScorePercentage, cancellationToken)
            : 0;

        return Ok(ApiResponse<object>.Ok(new
        {
            TotalTimeSpentSeconds = totalTimeSpentSeconds,
            TotalTimeSpentHours = Math.Round(totalTimeSpentSeconds / 3600.0, 1),
            AssessmentsCompleted = assessmentsTaken,
            AverageScorePercentage = Math.Round(averageScore, 1)
        }, "Overview fetched."));
    }

    [HttpGet("progress/{subjectId}")]
    public async Task<IActionResult> GetSubjectProgress(Guid subjectId, CancellationToken cancellationToken)
    {
        var learnerIdStr = HttpContext.User.FindFirst("sub")?.Value;
        if (!Guid.TryParse(learnerIdStr, out var learnerId)) return Unauthorized();

        var progress = await _context.SubjectProgresses
            .FirstOrDefaultAsync(sp => sp.LearnerId == learnerId && sp.SubjectId == subjectId, cancellationToken);

        if (progress == null)
        {
            // First time requesting, create it at 0%
            progress = new SubjectProgress(learnerId, subjectId);
            _context.SubjectProgresses.Add(progress);
            await _context.SaveChangesAsync(cancellationToken);
        }

        // Auto-recalculate if older than 1 hour (Cache invalidation strategy)
        if (DateTime.UtcNow - progress.LastCalculatedAt > TimeSpan.FromHours(1))
        {
            await RecalculateProgressInternal(learnerId, subjectId, progress, cancellationToken);
        }

        return Ok(ApiResponse<object>.Ok(new
        {
            progress.TotalContentItems,
            progress.CompletedItems,
            progress.ProgressPercentage,
            progress.LastCalculatedAt
        }, "Progress fetched."));
    }

    [HttpPost("progress/{subjectId}/recalculate")]
    public async Task<IActionResult> RecalculateProgress(Guid subjectId, CancellationToken cancellationToken)
    {
        var learnerIdStr = HttpContext.User.FindFirst("sub")?.Value;
        if (!Guid.TryParse(learnerIdStr, out var learnerId)) return Unauthorized();

        var progress = await _context.SubjectProgresses
            .FirstOrDefaultAsync(sp => sp.LearnerId == learnerId && sp.SubjectId == subjectId, cancellationToken);

        if (progress == null)
        {
            progress = new SubjectProgress(learnerId, subjectId);
            _context.SubjectProgresses.Add(progress);
        }

        await RecalculateProgressInternal(learnerId, subjectId, progress, cancellationToken);
        
        return Ok(ApiResponse.Ok("Progress recalculated successfully."));
    }

    [HttpGet("certificates")]
    public async Task<IActionResult> GetCertificates(CancellationToken cancellationToken)
    {
        var learnerIdStr = HttpContext.User.FindFirst("sub")?.Value;
        if (!Guid.TryParse(learnerIdStr, out var learnerId)) return Unauthorized();

        var certificates = await _context.Certificates
            .Include(c => c.Subject)
            .Where(c => c.LearnerId == learnerId)
            .Select(c => new
            {
                c.CertificateCode,
                c.IssuedAt,
                SubjectName = c.Subject.Name
            })
            .ToListAsync(cancellationToken);

        return Ok(ApiResponse<object>.Ok(certificates, "Certificates fetched."));
    }

    [HttpPost("certificates/issue/{subjectId}")]
    public async Task<IActionResult> IssueCertificate(Guid subjectId, CancellationToken cancellationToken)
    {
        var learnerIdStr = HttpContext.User.FindFirst("sub")?.Value;
        if (!Guid.TryParse(learnerIdStr, out var learnerId)) return Unauthorized();

        // 1. Verify Progress is 100%
        var progress = await _context.SubjectProgresses
            .FirstOrDefaultAsync(sp => sp.LearnerId == learnerId && sp.SubjectId == subjectId, cancellationToken);

        if (progress == null || progress.ProgressPercentage < 100)
        {
            return BadRequest(ApiResponse.Failure("Cannot issue certificate. Learner has not achieved 100% progress."));
        }

        // 2. Prevent duplicate issuance
        var alreadyIssued = await _context.Certificates
            .AnyAsync(c => c.LearnerId == learnerId && c.SubjectId == subjectId, cancellationToken);

        if (alreadyIssued)
        {
            return BadRequest(ApiResponse.Failure("Certificate already issued for this subject."));
        }

        // 3. Issue
        var certificate = new Certificate(learnerId, subjectId);
        _context.Certificates.Add(certificate);
        await _context.SaveChangesAsync(cancellationToken);

        return Ok(ApiResponse<object>.Ok(new { certificate.CertificateCode }, "Certificate generated successfully!"));
    }

    private async Task RecalculateProgressInternal(Guid learnerId, Guid subjectId, SubjectProgress progress, CancellationToken cancellationToken)
    {
        // 1. Calculate total curriculum scope for this subject
        var totalEducationalContent = await _context.EducationalContents
            .Where(ec => ec.SubjectId == subjectId && ec.Status == Domain.Content.ContentStatus.Published)
            .CountAsync(cancellationToken);

        // 2. Count what the learner has completed
        var completedEducationalContent = await _context.ContentEngagements
            .Include(ce => ce.EducationalContent)
            .Where(ce => ce.LearnerId == learnerId 
                      && ce.IsCompleted 
                      && ce.EducationalContent.SubjectId == subjectId)
            .CountAsync(cancellationToken);

        // Update the materialized record
        progress.UpdateProgress(totalEducationalContent, completedEducationalContent);
        await _context.SaveChangesAsync(cancellationToken);
    }
}
