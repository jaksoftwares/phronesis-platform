using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Phronesis.Application.Common.Interfaces;
using Phronesis.Domain.Identity;
using Phronesis.Domain.Users;
using Phronesis.Application.Authentication;
using Phronesis.Infrastructure.Authentication;
using Phronesis.Shared.Responses;

namespace Phronesis.Api.Controllers.V1;

[ApiController]
[Route("api/v1/guardians")]
public class GuardiansController : ControllerBase
{
    private readonly IApplicationDbContext _context;
    private readonly IPasswordHasher _passwordHasher;
    private readonly ILogger<GuardiansController> _logger;
    private readonly IConfiguration _configuration;

    public GuardiansController(
        IApplicationDbContext context, 
        IPasswordHasher passwordHasher,
        ILogger<GuardiansController> logger,
        IConfiguration configuration)
    {
        _context = context;
        _passwordHasher = passwordHasher;
        _logger = logger;
        _configuration = configuration;
    }

    [HttpPost("register")]
    public async Task<IActionResult> RegisterGuardian([FromBody] RegisterGuardianRequest request, CancellationToken cancellationToken)
    {
        if (_context.Users.Any(u => u.Email == request.Email))
        {
            return BadRequest(ApiResponse.Failure("Email already in use."));
        }

        var passwordHash = _passwordHasher.Hash(request.Password);
        var user = new User(request.Email, passwordHash, request.FirstName, request.LastName);
        var guardianProfile = new GuardianProfile(user.Id, request.PhoneNumber);

        // Fetch or create "Guardian" role
        var role = _context.Roles.FirstOrDefault(r => r.Name == "Guardian");
        if (role == null)
        {
            role = new Role("Guardian", "Parent or Guardian");
            _context.Roles.Add(role);
            await _context.SaveChangesAsync(cancellationToken);
        }
        _context.UserRoles.Add(new UserRole(user.Id, role.Id));

        _context.Users.Add(user);
        _context.GuardianProfiles.Add(guardianProfile);

        // If LearnerRegistrationNumber is provided, dispatch a pending link request to the learner
        if (!string.IsNullOrWhiteSpace(request.LearnerRegistrationNumber))
        {
            var learner = await _context.LearnerProfiles
                .FirstOrDefaultAsync(l => l.RegistrationNumber == request.LearnerRegistrationNumber, cancellationToken);
            
            if (learner != null)
            {
                var linkRequest = new LearnerGuardianLinkRequest(
                    learner.Id, 
                    guardianProfile.Id, 
                    request.RelationshipType ?? RelationshipType.Other
                );
                _context.LearnerGuardianLinkRequests.Add(linkRequest);
            }
        }
        
        var token = Guid.NewGuid().ToString("N");
        user.SetEmailVerificationToken(token, DateTime.UtcNow.AddHours(1));

        await _context.SaveChangesAsync(cancellationToken);

        var frontendUrl = _configuration["FrontendUrl"] ?? "http://localhost:3000";
        var verificationLink = $"{frontendUrl}/shared/verify-email?token={token}&email={System.Web.HttpUtility.UrlEncode(user.Email)}";
        
        _logger.LogInformation("================================================");
        _logger.LogInformation("DEV ALERT: REGISTRATION EMAIL VERIFICATION LINK");
        _logger.LogInformation("Link: {VerificationLink}", verificationLink);
        _logger.LogInformation("================================================");

        return Ok(ApiResponse.Ok("Guardian registered successfully."));
    }

    [HttpGet("me/learners")]
    // [Authorize]
    public async Task<IActionResult> GetMyLearners(CancellationToken cancellationToken)
    {
        // Mocking user ID for now since auth context isn't fully wired to User.Identity
        var userIdStr = HttpContext.User.FindFirst("sub")?.Value;
        if (!Guid.TryParse(userIdStr, out var userId))
            return Unauthorized();

        var guardian = await _context.GuardianProfiles
            .FirstOrDefaultAsync(g => g.UserId == userId, cancellationToken);
            
        if (guardian == null) return NotFound("Guardian profile not found.");

        var linkedLearners = await _context.LearnerGuardians
            .Include(lg => lg.LearnerProfile)
            .ThenInclude(lp => lp.User)
            .Where(lg => lg.GuardianProfileId == guardian.Id && lg.CanViewProgress)
            .Select(lg => new {
                lg.LearnerProfileId,
                lg.LearnerProfile.User.FirstName,
                lg.LearnerProfile.User.LastName,
                lg.RelationshipType,
                lg.IsPrimaryPayer
            })
            .ToListAsync(cancellationToken);

        return Ok(ApiResponse<object>.Ok(linkedLearners, "Fetched learners."));
    }

