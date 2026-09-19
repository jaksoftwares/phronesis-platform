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
[Route("api/v1/teachers")]
public class TeachersController : ControllerBase
{
    private readonly IApplicationDbContext _context;
    private readonly IPasswordHasher _passwordHasher;
    private readonly ILogger<TeachersController> _logger;
    private readonly IConfiguration _configuration;

    public TeachersController(
        IApplicationDbContext context, 
        IPasswordHasher passwordHasher,
        ILogger<TeachersController> logger,
        IConfiguration configuration)
    {
        _context = context;
        _passwordHasher = passwordHasher;
        _logger = logger;
        _configuration = configuration;
    }

    [HttpPost("register")]
    public async Task<IActionResult> RegisterTeacher([FromBody] RegisterTeacherRequest request, CancellationToken cancellationToken)
    {
        if (_context.Users.Any(u => u.Email == request.Email))
        {
            return BadRequest(ApiResponse.Failure("Email already in use."));
        }

        var passwordHash = _passwordHasher.Hash(request.Password);
        var user = new User(request.Email, passwordHash, request.FirstName, request.LastName);
        var teacherProfile = new TeacherProfile(user.Id);

        var role = _context.Roles.FirstOrDefault(r => r.Name == "Teacher");
        if (role == null)
        {
            role = new Role("Teacher", "Teaching Staff");
            _context.Roles.Add(role);
            await _context.SaveChangesAsync(cancellationToken);
        }
        _context.UserRoles.Add(new UserRole(user.Id, role.Id));

        _context.Users.Add(user);
        _context.TeacherProfiles.Add(teacherProfile);
        
        var token = Guid.NewGuid().ToString("N");
        user.SetEmailVerificationToken(token, DateTime.UtcNow.AddHours(1));

        await _context.SaveChangesAsync(cancellationToken);

        var frontendUrl = _configuration["FrontendUrl"] ?? "http://localhost:3000";
        var verificationLink = $"{frontendUrl}/shared/verify-email?token={token}&email={System.Web.HttpUtility.UrlEncode(user.Email)}";
        
        _logger.LogInformation("================================================");
        _logger.LogInformation("DEV ALERT: REGISTRATION EMAIL VERIFICATION LINK");
        _logger.LogInformation("Link: {VerificationLink}", verificationLink);
        _logger.LogInformation("================================================");

        return Ok(ApiResponse.Ok("Teacher registered successfully. Status is Pending verification."));
    }

    [HttpGet("{teacherId}/profile")]
    public async Task<IActionResult> GetProfile(Guid teacherId, CancellationToken cancellationToken)
    {
        var teacher = await _context.TeacherProfiles
            .Include(t => t.User)
            .FirstOrDefaultAsync(t => t.UserId == teacherId, cancellationToken);

        if (teacher == null) return NotFound("Teacher profile not found.");

        return Ok(ApiResponse<object>.Ok(new {
            teacher.User.FirstName,
            teacher.User.LastName,
            teacher.Bio,
            teacher.Qualifications,
            teacher.ExperienceYears,
            teacher.TeachingSkills,
            teacher.VerificationState,
            teacher.IsActive
        }, "Fetched profile."));
    }

    [HttpPatch("me/profile")]
    public async Task<IActionResult> UpdateMyProfile([FromBody] UpdateTeacherProfileRequest request, CancellationToken cancellationToken)
    {
        var userIdStr = HttpContext.User.FindFirst("sub")?.Value;
        if (!Guid.TryParse(userIdStr, out var userId))
            return Unauthorized();

        var teacher = await _context.TeacherProfiles
            .FirstOrDefaultAsync(t => t.UserId == userId, cancellationToken);
            
        if (teacher == null) return NotFound("Teacher profile not found.");

        teacher.UpdateProfile(
            request.Bio ?? teacher.Bio,
            request.Qualifications ?? teacher.Qualifications,
            request.ExperienceYears ?? teacher.ExperienceYears,
            request.TeachingSkills ?? teacher.TeachingSkills
        );

        await _context.SaveChangesAsync(cancellationToken);

        return Ok(ApiResponse.Ok("Teacher profile updated successfully."));
    }
}

public record RegisterTeacherRequest(
    string Email,
    string Password,
    string FirstName,
    string LastName
);

public record UpdateTeacherProfileRequest(
    string? Bio,
    string? Qualifications,
    int? ExperienceYears,
    string? TeachingSkills
);