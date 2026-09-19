using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Phronesis.Application.Common.Interfaces;
using Phronesis.Domain.Users;
using Phronesis.Shared.Responses;

namespace Phronesis.Api.Controllers.V1;

[ApiController]
[Route("api/v1/teacher-onboarding")]
[Authorize]
public class TeacherOnboardingController : ControllerBase
{
    private readonly IApplicationDbContext _context;
    private readonly IFileStorageService _fileStorage;

    public TeacherOnboardingController(IApplicationDbContext context, IFileStorageService fileStorage)
    {
        _context = context;
        _fileStorage = fileStorage;
    }

    private Guid GetUserId()
    {
        var sub = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value 
               ?? User.FindFirst("sub")?.Value;
        
        if (string.IsNullOrEmpty(sub) || !Guid.TryParse(sub, out var userId))
        {
            throw new UnauthorizedAccessException();
        }
        return userId;
    }

    [HttpGet("me/application")]
    public async Task<IActionResult> GetMyApplication(CancellationToken cancellationToken)
    {
        var userId = GetUserId();

        var teacherProfile = await _context.TeacherProfiles
            .FirstOrDefaultAsync(t => t.UserId == userId, cancellationToken);

        if (teacherProfile == null)
            return Unauthorized(ApiResponse.Failure("Not a teacher profile."));

        var application = await _context.TeacherApplications
            .Include(a => a.Documents)
            .FirstOrDefaultAsync(a => a.TeacherProfileId == teacherProfile.Id, cancellationToken);

        if (application == null)
            return NotFound(ApiResponse.Failure("No application found."));

        var competences = await _context.TeacherCompetences
            .Where(c => c.TeacherProfileId == teacherProfile.Id)
            .ToListAsync(cancellationToken);

        return Ok(ApiResponse<object>.Ok(new
        {
            application.Id,
            application.Status,
            application.SubmittedAt,
            application.AdminNotes,
            application.InterviewDate,
            application.InterviewLink,
            application.InterviewNotes,
            Documents = application.Documents.Select(d => new { d.Id, d.DocumentType, d.FileUri, d.VerificationStatus, d.RejectionReason }),
            Profile = new
            {
                teacherProfile.Bio,
                teacherProfile.Qualifications,
                teacherProfile.ExperienceYears,
                teacherProfile.TeachingSkills
            },
            Subjects = competences.Select(c => new
            {
                c.SubjectId,
                c.GradeLevelId
            })
        }, "Application retrieved."));
    }

    [HttpPost("me/application")]
    public async Task<IActionResult> CreateDraftApplication(CancellationToken cancellationToken)
    {
        var userId = GetUserId();

        var teacherProfile = await _context.TeacherProfiles
            .FirstOrDefaultAsync(t => t.UserId == userId, cancellationToken);

        if (teacherProfile == null)
            return Unauthorized(ApiResponse.Failure("Not a teacher profile."));

        var existing = await _context.TeacherApplications
            .FirstOrDefaultAsync(a => a.TeacherProfileId == teacherProfile.Id, cancellationToken);

        if (existing != null)
            return BadRequest(ApiResponse.Failure("An application already exists."));

        var application = new TeacherApplication(teacherProfile.Id);
        _context.TeacherApplications.Add(application);
        await _context.SaveChangesAsync(cancellationToken);

        return Ok(ApiResponse<object>.Ok(new { application.Id, application.Status }, "Draft application created."));
    }

    [HttpPut("me/profile")]
    public async Task<IActionResult> UpdateProfile([FromBody] UpdateOnboardingProfileRequest request, CancellationToken cancellationToken)
    {
        var userId = GetUserId();

        var teacherProfile = await _context.TeacherProfiles
            .FirstOrDefaultAsync(t => t.UserId == userId, cancellationToken);

        if (teacherProfile == null)
            return Unauthorized();

        teacherProfile.UpdateProfile(request.Bio, request.Qualifications, request.ExperienceYears, request.TeachingSkills);
        
        await _context.SaveChangesAsync(cancellationToken);

        return Ok(ApiResponse.Ok("Profile step updated."));
    }

    [HttpPut("me/subjects")]
    public async Task<IActionResult> UpdateSubjects([FromBody] UpdateOnboardingSubjectsRequest request, CancellationToken cancellationToken)
    {
        var userId = GetUserId();

        var teacherProfile = await _context.TeacherProfiles
            .FirstOrDefaultAsync(t => t.UserId == userId, cancellationToken);

        if (teacherProfile == null)
            return Unauthorized();

        if (request.SubjectSelections.Count > 3)
            return BadRequest(ApiResponse.Failure("You can select a maximum of 3 subjects."));

        // Remove existing competences
        var existingCompetences = await _context.TeacherCompetences
            .Where(c => c.TeacherProfileId == teacherProfile.Id)
            .ToListAsync(cancellationToken);
        
        _context.TeacherCompetences.RemoveRange(existingCompetences);

        // Add new competences
        foreach (var selection in request.SubjectSelections)
        {
            _context.TeacherCompetences.Add(new TeacherCompetence(teacherProfile.Id, selection.SubjectId, selection.GradeLevelId));
        }

        await _context.SaveChangesAsync(cancellationToken);

        return Ok(ApiResponse.Ok("Subjects updated successfully."));
    }

