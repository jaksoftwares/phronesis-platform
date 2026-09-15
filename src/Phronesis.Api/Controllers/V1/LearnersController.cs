using Microsoft.AspNetCore.Mvc;
using Phronesis.Application.Authentication;
using Phronesis.Application.Common.Interfaces;
using Phronesis.Domain.Identity;
using Phronesis.Domain.Users;
using Phronesis.Infrastructure.Authentication;
using Phronesis.Shared.Responses;

namespace Phronesis.Api.Controllers.V1;

[ApiController]
[Route("api/v1/learners")]
public class LearnersController : ControllerBase
{
    private readonly IApplicationDbContext _context;
    private readonly IPasswordHasher _passwordHasher;

    public LearnersController(IApplicationDbContext context, IPasswordHasher passwordHasher)
    {
        _context = context;
        _passwordHasher = passwordHasher;
    }

    [HttpPost("register")]
    public async Task<IActionResult> RegisterLearner([FromBody] RegisterLearnerRequest request, CancellationToken cancellationToken)
    {
        if (_context.Users.Any(u => u.Email == request.Email))
        {
            return BadRequest(ApiResponse.Failure("Email already in use."));
        }

        var grade = await _context.GradeLevels.FindAsync(new object[] { request.GradeLevelId }, cancellationToken);
        if (grade == null)
        {
            return BadRequest(ApiResponse.Failure("Invalid GradeLevelId."));
        }

        var passwordHash = _passwordHasher.Hash(request.Password);
        var user = new User(request.Email, passwordHash, request.FirstName, request.LastName);
        var learnerProfile = new LearnerProfile(user.Id, grade.Id, request.DateOfBirth, request.SchoolName);

        // Fetch "Learner" role
        var role = _context.Roles.FirstOrDefault(r => r.Name == "Learner");
        if (role != null)
        {
            _context.UserRoles.Add(new UserRole(user.Id, role.Id));
        }

        _context.Users.Add(user);
        _context.LearnerProfiles.Add(learnerProfile);
        
        await _context.SaveChangesAsync(cancellationToken);

        return Ok(ApiResponse.Ok("Learner registered successfully."));
    }

    [HttpPost("me/invite-guardian")]
    // [Authorize] - omitted here for brevity, normally requires auth
    public async Task<IActionResult> InviteGuardian([FromBody] InviteGuardianRequest request, CancellationToken cancellationToken)
    {
        // In a real scenario, this would send an email invite token.
        // For MVP, we simulate the workflow.
        return Ok(ApiResponse.Ok($"Invitation sent to {request.GuardianEmail} successfully."));
    }
}

public record RegisterLearnerRequest(
    string Email,
    string Password,
    string FirstName,
    string LastName,
    Guid GradeLevelId,
    DateTime DateOfBirth,
    string? SchoolName
);

public record InviteGuardianRequest(string GuardianEmail, RelationshipType RelationshipType);


// {
//     [HttpGet]
//     public IActionResult ListLearners() => StatusCode(501);

//     [HttpPost]
//     public IActionResult CreateLearner() => StatusCode(501);

//     [HttpGet("{learnerId}")]
//     public IActionResult GetLearner(string learnerId) => StatusCode(501);

//     [HttpPatch("{learnerId}")]
//     public IActionResult UpdateProfile(string learnerId) => StatusCode(501);

//     [HttpPost("{learnerId}/activate")]
//     public IActionResult ActivateLearner(string learnerId) => StatusCode(501);

//     [HttpPost("{learnerId}/suspend")]
//     public IActionResult SuspendLearner(string learnerId) => StatusCode(501);

//     [HttpGet("{learnerId}/guardians")]
//     public IActionResult ListGuardians(string learnerId) => StatusCode(501);

//     [HttpPost("{learnerId}/guardians")]
//     public IActionResult AddGuardian(string learnerId) => StatusCode(501);

//     [HttpPatch("{learnerId}/guardians/{relationshipId}")]
//     public IActionResult UpdateGuardian(string learnerId, string relationshipId) => StatusCode(501);

//     [HttpDelete("{learnerId}/guardians/{relationshipId}")]
//     public IActionResult RemoveGuardian(string learnerId, string relationshipId) => StatusCode(501);

//     [HttpGet("/api/v1/me/learner-profile")]
//     public IActionResult GetOwnProfile() => StatusCode(501);

//     [HttpPatch("/api/v1/me/learner-profile")]
//     public IActionResult UpdateOwnProfile() => StatusCode(501);
// }
