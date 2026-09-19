using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
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
    private readonly ILogger<LearnersController> _logger;
    private readonly IConfiguration _configuration;

    public LearnersController(
        IApplicationDbContext context, 
        IPasswordHasher passwordHasher,
        ILogger<LearnersController> logger,
        IConfiguration configuration)
    {
        _context = context;
        _passwordHasher = passwordHasher;
        _logger = logger;
        _configuration = configuration;
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

        // Fetch or create "Learner" role
        var role = await _context.Roles.FirstOrDefaultAsync(r => r.Name == "Learner", cancellationToken);
        if (role == null)
        {
            role = new Role("Learner", "Student/Learner");
            _context.Roles.Add(role);
            await _context.SaveChangesAsync(cancellationToken);
        }
        _context.UserRoles.Add(new UserRole(user.Id, role.Id));

        var token = Guid.NewGuid().ToString("N");
        user.SetEmailVerificationToken(token, DateTime.UtcNow.AddHours(1));

        _context.Users.Add(user);
        _context.LearnerProfiles.Add(learnerProfile);
        
        await _context.SaveChangesAsync(cancellationToken);

        var frontendUrl = _configuration["FrontendUrl"] ?? "http://localhost:3000";
        var verificationLink = $"{frontendUrl}/shared/verify-email?token={token}&email={System.Web.HttpUtility.UrlEncode(user.Email)}";
        
        _logger.LogInformation("================================================");
        _logger.LogInformation("DEV ALERT: REGISTRATION EMAIL VERIFICATION LINK");
        _logger.LogInformation("Link: {VerificationLink}", verificationLink);
        _logger.LogInformation("================================================");

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