    [HttpPost("me/documents")]
    public async Task<IActionResult> UploadDocument([FromForm] UploadOnboardingDocumentRequest request, CancellationToken cancellationToken)
    {
        var userId = GetUserId();

        var teacherProfile = await _context.TeacherProfiles
            .FirstOrDefaultAsync(t => t.UserId == userId, cancellationToken);

        if (teacherProfile == null)
            return Unauthorized();

        var application = await _context.TeacherApplications
            .Include(a => a.Documents)
            .FirstOrDefaultAsync(a => a.TeacherProfileId == teacherProfile.Id, cancellationToken);

        if (application == null)
            return BadRequest(ApiResponse.Failure("You must create a draft application first."));

        if (application.Status != ApplicationStatus.Draft)
            return BadRequest(ApiResponse.Failure("Documents can only be uploaded while in Draft status."));

        if (request.File == null || request.File.Length == 0)
            return BadRequest(ApiResponse.Failure("File is required."));

        using var stream = request.File.OpenReadStream();
        var uri = await _fileStorage.UploadFileAsync(stream, request.File.FileName, "uploads/teacher-documents", cancellationToken);

        // Query directly from the DbSet to get any existing row (avoids stale tracking issues)
        var existingDocs = await _context.TeacherDocuments
            .Where(d => d.TeacherApplicationId == application.Id && d.DocumentType == request.DocumentType)
            .ToListAsync(cancellationToken);

        if (existingDocs.Count > 0)
        {
            // Update the first one in-place, remove any duplicates
            existingDocs[0].UpdateFileUri(uri);
            if (existingDocs.Count > 1)
            {
                // Clean up orphan duplicates from previous broken uploads
                _context.TeacherDocuments.RemoveRange(existingDocs.Skip(1));
            }
        }
        else
        {
            var document = new TeacherDocument(application.Id, request.DocumentType, uri);
            _context.TeacherDocuments.Add(document);
        }

        await _context.SaveChangesAsync(cancellationToken);

        return Ok(ApiResponse.Ok("Document uploaded successfully."));
    }

    [HttpPost("me/submit")]
    public async Task<IActionResult> SubmitApplication(CancellationToken cancellationToken)
    {
        var userId = GetUserId();

        var teacherProfile = await _context.TeacherProfiles
            .FirstOrDefaultAsync(t => t.UserId == userId, cancellationToken);

        if (teacherProfile == null)
            return Unauthorized();

        var application = await _context.TeacherApplications
            .Include(a => a.Documents)
            .FirstOrDefaultAsync(a => a.TeacherProfileId == teacherProfile.Id, cancellationToken);

        if (application == null)
            return BadRequest(ApiResponse.Failure("No application found."));

        // Validation
        if (string.IsNullOrWhiteSpace(teacherProfile.Bio) || string.IsNullOrWhiteSpace(teacherProfile.Qualifications))
            return BadRequest(ApiResponse.Failure("Profile details (Bio, Qualifications) are required before submission."));

        var subjectsCount = await _context.TeacherCompetences.CountAsync(c => c.TeacherProfileId == teacherProfile.Id, cancellationToken);
        if (subjectsCount == 0)
            return BadRequest(ApiResponse.Failure("You must select at least one subject."));

        var hasId = application.Documents.Any(d => d.DocumentType == DocumentType.NationalId);
        var hasResume = application.Documents.Any(d => d.DocumentType == DocumentType.Resume);
        var hasKcse = application.Documents.Any(d => d.DocumentType == DocumentType.KcseCertificate);
        var hasDegree = application.Documents.Any(d => d.DocumentType == DocumentType.DegreeCertificate);
        
        if (!hasId || !hasResume || !hasKcse || !hasDegree)
            return BadRequest(ApiResponse.Failure("National ID, KCSE Certificate, Degree/Teaching Certificate, and Resume are mandatory documents."));

        application.Submit(policyAccepted: true);
        await _context.SaveChangesAsync(cancellationToken);

        return Ok(ApiResponse.Ok("Application submitted successfully."));
    }
}

public record UpdateOnboardingProfileRequest(string Bio, string Qualifications, int ExperienceYears, string TeachingSkills);
public record SubjectSelection(Guid SubjectId, Guid GradeLevelId);
public record UpdateOnboardingSubjectsRequest(List<SubjectSelection> SubjectSelections);

public class UploadOnboardingDocumentRequest
{
    public DocumentType DocumentType { get; set; }
    public IFormFile File { get; set; } = null!;
}
