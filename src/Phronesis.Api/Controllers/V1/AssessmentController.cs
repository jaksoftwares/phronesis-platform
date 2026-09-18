using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Phronesis.Application.Common.Interfaces;
using Phronesis.Domain.Learning;
using Phronesis.Shared.Responses;

namespace Phronesis.Api.Controllers.V1;

[ApiController]
[Route("api/v1/assessments")]
public class AssessmentController : ControllerBase
{
    private readonly IApplicationDbContext _context;

    public AssessmentController(IApplicationDbContext context)
    {
        _context = context;
    }

    [HttpPost]
    public async Task<IActionResult> CreateAssessment([FromBody] CreateAssessmentRequest request, CancellationToken cancellationToken)
    {
        // Add roles checking in real world (Author/Admin)
        var assessment = new Assessment(
            request.Title,
            request.Description,
            request.DurationMinutes,
            request.PassingScorePercentage,
            request.Type,
            request.SubjectId,
            request.TopicId);

        _context.Assessments.Add(assessment);
        await _context.SaveChangesAsync(cancellationToken);

        return Ok(ApiResponse<object>.Ok(new { assessment.Id }, "Assessment created successfully."));
    }

    [HttpPost("{id}/questions")]
    public async Task<IActionResult> AddQuestion(Guid id, [FromBody] AddQuestionRequest request, CancellationToken cancellationToken)
    {
        var assessment = await _context.Assessments
            .Include(a => a.Questions)
            .FirstOrDefaultAsync(a => a.Id == id, cancellationToken);

        if (assessment == null) return NotFound();

        try
        {
            var question = assessment.AddQuestion(request.Text, request.Points, request.Type);
            
            foreach (var opt in request.Options)
            {
                question.AddOption(opt.Text, opt.IsCorrect);
            }

            await _context.SaveChangesAsync(cancellationToken);
            return Ok(ApiResponse<object>.Ok(new { question.Id }, "Question added successfully."));
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(ApiResponse.Failure(ex.Message));
        }
    }

    [HttpPost("{id}/publish")]
    public async Task<IActionResult> PublishAssessment(Guid id, CancellationToken cancellationToken)
    {
        var assessment = await _context.Assessments
            .Include(a => a.Questions)
            .FirstOrDefaultAsync(a => a.Id == id, cancellationToken);

        if (assessment == null) return NotFound();

        try
        {
            assessment.Publish();
            await _context.SaveChangesAsync(cancellationToken);
            return Ok(ApiResponse.Ok("Assessment published."));
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(ApiResponse.Failure(ex.Message));
        }
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetAssessmentForLearner(Guid id, CancellationToken cancellationToken)
    {
        var assessment = await _context.Assessments
            .Include(a => a.Questions)
                .ThenInclude(q => q.Options)
            .FirstOrDefaultAsync(a => a.Id == id && a.IsPublished, cancellationToken);

        if (assessment == null) return NotFound(ApiResponse.Failure("Assessment not found or not published."));

        // STRIP SECRETS: We map to an anonymous object and intentionally exclude IsCorrect
        var payload = new
        {
            assessment.Id,
            assessment.Title,
            assessment.Description,
            assessment.DurationMinutes,
            assessment.PassingScorePercentage,
            assessment.Type,
            Questions = assessment.Questions.Select(q => new
            {
                q.Id,
                q.Text,
                q.Points,
                q.Type,
                Options = q.Options.Select(o => new
                {
                    o.Id,
                    o.Text
                    // SECURITY: Do not leak o.IsCorrect here!
                })
            })
        };

        return Ok(ApiResponse<object>.Ok(payload, "Assessment payload fetched securely."));
    }

    [HttpPost("{id}/attempts")]
    public async Task<IActionResult> StartAttempt(Guid id, CancellationToken cancellationToken)
    {
        var learnerIdStr = HttpContext.User.FindFirst("sub")?.Value;
        if (!Guid.TryParse(learnerIdStr, out var learnerId)) return Unauthorized();

        var assessmentExists = await _context.Assessments.AnyAsync(a => a.Id == id && a.IsPublished, cancellationToken);
        if (!assessmentExists) return NotFound();

        var attempt = new AssessmentAttempt(learnerId, id);
        _context.AssessmentAttempts.Add(attempt);
        await _context.SaveChangesAsync(cancellationToken);

        return Ok(ApiResponse<object>.Ok(new { attempt.Id, attempt.StartedAt }, "Attempt started."));
    }

    [HttpPost("attempts/{attemptId}/submit")]
    public async Task<IActionResult> SubmitAttempt(Guid attemptId, [FromBody] SubmitAttemptRequest request, CancellationToken cancellationToken)
    {
        var learnerIdStr = HttpContext.User.FindFirst("sub")?.Value;
        if (!Guid.TryParse(learnerIdStr, out var learnerId)) return Unauthorized();

        var attempt = await _context.AssessmentAttempts
            .Include(a => a.Assessment)
                .ThenInclude(a => a.Questions)
                    .ThenInclude(q => q.Options)
            .FirstOrDefaultAsync(a => a.Id == attemptId && a.LearnerId == learnerId, cancellationToken);

        if (attempt == null) return NotFound();

        try
        {
            double maxPossibleScore = 0;

            foreach (var q in attempt.Assessment.Questions)
            {
                maxPossibleScore += q.Points;
                
                var submittedAnswer = request.Answers.FirstOrDefault(a => a.QuestionId == q.Id);
                if (submittedAnswer != null)
                {
                    var selectedOption = q.Options.FirstOrDefault(o => o.Id == submittedAnswer.SelectedOptionId);
                    if (selectedOption != null)
                    {
                        var pointsAwarded = selectedOption.IsCorrect ? q.Points : 0;
                        attempt.AddAnswer(q.Id, selectedOption.Id, selectedOption.IsCorrect, pointsAwarded);
                    }
                }
            }

            attempt.Complete(maxPossibleScore);
            await _context.SaveChangesAsync(cancellationToken);

            var passed = attempt.ScorePercentage >= attempt.Assessment.PassingScorePercentage;

            return Ok(ApiResponse<object>.Ok(new 
            { 
                attempt.TotalScore,
                attempt.ScorePercentage,
                Passed = passed
            }, "Attempt evaluated successfully."));
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(ApiResponse.Failure(ex.Message));
        }
    }
}

public class CreateAssessmentRequest
{
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public int DurationMinutes { get; set; }
    public double PassingScorePercentage { get; set; }
    public AssessmentType Type { get; set; }
    public Guid SubjectId { get; set; }
    public Guid? TopicId { get; set; }
}

public class AddQuestionRequest
{
    public string Text { get; set; } = string.Empty;
    public double Points { get; set; }
    public QuestionType Type { get; set; }
    public List<QuestionOptionDto> Options { get; set; } = new();
}

public class QuestionOptionDto
{
    public string Text { get; set; } = string.Empty;
    public bool IsCorrect { get; set; }
}

public class SubmitAttemptRequest
{
    public List<SubmittedAnswerDto> Answers { get; set; } = new();
}

public class SubmittedAnswerDto
{
    public Guid QuestionId { get; set; }
    public Guid SelectedOptionId { get; set; }
}
