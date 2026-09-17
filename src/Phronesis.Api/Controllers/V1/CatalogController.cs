using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Phronesis.Application.Common.Interfaces;
using Phronesis.Domain.Content;
using Phronesis.Shared.Responses;

namespace Phronesis.Api.Controllers.V1;

[ApiController]
[Route("api/v1/catalog")]
public class CatalogController : ControllerBase
{
    private readonly IApplicationDbContext _context;

    public CatalogController(IApplicationDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<IActionResult> SearchCatalog(
        [FromQuery] string? searchTerm,
        [FromQuery] Guid? gradeLevelId,
        [FromQuery] Guid? subjectId,
        [FromQuery] Guid? strandId,
        [FromQuery] ContentType? contentType,
        [FromQuery] bool? isPremium,
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 20,
        CancellationToken cancellationToken = default)
    {
        // STRICT RULE: Only published content is visible in the catalog
        var query = _context.EducationalContents
            .Include(c => c.Tags)
            .Include(c => c.Attachments.Where(a => a.IsPrimary)) // Only fetch primary for cards
            .Where(c => c.Status == ContentStatus.Published)
            .AsQueryable();

        // Apply filters
        if (!string.IsNullOrWhiteSpace(searchTerm))
        {
            var term = searchTerm.ToLower();
            query = query.Where(c => 
                c.Title.ToLower().Contains(term) || 
                c.Description.ToLower().Contains(term) ||
                c.Tags.Any(t => t.Name.ToLower().Contains(term)));
        }

        if (gradeLevelId.HasValue)
            query = query.Where(c => c.GradeLevelId == gradeLevelId.Value);

        if (subjectId.HasValue)
            query = query.Where(c => c.SubjectId == subjectId.Value);
            
        if (strandId.HasValue)
            query = query.Where(c => c.StrandId == strandId.Value);

        if (contentType.HasValue)
            query = query.Where(c => c.ContentType == contentType.Value);

        if (isPremium.HasValue)
            query = query.Where(c => c.IsPremium == isPremium.Value);

        // Execute count
        var totalItems = await query.CountAsync(cancellationToken);

        // Execute pagination
        var items = await query
            .OrderByDescending(c => c.UpdatedAt ?? c.CreatedAt) // Freshest first
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .Select(c => new 
            {
                c.Id,
                c.Title,
                c.Description,
                c.ContentType,
                c.IsPremium,
                c.Version,
                AuthorId = c.AuthorId, // In reality we'd join with User to get Author Name
                Tags = c.Tags.Select(t => t.Name),
                PrimaryAttachmentUrl = c.Attachments.FirstOrDefault() != null ? c.Attachments.First().FileUri : null,
                PublishedAt = c.UpdatedAt ?? c.CreatedAt
            })
            .ToListAsync(cancellationToken);

        return Ok(ApiResponse<object>.Ok(new {
            TotalItems = totalItems,
            PageNumber = pageNumber,
            PageSize = pageSize,
            TotalPages = (int)Math.Ceiling(totalItems / (double)pageSize),
            Items = items
        }, "Catalog search successful."));
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetContentDetails(Guid id, CancellationToken cancellationToken)
    {
        var content = await _context.EducationalContents
            .Include(c => c.Tags)
            .Include(c => c.Attachments)
            .Where(c => c.Status == ContentStatus.Published)
            .FirstOrDefaultAsync(c => c.Id == id, cancellationToken);

        if (content == null)
            return NotFound(ApiResponse.Failure("Content not found or not published."));

        return Ok(ApiResponse<object>.Ok(new
        {
            content.Id,
            content.Title,
            content.Description,
            content.ContentType,
            content.Status,
            content.IsPremium,
            content.Version,
            content.AuthorId,
            Tags = content.Tags.Select(t => t.Name),
            Attachments = content.Attachments.Select(a => new { a.Id, a.FileName, a.FileUri, a.MimeType, a.SizeInBytes, a.IsPrimary }),
            PublishedAt = content.UpdatedAt ?? content.CreatedAt
        }));
    }

    [HttpPost("{id}/save")]
    public async Task<IActionResult> SaveContent(Guid id, CancellationToken cancellationToken)
    {
        var userIdStr = HttpContext.User.FindFirst("sub")?.Value;
        if (!Guid.TryParse(userIdStr, out var userId)) return Unauthorized();

        var contentExists = await _context.EducationalContents
            .AnyAsync(c => c.Id == id && c.Status == ContentStatus.Published, cancellationToken);
            
        if (!contentExists) return NotFound(ApiResponse.Failure("Content not found."));

        var alreadySaved = await _context.SavedContents
            .AnyAsync(sc => sc.UserId == userId && sc.EducationalContentId == id, cancellationToken);

        if (alreadySaved) return BadRequest(ApiResponse.Failure("Content is already saved."));

        var savedContent = new SavedContent(userId, id);
        _context.SavedContents.Add(savedContent);
        await _context.SaveChangesAsync(cancellationToken);

        return Ok(ApiResponse.Ok("Content saved successfully."));
    }

    [HttpDelete("{id}/save")]
    public async Task<IActionResult> RemoveSavedContent(Guid id, CancellationToken cancellationToken)
    {
        var userIdStr = HttpContext.User.FindFirst("sub")?.Value;
        if (!Guid.TryParse(userIdStr, out var userId)) return Unauthorized();

        var savedContent = await _context.SavedContents
            .FirstOrDefaultAsync(sc => sc.UserId == userId && sc.EducationalContentId == id, cancellationToken);

        if (savedContent == null) return NotFound();

        _context.SavedContents.Remove(savedContent);
        await _context.SaveChangesAsync(cancellationToken);

        return Ok(ApiResponse.Ok("Saved content removed."));
    }

    [HttpGet("saved")]
    public async Task<IActionResult> GetSavedContent(CancellationToken cancellationToken)
    {
        var userIdStr = HttpContext.User.FindFirst("sub")?.Value;
        if (!Guid.TryParse(userIdStr, out var userId)) return Unauthorized();

        var savedItems = await _context.SavedContents
            .Include(sc => sc.EducationalContent)
                .ThenInclude(ec => ec.Tags)
            .Include(sc => sc.EducationalContent)
                .ThenInclude(ec => ec.Attachments.Where(a => a.IsPrimary))
            .Where(sc => sc.UserId == userId && sc.EducationalContent.Status == ContentStatus.Published)
            .OrderByDescending(sc => sc.CreatedAt)
            .Select(sc => new
            {
                SavedAt = sc.CreatedAt,
                Content = new 
                {
                    sc.EducationalContent.Id,
                    sc.EducationalContent.Title,
                    sc.EducationalContent.ContentType,
                    sc.EducationalContent.IsPremium,
                    Tags = sc.EducationalContent.Tags.Select(t => t.Name),
                    PrimaryAttachmentUrl = sc.EducationalContent.Attachments.FirstOrDefault() != null ? sc.EducationalContent.Attachments.First().FileUri : null
                }
            })
            .ToListAsync(cancellationToken);

        return Ok(ApiResponse<object>.Ok(savedItems, "Saved content fetched."));
    }
}
