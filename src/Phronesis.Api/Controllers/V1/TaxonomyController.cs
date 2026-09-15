using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Phronesis.Application.Common.Interfaces;
using Phronesis.Domain.Academic;
using Phronesis.Shared.Responses;

namespace Phronesis.Api.Controllers.V1;

[ApiController]
[Route("api/v1/academic")]
public class TaxonomyController : ControllerBase
{
    private readonly IApplicationDbContext _context;

    public TaxonomyController(IApplicationDbContext context)
    {
        _context = context;
    }

    [HttpPost("subjects")]
    public async Task<IActionResult> CreateSubject([FromBody] CreateSubjectRequest request, CancellationToken cancellationToken)
    {
        var subject = new Subject(request.Name, request.Code, request.Description);
        _context.Subjects.Add(subject);
        await _context.SaveChangesAsync(cancellationToken);

        return Ok(ApiResponse<object>.Ok(new { subject.Id }, "Subject created successfully."));
    }

    [HttpPost("subjects/{subjectId}/strands")]
    public async Task<IActionResult> AddStrand(Guid subjectId, [FromBody] AddStrandRequest request, CancellationToken cancellationToken)
    {
        var subject = await _context.Subjects.FindAsync(new object[] { subjectId }, cancellationToken);
        if (subject == null) return NotFound("Subject not found.");

        var strand = new Strand(subject.Id, request.Name, request.SortOrder);
        _context.Strands.Add(strand);
        await _context.SaveChangesAsync(cancellationToken);

        return Ok(ApiResponse<object>.Ok(new { strand.Id }, "Strand added successfully."));
    }

    [HttpPost("strands/{strandId}/sub-strands")]
    public async Task<IActionResult> AddSubStrand(Guid strandId, [FromBody] AddSubStrandRequest request, CancellationToken cancellationToken)
    {
        var strand = await _context.Strands.FindAsync(new object[] { strandId }, cancellationToken);
        if (strand == null) return NotFound("Strand not found.");

        var subStrand = new SubStrand(strand.Id, request.Name, request.SortOrder);
        _context.SubStrands.Add(subStrand);
        await _context.SaveChangesAsync(cancellationToken);

        return Ok(ApiResponse<object>.Ok(new { subStrand.Id }, "SubStrand added successfully."));
    }

    [HttpPost("sub-strands/{subStrandId}/objectives")]
    public async Task<IActionResult> AddLearningObjective(Guid subStrandId, [FromBody] AddLearningObjectiveRequest request, CancellationToken cancellationToken)
    {
        var subStrand = await _context.SubStrands.FindAsync(new object[] { subStrandId }, cancellationToken);
        if (subStrand == null) return NotFound("SubStrand not found.");

        var objective = new LearningObjective(subStrand.Id, request.Description, request.SortOrder);
        _context.LearningObjectives.Add(objective);
        await _context.SaveChangesAsync(cancellationToken);

        return Ok(ApiResponse<object>.Ok(new { objective.Id }, "Learning Objective added successfully."));
    }
}

public record CreateSubjectRequest(string Name, string Code, string Description);
public record AddStrandRequest(string Name, int SortOrder);
public record AddSubStrandRequest(string Name, int SortOrder);
public record AddLearningObjectiveRequest(string Description, int SortOrder);
