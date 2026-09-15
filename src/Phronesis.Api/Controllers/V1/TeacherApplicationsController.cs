using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Phronesis.Application.Common.Interfaces;
using Phronesis.Domain.Users;
using Phronesis.Shared.Responses;

namespace Phronesis.Api.Controllers.V1;

[ApiController]
[Route("api/v1/teacher-applications")]
public class TeacherApplicationsController : ControllerBase
{
    private readonly IApplicationDbContext _context;

    public TeacherApplicationsController(IApplicationDbContext context)
    {
        _context = context;
    }

    [HttpPost]
    public async Task<IActionResult> StartApplication(CancellationToken cancellationToken)
    {
        // Mock sub extraction
        var userIdStr = HttpContext.User.FindFirst("sub")?.Value;
        if (!Guid.TryParse(userIdStr, out var userId))
            return Unauthorized();

        var teacherProfile = await _context.TeacherProfiles
            .FirstOrDefaultAsync(t => t.UserId == userId, cancellationToken);
            
        if (teacherProfile == null)
            return NotFound("Teacher profile not found.");

        var existingApp = await _context.TeacherApplications
            .AnyAsync(a => a.TeacherProfileId == teacherProfile.Id && 
                          (a.Status != ApplicationStatus.Rejected && a.Status != ApplicationStatus.Approved), cancellationToken);
        
        if (existingApp)
            return BadRequest(ApiResponse.Failure("You already have an active application."));

        var application = new TeacherApplication(teacherProfile.Id);
        _context.TeacherApplications.Add(application);
        await _context.SaveChangesAsync(cancellationToken);

        return Ok(ApiResponse<object>.Ok(new { ApplicationId = application.Id }, "Application started."));
    }

    [HttpPost("{applicationId}/documents")]
    public async Task<IActionResult> AddDocument(Guid applicationId, [FromBody] AddDocumentRequest request, CancellationToken cancellationToken)
    {
        var application = await _context.TeacherApplications
            .Include(a => a.Documents)
            .FirstOrDefaultAsync(a => a.Id == applicationId, cancellationToken);

        if (application == null)
            return NotFound();

        var doc = new TeacherDocument(application.Id, request.Type, request.FileUri);
        application.AddDocument(doc);
        
        await _context.SaveChangesAsync(cancellationToken);

        return Ok(ApiResponse.Ok("Document added to application."));
    }

    [HttpPost("{applicationId}/submit")]
    public async Task<IActionResult> SubmitApplication(Guid applicationId, [FromBody] SubmitApplicationRequest request, CancellationToken cancellationToken)
    {
        var application = await _context.TeacherApplications
            .FirstOrDefaultAsync(a => a.Id == applicationId, cancellationToken);

        if (application == null)
            return NotFound();

        application.Submit(request.PolicyAccepted);
        await _context.SaveChangesAsync(cancellationToken);

        return Ok(ApiResponse.Ok("Application submitted successfully."));
    }
}

public record AddDocumentRequest(DocumentType Type, string FileUri);
public record SubmitApplicationRequest(bool PolicyAccepted);
