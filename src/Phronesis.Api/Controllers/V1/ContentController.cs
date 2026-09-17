using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Phronesis.Application.Common.Interfaces;
using Phronesis.Domain.Content;
using Phronesis.Shared.Responses;

namespace Phronesis.Api.Controllers.V1;

[ApiController]
[Route("api/v1/content")]
public class ContentController : ControllerBase
{
    private readonly IApplicationDbContext _context;

    public ContentController(IApplicationDbContext context)
    {
        _context = context;
    }

    [HttpPost]
    public async Task<IActionResult> CreateContent([FromBody] CreateContentRequest request, CancellationToken cancellationToken)
    {
        // Mock sub extraction
        var authorIdStr = HttpContext.User.FindFirst("sub")?.Value;
        if (!Guid.TryParse(authorIdStr, out var authorId))
            return Unauthorized();

        var content = new EducationalContent(
            request.Title,
            request.Description,
            request.Version,
            request.IsPremium,
            request.ContentType,
            authorId,
            request.GradeLevelId,
            request.SubjectId
        );

        content.SetTaxonomy(request.StrandId, request.SubStrandId, request.LearningObjectiveId);

        if (request.Tags != null)
        {
            foreach (var tag in request.Tags)
            {
                content.AddTag(tag);
            }
        }

        _context.EducationalContents.Add(content);
        await _context.SaveChangesAsync(cancellationToken);

        return Ok(ApiResponse<object>.Ok(new { content.Id }, "Content draft created successfully."));
    }

    [HttpPut("{contentId}")]
    public async Task<IActionResult> UpdateContent(Guid contentId, [FromBody] UpdateContentRequest request, CancellationToken cancellationToken)
    {
        var content = await _context.EducationalContents
            .Include(c => c.Tags)
            .FirstOrDefaultAsync(c => c.Id == contentId, cancellationToken);

        if (content == null) return NotFound();
        if (content.Status != ContentStatus.Draft) return BadRequest(ApiResponse.Failure("Only draft content can be modified."));

        // Mock sub extraction for author verification
        var authorIdStr = HttpContext.User.FindFirst("sub")?.Value;
        if (!Guid.TryParse(authorIdStr, out var authorId) || content.AuthorId != authorId)
            return Forbid();

        content.UpdateMetadata(request.Title, request.Description, request.Version, request.IsPremium);
        content.SetTaxonomy(request.StrandId, request.SubStrandId, request.LearningObjectiveId);

        // Simple tag sync
        var existingTags = content.Tags.Select(t => t.Name).ToList();
        var incomingTags = request.Tags ?? new List<string>();

        var tagsToRemove = existingTags.Except(incomingTags, StringComparer.OrdinalIgnoreCase).ToList();
        var tagsToAdd = incomingTags.Except(existingTags, StringComparer.OrdinalIgnoreCase).ToList();

        foreach (var t in tagsToRemove) content.RemoveTag(t);
        foreach (var t in tagsToAdd) content.AddTag(t);

        await _context.SaveChangesAsync(cancellationToken);

        return Ok(ApiResponse.Ok("Content updated successfully."));
    }

    [HttpGet("{contentId}")]
    public async Task<IActionResult> GetContent(Guid contentId, CancellationToken cancellationToken)
    {
        var content = await _context.EducationalContents
            .Include(c => c.Tags)
            .FirstOrDefaultAsync(c => c.Id == contentId, cancellationToken);

        if (content == null) return NotFound();

        return Ok(ApiResponse<object>.Ok(content, "Content fetched successfully."));
    }

    [HttpGet("author/{authorId}")]
    public async Task<IActionResult> GetContentByAuthor(Guid authorId, CancellationToken cancellationToken)
    {
        var contents = await _context.EducationalContents
            .Where(c => c.AuthorId == authorId)
            .Select(c => new
            {
                c.Id,
                c.Title,
                c.Status,
                c.ContentType,
                c.Version
            })
            .ToListAsync(cancellationToken);

        return Ok(ApiResponse<object>.Ok(contents, "Author content fetched."));
    }
}

public record CreateContentRequest(
    string Title, 
    string Description, 
    string Version, 
    bool IsPremium, 
    ContentType ContentType, 
    Guid GradeLevelId, 
    Guid SubjectId, 
    Guid? StrandId, 
    Guid? SubStrandId, 
    Guid? LearningObjectiveId,
    List<string>? Tags
);

public record UpdateContentRequest(
    string Title, 
    string Description, 
    string Version, 
    bool IsPremium, 
    Guid? StrandId, 
    Guid? SubStrandId, 
    Guid? LearningObjectiveId,
    List<string>? Tags
);
