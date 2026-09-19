using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Phronesis.Application.Common.Interfaces;
using Phronesis.Domain.Academic;
using Phronesis.Domain.Users;
using Phronesis.Shared.Responses;

namespace Phronesis.Api.Controllers.V1;

[ApiController]
[Route("api/v1/academic/configurations")]
public class AcademicConfigurationController : ControllerBase
{
    private readonly IApplicationDbContext _context;

    public AcademicConfigurationController(IApplicationDbContext context)
    {
        _context = context;
    }


    [HttpPost("grade-subjects")]
    public async Task<IActionResult> MapSubjectToGrade([FromBody] MapSubjectRequest request, CancellationToken cancellationToken)
    {
        var exists = await _context.GradeSubjects
            .AnyAsync(gs => gs.GradeLevelId == request.GradeLevelId && gs.SubjectId == request.SubjectId, cancellationToken);
            
        if (exists) return BadRequest(ApiResponse.Failure("This subject is already mapped to this grade."));

        var gradeSubject = new GradeSubject(request.GradeLevelId, request.SubjectId, request.IsCore, request.PeriodsPerWeek);
        _context.GradeSubjects.Add(gradeSubject);
        await _context.SaveChangesAsync(cancellationToken);

        return Ok(ApiResponse<object>.Ok(new { gradeSubject.Id }, "Subject mapped to grade successfully."));
    }

    [HttpGet("grades/{gradeId}/subjects")]
    public async Task<IActionResult> GetSubjectsForGrade(Guid gradeId, CancellationToken cancellationToken)
    {
        var subjects = await _context.GradeSubjects
            .Include(gs => gs.Subject)
            .Where(gs => gs.GradeLevelId == gradeId)
            .Select(gs => new 
            {
                gs.Subject.Id,
                gs.Subject.Name,
                gs.Subject.Code,
                gs.IsCore,
                gs.PeriodsPerWeek
            })
            .ToListAsync(cancellationToken);

        return Ok(ApiResponse<object>.Ok(subjects, "Fetched grade subjects."));
    }

    [HttpPost("prerequisites")]
    public async Task<IActionResult> MapPrerequisite([FromBody] MapPrerequisiteRequest request, CancellationToken cancellationToken)
    {
        if (request.SubStrandId == request.PrerequisiteId)
            return BadRequest(ApiResponse.Failure("Cannot set a topic as its own prerequisite."));

        var prerequisite = new SubStrandPrerequisite(request.SubStrandId, request.PrerequisiteId);
        _context.SubStrandPrerequisites.Add(prerequisite);
        await _context.SaveChangesAsync(cancellationToken);

        return Ok(ApiResponse<object>.Ok(new { prerequisite.Id }, "Prerequisite mapped successfully."));
    }

    [HttpPost("teacher-competences")]
    public async Task<IActionResult> AssignTeacherCompetence([FromBody] AssignTeacherCompetenceRequest request, CancellationToken cancellationToken)
    {
        var exists = await _context.TeacherCompetences
            .AnyAsync(tc => tc.TeacherProfileId == request.TeacherProfileId 
                         && tc.SubjectId == request.SubjectId 
                         && tc.GradeLevelId == request.GradeLevelId, cancellationToken);

        if (exists) return BadRequest(ApiResponse.Failure("Teacher is already mapped to this subject and grade."));

        var competence = new TeacherCompetence(request.TeacherProfileId, request.SubjectId, request.GradeLevelId);
        _context.TeacherCompetences.Add(competence);
        await _context.SaveChangesAsync(cancellationToken);

        return Ok(ApiResponse<object>.Ok(new { competence.Id }, "Teacher competence assigned."));
    }
}

public record MapSubjectRequest(Guid GradeLevelId, Guid SubjectId, bool IsCore, int PeriodsPerWeek);
public record MapPrerequisiteRequest(Guid SubStrandId, Guid PrerequisiteId);
public record AssignTeacherCompetenceRequest(Guid TeacherProfileId, Guid SubjectId, Guid GradeLevelId);
