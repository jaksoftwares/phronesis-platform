using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Phronesis.Application.Common.Interfaces;
using Phronesis.Domain.Academic;
using Phronesis.Shared.Responses;

namespace Phronesis.Api.Controllers.V1;

[ApiController]
[Route("api/v1/academic/curricula")]
public class CurriculumController : ControllerBase
{
    private readonly IApplicationDbContext _context;

    public CurriculumController(IApplicationDbContext context)
    {
        _context = context;
    }

    [HttpPost]
    public async Task<IActionResult> CreateCurriculum([FromBody] CreateCurriculumRequest request, CancellationToken cancellationToken)
    {
        var curriculum = new Curriculum(request.Name, request.Version, request.Description);
        _context.Curricula.Add(curriculum);
        await _context.SaveChangesAsync(cancellationToken);

        return Ok(ApiResponse<object>.Ok(new { curriculum.Id }, "Curriculum created successfully."));
    }

    [HttpGet]
    public async Task<IActionResult> GetCurricula(CancellationToken cancellationToken)
    {
        var curricula = await _context.Curricula
            .Where(c => c.IsActive)
            .ToListAsync(cancellationToken);

        return Ok(ApiResponse<object>.Ok(curricula, "Curricula fetched successfully."));
    }

    [HttpPost("{curriculumId}/education-levels")]
    public async Task<IActionResult> AddEducationLevel(Guid curriculumId, [FromBody] AddEducationLevelRequest request, CancellationToken cancellationToken)
    {
        var curriculum = await _context.Curricula.FindAsync(new object[] { curriculumId }, cancellationToken);
        if (curriculum == null) return NotFound("Curriculum not found.");

        var eduLevel = new EducationLevel(curriculum.Id, request.Name, request.Description, request.SortOrder);
        _context.EducationLevels.Add(eduLevel);
        await _context.SaveChangesAsync(cancellationToken);

        return Ok(ApiResponse.Ok("Education Level added successfully."));
    }

    [HttpGet("{curriculumId}/tree")]
    public async Task<IActionResult> GetCurriculumTree(Guid curriculumId, CancellationToken cancellationToken)
    {
        // Fetches Curriculum -> Education Levels
        var curriculum = await _context.Curricula
            .FirstOrDefaultAsync(c => c.Id == curriculumId, cancellationToken);

        if (curriculum == null) return NotFound();

        var eduLevels = await _context.EducationLevels
            .Where(e => e.CurriculumId == curriculumId && e.IsActive)
            .OrderBy(e => e.SortOrder)
            .ToListAsync(cancellationToken);

        return Ok(ApiResponse<object>.Ok(new { Curriculum = curriculum, EducationLevels = eduLevels }, "Fetched tree."));
    }
}

public record CreateCurriculumRequest(string Name, string Version, string Description);
public record AddEducationLevelRequest(string Name, string Description, int SortOrder);
