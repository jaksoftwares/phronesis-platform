using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Phronesis.Application.Common.Interfaces;
using Phronesis.Shared.Responses;

namespace Phronesis.Api.Controllers.Academic;

[ApiController]
[Route("api/v1/academic")]
public class AcademicController : ControllerBase
{
    private readonly IApplicationDbContext _context;

    public AcademicController(IApplicationDbContext context)
    {
        _context = context;
    }

    [HttpGet("grades")]
    public async Task<IActionResult> ListAllGrades(CancellationToken cancellationToken)
    {
        var grades = await _context.GradeLevels
            .AsNoTracking()
            .OrderBy(g => g.SortOrder)
            .Select(g => new 
            {
                g.Id,
                g.Name,
                g.Description,
                g.SortOrder
            })
            .ToListAsync(cancellationToken);

        return Ok(ApiResponse<object>.Ok(grades, "Fetched all active grades."));
    }

    [HttpGet("/api/v1/curricula")]
    public IActionResult ListCurricula() => StatusCode(501);

    [HttpPost("/api/v1/curricula")]
    public IActionResult CreateCurriculum() => StatusCode(501);

    [HttpGet("/api/v1/curricula/{curriculumId}")]
    public IActionResult GetCurriculum(string curriculumId) => StatusCode(501);

    [HttpPatch("/api/v1/curricula/{curriculumId}")]
    public IActionResult UpdateCurriculum(string curriculumId) => StatusCode(501);

    [HttpPost("/api/v1/curricula/{curriculumId}/publish")]
    public IActionResult PublishCurriculum(string curriculumId) => StatusCode(501);

    [HttpGet("/api/v1/curricula/{curriculumId}/grades")]
    public IActionResult ListGrades(string curriculumId) => StatusCode(501);

    [HttpPost("/api/v1/curricula/{curriculumId}/grades")]
    public IActionResult CreateGrade(string curriculumId) => StatusCode(501);

    [HttpPatch("/api/v1/grades/{gradeId}")]
    public IActionResult UpdateGrade(string gradeId) => StatusCode(501);

    [HttpGet("/api/v1/grades/{gradeId}/subjects")]
    public async Task<IActionResult> GradeSubjects(Guid gradeId, CancellationToken cancellationToken)
    {
        var subjects = await _context.GradeSubjects
            .Include(gs => gs.Subject)
            .Where(gs => gs.GradeLevelId == gradeId)
            .AsNoTracking()
            .Select(gs => new
            {
                gs.Subject.Id,
                gs.Subject.Name,
                gs.Subject.Code,
                gs.Subject.Description,
                gs.IsCore
            })
            .OrderBy(s => s.Name)
            .ToListAsync(cancellationToken);

        return Ok(ApiResponse<object>.Ok(subjects, "Subjects for grade fetched."));
    }

    [HttpPost("/api/v1/grades/{gradeId}/subjects")]
    public IActionResult AttachSubject(string gradeId) => StatusCode(501);

    [HttpDelete("/api/v1/grades/{gradeId}/subjects/{subjectId}")]
    public IActionResult DetachSubject(string gradeId, string subjectId) => StatusCode(501);

    [HttpGet("/api/v1/subjects")]
    public async Task<IActionResult> ListSubjects(CancellationToken cancellationToken)
    {
        var subjects = await _context.Subjects
            .AsNoTracking()
            .Select(s => new 
            {
                s.Id,
                s.Name,
                s.Code,
                s.Description
            })
            .ToListAsync(cancellationToken);

        return Ok(ApiResponse<object>.Ok(subjects, "Fetched all subjects."));
    }

    [HttpPost("/api/v1/subjects")]
    public IActionResult CreateSubject() => StatusCode(501);

    [HttpGet("/api/v1/subjects/{subjectId}")]
    public IActionResult GetSubject(string subjectId) => StatusCode(501);

    [HttpPatch("/api/v1/subjects/{subjectId}")]
    public IActionResult UpdateSubject(string subjectId) => StatusCode(501);

    [HttpGet("/api/v1/subjects/{subjectId}/topics")]
    public async Task<IActionResult> ListTopics(Guid subjectId, CancellationToken cancellationToken)
    {
        var strands = await _context.Strands
            .Where(s => s.SubjectId == subjectId && s.IsActive)
            .AsNoTracking()
            .OrderBy(s => s.SortOrder)
            .Select(s => new
            {
                s.Id,
                s.Name,
                s.SortOrder
            })
            .ToListAsync(cancellationToken);

        return Ok(ApiResponse<object>.Ok(strands, "Topics (strands) for subject fetched."));
    }

    [HttpPost("/api/v1/subjects/{subjectId}/topics")]
    public IActionResult CreateTopic(string subjectId) => StatusCode(501);

    [HttpGet("/api/v1/topics/{topicId}")]
    public IActionResult GetTopic(string topicId) => StatusCode(501);

    [HttpPatch("/api/v1/topics/{topicId}")]
    public IActionResult UpdateTopic(string topicId) => StatusCode(501);

    [HttpPost("/api/v1/topics/{topicId}/children")]
    public IActionResult CreateSubtopic(string topicId) => StatusCode(501);

    [HttpGet("/api/v1/topics/{topicId}/resources")]
    public IActionResult MappedResources(string topicId) => StatusCode(501);

    [HttpGet("/api/v1/academic/catalog")]
    public IActionResult FullCatalog() => StatusCode(501);
}

