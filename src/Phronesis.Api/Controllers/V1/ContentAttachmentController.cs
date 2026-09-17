using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Phronesis.Application.Common.Interfaces;
using Phronesis.Domain.Content;
using Phronesis.Shared.Responses;

namespace Phronesis.Api.Controllers.V1;

[ApiController]
[Route("api/v1/content")]
public class ContentAttachmentController : ControllerBase
{
    private readonly IApplicationDbContext _context;
    private readonly IWebHostEnvironment _env;

    public ContentAttachmentController(IApplicationDbContext context, IWebHostEnvironment env)
    {
        _context = context;
        _env = env;
    }

    [HttpPost("{contentId}/attachments")]
    public async Task<IActionResult> UploadAttachment(Guid contentId, IFormFile file, [FromForm] bool isPrimary, CancellationToken cancellationToken)
    {
        var content = await _context.EducationalContents
            .Include(c => c.Attachments)
            .FirstOrDefaultAsync(c => c.Id == contentId, cancellationToken);
            
        if (content == null) return NotFound();

        // Security check
        var authorIdStr = HttpContext.User.FindFirst("sub")?.Value;
        if (!Guid.TryParse(authorIdStr, out var authorId) || content.AuthorId != authorId)
            return Forbid();

        if (file == null || file.Length == 0)
            return BadRequest(ApiResponse.Failure("No file provided."));

        // File saving logic (MVP: local disk)
        var uploadsFolder = Path.Combine(_env.WebRootPath ?? Path.Combine(Directory.GetCurrentDirectory(), "wwwroot"), "uploads");
        Directory.CreateDirectory(uploadsFolder);

        // Sanitize file name
        var trustedFileNameForFileStorage = $"{Guid.NewGuid()}_{Path.GetFileName(file.FileName)}";
        var filePath = Path.Combine(uploadsFolder, trustedFileNameForFileStorage);

        using (var stream = new FileStream(filePath, FileMode.Create))
        {
            await file.CopyToAsync(stream, cancellationToken);
        }

        var fileUri = $"/uploads/{trustedFileNameForFileStorage}";

        try
        {
            var attachment = content.AddAttachment(file.FileName, fileUri, file.ContentType, file.Length, isPrimary);
            await _context.SaveChangesAsync(cancellationToken);
            
            return Ok(ApiResponse<object>.Ok(new { attachment.Id, attachment.FileUri }, "File uploaded successfully."));
        }
        catch (InvalidOperationException ex)
        {
            // Clean up the physical file if DB fails/rejects
            System.IO.File.Delete(filePath);
            return BadRequest(ApiResponse.Failure(ex.Message));
        }
    }

    [HttpDelete("{contentId}/attachments/{attachmentId}")]
    public async Task<IActionResult> RemoveAttachment(Guid contentId, Guid attachmentId, CancellationToken cancellationToken)
    {
        var content = await _context.EducationalContents
            .Include(c => c.Attachments)
            .FirstOrDefaultAsync(c => c.Id == contentId, cancellationToken);
            
        if (content == null) return NotFound();

        var authorIdStr = HttpContext.User.FindFirst("sub")?.Value;
        if (!Guid.TryParse(authorIdStr, out var authorId) || content.AuthorId != authorId)
            return Forbid();

        var attachment = content.Attachments.FirstOrDefault(a => a.Id == attachmentId);
        if (attachment == null) return NotFound();

        try
        {
            content.RemoveAttachment(attachmentId);
            await _context.SaveChangesAsync(cancellationToken);

            // Best effort cleanup of physical file
            var physicalPath = Path.Combine(_env.WebRootPath ?? Path.Combine(Directory.GetCurrentDirectory(), "wwwroot"), attachment.FileUri.TrimStart('/'));
            if (System.IO.File.Exists(physicalPath))
            {
                System.IO.File.Delete(physicalPath);
            }

            return Ok(ApiResponse.Ok("Attachment removed successfully."));
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(ApiResponse.Failure(ex.Message));
        }
    }

    [HttpGet("{contentId}/attachments")]
    public async Task<IActionResult> GetAttachments(Guid contentId, CancellationToken cancellationToken)
    {
        var attachments = await _context.ContentAttachments
            .Where(ca => ca.EducationalContentId == contentId)
            .Select(ca => new 
            {
                ca.Id,
                ca.FileName,
                ca.FileUri,
                ca.MimeType,
                ca.SizeInBytes,
                ca.IsPrimary
            })
            .ToListAsync(cancellationToken);

        return Ok(ApiResponse<object>.Ok(attachments, "Attachments fetched."));
    }
}