    [HttpPost("me/invite-learner")]
    // [Authorize]
    public async Task<IActionResult> InviteLearner([FromBody] InviteLearnerRequest request, CancellationToken cancellationToken)
    {
        var userIdStr = HttpContext.User.FindFirst("sub")?.Value;
        if (!Guid.TryParse(userIdStr, out var userId))
            return Unauthorized();

        var guardian = await _context.GuardianProfiles
            .FirstOrDefaultAsync(g => g.UserId == userId, cancellationToken);
            
        if (guardian == null) return NotFound("Guardian profile not found.");

        var learner = await _context.LearnerProfiles
            .FirstOrDefaultAsync(l => l.RegistrationNumber == request.LearnerRegistrationNumber, cancellationToken);
            
        if (learner == null) return NotFound(ApiResponse.Failure("Learner not found with that Registration Number."));

        // Check if already linked
        var alreadyLinked = await _context.LearnerGuardians
            .AnyAsync(lg => lg.LearnerProfileId == learner.Id && lg.GuardianProfileId == guardian.Id, cancellationToken);
        if (alreadyLinked) return BadRequest(ApiResponse.Failure("You are already linked to this learner."));

        // Check if request already pending
        var alreadyPending = await _context.LearnerGuardianLinkRequests
            .AnyAsync(r => r.LearnerProfileId == learner.Id && r.GuardianProfileId == guardian.Id && r.Status == LinkRequestStatus.Pending, cancellationToken);
        if (alreadyPending) return BadRequest(ApiResponse.Failure("A link request is already pending for this learner."));

        var linkRequest = new LearnerGuardianLinkRequest(
            learner.Id, 
            guardian.Id, 
            request.RelationshipType
        );
        _context.LearnerGuardianLinkRequests.Add(linkRequest);
        await _context.SaveChangesAsync(cancellationToken);

        return Ok(ApiResponse.Ok("Invite sent to learner successfully."));
    }

    [HttpPost("me/link-requests/{requestId}/accept")]
    // [Authorize]
    public async Task<IActionResult> AcceptLearnerLink(Guid requestId, CancellationToken cancellationToken)
    {
        var userIdStr = HttpContext.User.FindFirst("sub")?.Value;
        if (!Guid.TryParse(userIdStr, out var userId))
            return Unauthorized();

        var guardian = await _context.GuardianProfiles
            .FirstOrDefaultAsync(g => g.UserId == userId, cancellationToken);
            
        if (guardian == null) return NotFound("Guardian profile not found.");

        var linkRequest = await _context.LearnerGuardianLinkRequests
            .FirstOrDefaultAsync(r => r.Id == requestId && r.GuardianProfileId == guardian.Id && r.Status == LinkRequestStatus.Pending, cancellationToken);

        if (linkRequest == null) return NotFound(ApiResponse.Failure("Pending link request not found."));

        linkRequest.Accept();

        var learnerGuardian = new LearnerGuardian(
            linkRequest.LearnerProfileId, 
            guardian.Id, 
            linkRequest.RelationshipType,
            canViewProgress: true,
            isPrimaryPayer: true
        );
        _context.LearnerGuardians.Add(learnerGuardian);

        await _context.SaveChangesAsync(cancellationToken);

        return Ok(ApiResponse.Ok("Link request accepted successfully."));
    }
}

public record RegisterGuardianRequest(
    string Email,
    string Password,
    string FirstName,
    string LastName,
    string PhoneNumber,
    string? LearnerRegistrationNumber,
    RelationshipType? RelationshipType
);

public record InviteLearnerRequest(string LearnerRegistrationNumber, RelationshipType RelationshipType);
