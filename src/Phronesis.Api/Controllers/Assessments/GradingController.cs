using Microsoft.AspNetCore.Mvc;
using Phronesis.Shared.Responses;

namespace Phronesis.Api.Controllers.Assessments;

[ApiController]
[Route("api/v1/grading")]
public class GradingController : ControllerBase
{
    [HttpGet("inbox")]
    public IActionResult GetGradingInbox()
    {
        // Mock data to wire up the frontend end-to-end
        var submissions = new[]
        {
            new { id = "s1", studentName = "Alex Mercer", assignmentTitle = "Algebra: Linear Equations Quiz", submittedAt = "2 hours ago", status = "Pending", score = (string?)null },
            new { id = "s2", studentName = "Sarah Connor", assignmentTitle = "Essay: Themes in Shakespeare", submittedAt = "5 hours ago", status = "Pending", score = (string?)null },
            new { id = "s3", studentName = "John Doe", assignmentTitle = "Mid-Term Mock Exam", submittedAt = "1 day ago", status = "Pending", score = (string?)null },
            new { id = "s4", studentName = "Jane Smith", assignmentTitle = "Algebra: Linear Equations Quiz", submittedAt = "2 days ago", status = "Graded", score = "85/100" },
            new { id = "s5", studentName = "Michael Chang", assignmentTitle = "Physics: Kinematics Lab", submittedAt = "3 days ago", status = "Graded", score = "92/100" }
        };

        return Ok(ApiResponse<object>.Ok(submissions, "Fetched grading inbox."));
    }

    [HttpPost("submissions/{id}/grade")]
    public IActionResult SubmitGrade(string id, [FromBody] SubmitGradeRequest request)
    {
        // Mocking saving a grade
        return Ok(ApiResponse<object>.Ok(new { Id = id, request.Score, Status = "Graded" }, "Grade saved successfully."));
    }
}

public class SubmitGradeRequest
{
    public double Score { get; set; }
    public string? Feedback { get; set; }
}
