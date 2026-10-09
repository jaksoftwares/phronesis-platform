using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Phronesis.Application.Common.Interfaces;
using Phronesis.Domain.Content;
using System.ComponentModel.DataAnnotations;

namespace Phronesis.Api.Controllers.Administration;

[ApiController]
[Route("api/v1/admin/content")]
[Authorize(Roles = "Admin")]
public class AdminContentController : ControllerBase
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUserService _currentUserService;

    public AdminContentController(IApplicationDbContext context, ICurrentUserService currentUserService)
    {
        _context = context;
        _currentUserService = currentUserService;
    }

    [HttpGet("pending")]
    public async Task<IActionResult> GetPendingContent()
    {
        var content = await _context.EducationalContents
            .Include(c => c.Author)
            .Include(c => c.Subject)
            .Include(c => c.GradeLevel)
            .Where(c => c.Status == ContentStatus.InReview)
            .OrderBy(c => c.CreatedAt)
            .Select(c => new
            {
                c.Id,
                c.Title,
                c.Description,
                c.ContentType,
                c.CreatedAt,
                Author = new { c.Author.FirstName, c.Author.LastName, c.Author.Email },
                Subject = c.Subject.Name,
                GradeLevel = c.GradeLevel.Name
            })
            .ToListAsync();

        return Ok(new { data = content });
    }
    
    [HttpGet("catalog")]
    public async Task<IActionResult> GetCatalog()
    {
        var content = await _context.EducationalContents
            .Include(c => c.Author)
            .Include(c => c.Subject)
            .Include(c => c.GradeLevel)
            .OrderByDescending(c => c.CreatedAt)
            .Select(c => new
            {
                c.Id,
                c.Title,
                c.Status,
                c.ContentType,
                c.CreatedAt,
                Author = new { c.Author.FirstName, c.Author.LastName },
                Subject = c.Subject.Name,
                GradeLevel = c.GradeLevel.Name
            })
            .ToListAsync();

        return Ok(new { data = content });
    }

    public class ReviewContentRequest
    {
        [Required] public string Feedback { get; set; } = string.Empty;
    }

    [HttpPost("{id}/approve")]
    public async Task<IActionResult> ApproveContent(Guid id, [FromBody] ReviewContentRequest req)
    {
        var content = await _context.EducationalContents.FindAsync(id);
        if (content == null) return NotFound(new { message = "Content not found" });

        var adminId = Guid.Parse(_currentUserService.UserId!);
        content.AddReview(adminId, ReviewOutcome.Approved, req.Feedback);

        await _context.SaveChangesAsync(default);
        return Ok(new { message = "Content approved and published." });
    }

    [HttpPost("{id}/reject")]
    public async Task<IActionResult> RejectContent(Guid id, [FromBody] ReviewContentRequest req)
    {
        var content = await _context.EducationalContents.FindAsync(id);
        if (content == null) return NotFound(new { message = "Content not found" });

        var adminId = Guid.Parse(_currentUserService.UserId!);
        content.AddReview(adminId, ReviewOutcome.Rejected, req.Feedback);

        await _context.SaveChangesAsync(default);
        return Ok(new { message = "Content rejected." });
    }

    [HttpPost("{id}/archive")]
    public async Task<IActionResult> ArchiveContent(Guid id)
    {
        var content = await _context.EducationalContents.FindAsync(id);
        if (content == null) return NotFound(new { message = "Content not found" });

        content.Archive();
        
        await _context.SaveChangesAsync(default);
        return Ok(new { message = "Content archived." });
    }
}
