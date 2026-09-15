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

    public GuardiansController(IApplicationDbContext context, IPasswordHasher passwordHasher)
    {
        _context = context;
        _passwordHasher = passwordHasher;
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

        // Fetch "Guardian" role
        var role = _context.Roles.FirstOrDefault(r => r.Name == "Guardian");
        if (role != null)
        {
            _context.UserRoles.Add(new UserRole(user.Id, role.Id));
        }

        _context.Users.Add(user);
        _context.GuardianProfiles.Add(guardianProfile);

        // If LearnerId is provided (e.g. from invite link)
        if (request.LearnerProfileId.HasValue)
        {
            var learner = await _context.LearnerProfiles.FindAsync(new object[] { request.LearnerProfileId.Value }, cancellationToken);
            if (learner != null)
            {
                var learnerGuardian = new LearnerGuardian(
                    learner.Id, 
                    guardianProfile.Id, 
                    request.RelationshipType ?? RelationshipType.Other,
                    canViewProgress: true,
                    isPrimaryPayer: true // Assume primary payer for MVP invite flow
                );
                _context.LearnerGuardians.Add(learnerGuardian);
            }
        }
        
        await _context.SaveChangesAsync(cancellationToken);

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
}

public record RegisterGuardianRequest(
    string Email,
    string Password,
    string FirstName,
    string LastName,
    string PhoneNumber,
    Guid? LearnerProfileId,
    RelationshipType? RelationshipType
);
