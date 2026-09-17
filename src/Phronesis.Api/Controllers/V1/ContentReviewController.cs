using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Phronesis.Application.Common.Interfaces;
using Phronesis.Domain.Content;
using Phronesis.Shared.Responses;

namespace Phronesis.Api.Controllers.V1;

[ApiController]
[Route("api/v1/content")]
public class ContentReviewController : ControllerBase
{
    private readonly IApplicationDbContext _context;

    public ContentReviewController(IApplicationDbContext context)
    {
        _context = context;
    }

    [HttpPost("{contentId}/submit-for-review")]
    public async Task<IActionResult> SubmitForReview(Guid contentId, CancellationToken cancellationToken)
    {
        var content = await _context.EducationalContents.FirstOrDefaultAsync(c => c.Id == contentId, cancellationToken);
        if (content == null) return NotFound();

        // Security check: Only author can submit their own content
        var userIdStr = HttpContext.User.FindFirst("sub")?.Value;
        if (!Guid.TryParse(userIdStr, out var userId) || content.AuthorId != userId)
            return Forbid();

        try
        {
            content.SubmitForReview();
            await _context.SaveChangesAsync(cancellationToken);
            return Ok(ApiResponse.Ok("Content submitted for review successfully."));
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(ApiResponse.Failure(ex.Message));
        }
    }

    [HttpPost("{contentId}/reviews")]
    public async Task<IActionResult> AddReview(Guid contentId, [FromBody] AddReviewRequest request, CancellationToken cancellationToken)
    {
        var content = await _context.EducationalContents
            .Include(c => c.Reviews)
            .FirstOrDefaultAsync(c => c.Id == contentId, cancellationToken);
            
        if (content == null) return NotFound();

        var reviewerIdStr = HttpContext.User.FindFirst("sub")?.Value;
        if (!Guid.TryParse(reviewerIdStr, out var reviewerId))
            return Unauthorized();

        // Authors cannot review their own content
        if (content.AuthorId == reviewerId)
            return BadRequest(ApiResponse.Failure("Authors cannot review their own content."));

        try
        {
            content.AddReview(reviewerId, request.Outcome, request.Feedback);
            await _context.SaveChangesAsync(cancellationToken);
            
            var msg = request.Outcome == ReviewOutcome.Approved 
                ? "Content approved and published successfully." 
                : "Content review submitted and sent back to author.";
                
            return Ok(ApiResponse.Ok(msg));
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(ApiResponse.Failure(ex.Message));
        }
    }

    [HttpGet("pending-reviews")]
    public async Task<IActionResult> GetPendingReviews(CancellationToken cancellationToken)
    {
        var pending = await _context.EducationalContents
            .Where(c => c.Status == ContentStatus.InReview)
            .Select(c => new
            {
                c.Id,
                c.Title,
                c.AuthorId,
                c.ContentType,
                c.Version
            })
            .ToListAsync(cancellationToken);

        return Ok(ApiResponse<object>.Ok(pending, "Fetched pending reviews successfully."));
    }

    [HttpPost("{contentId}/archive")]
    public async Task<IActionResult> ArchiveContent(Guid contentId, CancellationToken cancellationToken)
    {
        var content = await _context.EducationalContents.FirstOrDefaultAsync(c => c.Id == contentId, cancellationToken);
        if (content == null) return NotFound();

        // In a real app, verify user has admin/publisher rights here

        content.Archive();
        await _context.SaveChangesAsync(cancellationToken);

        return Ok(ApiResponse.Ok("Content archived successfully."));
    }
}

public record AddReviewRequest(ReviewOutcome Outcome, string Feedback);
