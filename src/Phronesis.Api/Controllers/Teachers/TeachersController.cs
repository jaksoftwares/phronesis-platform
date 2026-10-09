using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Phronesis.Application.Authentication;
using Phronesis.Application.Common.Interfaces;
using Phronesis.Domain.Identity;
using Phronesis.Domain.Users;
using Phronesis.Infrastructure.Authentication;
using Phronesis.Shared.Responses;

namespace Phronesis.Api.Controllers.Teachers;

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

    [HttpGet("me/dashboard")]
    public async Task<IActionResult> GetDashboard(CancellationToken cancellationToken)
    {
        var userIdStr = HttpContext.User.FindFirst("sub")?.Value;
        if (!Guid.TryParse(userIdStr, out var userId)) return Unauthorized();

        var teacher = await _context.TeacherProfiles
            .FirstOrDefaultAsync(t => t.UserId == userId, cancellationToken);
        if (teacher == null) return NotFound("Teacher profile not found.");

        var dbContext = _context as Phronesis.Infrastructure.Persistence.PhronesisDbContext;
        if (dbContext == null) return StatusCode(500, "Database context cast failed.");

        // 1. Calculate Active Students (distinct learners enrolled in teacher's classes)
        var classIds = await dbContext.Set<Phronesis.Domain.Tuition.VirtualClass>()
            .Where(vc => vc.TeacherId == userId)
            .Select(vc => vc.Id)
            .ToListAsync(cancellationToken);

        var activeStudents = await dbContext.Set<Phronesis.Domain.Tuition.ClassEnrollment>()
            .Where(ce => classIds.Contains(ce.VirtualClassId) && ce.Status == Phronesis.Domain.Tuition.EnrollmentStatus.Active)
            .Select(ce => ce.LearnerId)
            .Distinct()
            .CountAsync(cancellationToken);

        // 2. Pending Grading
        // Mocking pending grading for now, typically count AssessmentAttempts for this teacher's assessments
        var pendingGrading = 24; 

        // 3. Upcoming Classes
        var upcomingClasses = await dbContext.Set<Phronesis.Domain.Tuition.ClassSession>()
            .Include(cs => cs.VirtualClass)
            .Where(cs => classIds.Contains(cs.VirtualClassId) && cs.StartTime > DateTime.UtcNow.AddHours(-1))
            .OrderBy(cs => cs.StartTime)
            .Take(3)
            .Select(cs => new 
            {
                Id = cs.Id,
                Title = cs.VirtualClass.Name,
                Topic = cs.Title,
                Time = cs.StartTime.ToString("hh:mm tt"),
                Duration = (cs.EndTime - cs.StartTime).TotalMinutes + "m",
                Students = activeStudents, // approximate
                IsLive = cs.StartTime <= DateTime.UtcNow && cs.EndTime > DateTime.UtcNow
            })
            .ToListAsync(cancellationToken);

        var data = new
        {
            Metrics = new
            {
                ActiveStudents = activeStudents,
                HoursTaught = 38.5,
                PendingGrading = pendingGrading,
                AvgRating = 4.9
            },
            UpcomingClasses = upcomingClasses
        };

        return Ok(ApiResponse<object>.Ok(data, "Fetched dashboard data."));
    }

    [HttpGet("me/classes")]
    public async Task<IActionResult> GetMyClasses(CancellationToken cancellationToken)
    {
        var userIdStr = HttpContext.User.FindFirst("sub")?.Value;
        if (!Guid.TryParse(userIdStr, out var userId)) return Unauthorized();

        var teacher = await _context.TeacherProfiles
            .FirstOrDefaultAsync(t => t.UserId == userId, cancellationToken);
        if (teacher == null) return NotFound("Teacher profile not found.");

        var dbContext = _context as Phronesis.Infrastructure.Persistence.PhronesisDbContext;
        if (dbContext == null) return StatusCode(500, "Database context cast failed.");

        var classes = await dbContext.Set<Phronesis.Domain.Tuition.VirtualClass>()
            .Where(vc => vc.TeacherId == userId)
            .Select(vc => new 
            {
                Id = vc.Id,
                Name = vc.Name,
                Students = dbContext.Set<Phronesis.Domain.Tuition.ClassEnrollment>()
                    .Count(ce => ce.VirtualClassId == vc.Id && ce.Status == Phronesis.Domain.Tuition.EnrollmentStatus.Active),
                Schedule = "Mon/Wed 10:00 AM" // Mocked schedule for now, would typically parse schedules
            })
            .ToListAsync(cancellationToken);

        return Ok(ApiResponse<object>.Ok(classes, "Fetched teacher classes."));
    }

    [HttpGet("me/classes/{classId}")]
    public async Task<IActionResult> GetClassDetails(Guid classId, CancellationToken cancellationToken)
    {
        var userIdStr = HttpContext.User.FindFirst("sub")?.Value;
        if (!Guid.TryParse(userIdStr, out var userId)) return Unauthorized();

        var teacher = await _context.TeacherProfiles
            .FirstOrDefaultAsync(t => t.UserId == userId, cancellationToken);
        if (teacher == null) return NotFound("Teacher profile not found.");

        var dbContext = _context as Phronesis.Infrastructure.Persistence.PhronesisDbContext;
        if (dbContext == null) return StatusCode(500, "Database context cast failed.");

        var virtualClass = await dbContext.Set<Phronesis.Domain.Tuition.VirtualClass>()
            .FirstOrDefaultAsync(vc => vc.Id == classId && vc.TeacherId == userId, cancellationToken);

        if (virtualClass == null) return NotFound("Class not found or unauthorized.");

        var enrollments = await dbContext.Set<Phronesis.Domain.Tuition.ClassEnrollment>()
            .Include(ce => ce.Learner)
            .Where(ce => ce.VirtualClassId == classId && ce.Status == Phronesis.Domain.Tuition.EnrollmentStatus.Active)
            .Select(ce => new
            {
                EnrollmentId = ce.Id,
                LearnerId = ce.LearnerId,
                FirstName = ce.Learner.FirstName,
                LastName = ce.Learner.LastName,
                Email = ce.Learner.Email,
                RegistrationNumber = "N/A"
            })
            .ToListAsync(cancellationToken);

        var responseData = new
        {
            Id = virtualClass.Id,
            Name = virtualClass.Name,
            Schedule = "Mon/Wed 10:00 AM", // Mocked
            Students = enrollments
        };

        return Ok(ApiResponse<object>.Ok(responseData, "Fetched class details."));
    }

    [HttpGet("me/schedule")]
    public async Task<IActionResult> GetTeacherSchedule(CancellationToken cancellationToken)
    {
        var userIdStr = HttpContext.User.FindFirst("sub")?.Value;
        if (!Guid.TryParse(userIdStr, out var userId)) return Unauthorized();

        var dbContext = _context as Phronesis.Infrastructure.Persistence.PhronesisDbContext;
        if (dbContext == null) return StatusCode(500, "Database context cast failed.");

        var sessions = await dbContext.Set<Phronesis.Domain.Tuition.ClassSession>()
            .Include(cs => cs.VirtualClass)
            .Where(cs => cs.VirtualClass.TeacherId == userId)
            .OrderBy(cs => cs.StartTime)
            .Take(50)
            .Select(cs => new
            {
                id = cs.Id,
                title = cs.Title,
                type = "Class", // Can be customized later
                day = cs.StartTime.ToString("ddd"), // "Mon", "Tue", etc.
                time = cs.StartTime.ToString("HH:mm"),
                duration = (cs.EndTime - cs.StartTime).TotalHours,
                students = cs.VirtualClass.Enrollments.Count(e => e.Status == Phronesis.Domain.Tuition.EnrollmentStatus.Active)
            })
            .ToListAsync(cancellationToken);

        // Fallback to mock data if DB has no sessions to keep the calendar populated for demo
        if (!sessions.Any())
        {
            return Ok(ApiResponse<object>.Ok(new[]
            {
                new { id = "e1", title = "A-Level Mathematics", type = "Class", day = "Mon", time = "10:00", duration = 1.5, students = 24 },
                new { id = "e2", title = "O-Level Physics", type = "Class", day = "Tue", time = "13:00", duration = 1.0, students = 18 },
                new { id = "e3", title = "Advanced Chemistry", type = "Class", day = "Wed", time = "15:30", duration = 1.0, students = 12 },
                new { id = "e4", title = "1-on-1: Sarah Connor", type = "Tutoring", day = "Thu", time = "14:00", duration = 0.5, students = 1 },
                new { id = "e5", title = "Office Hours", type = "Availability", day = "Fri", time = "14:00", duration = 2.0, students = 0 },
            }, "Fetched mock teacher schedule."));
        }

        return Ok(ApiResponse<object>.Ok(sessions, "Fetched teacher schedule."));
    }

    [HttpGet("me/earnings")]
    public IActionResult GetTeacherEarnings()
    {
        var userIdStr = HttpContext.User.FindFirst("sub")?.Value;
        if (!Guid.TryParse(userIdStr, out var userId)) return Unauthorized();

        // Returning mocked financials for the prototype
        var response = new
        {
            Balance = 845.00m,
            YtdEarnings = 24650.00m,
            HourlyRate = 45.00m,
            Ledger = new[]
            {
                new { id = "tx1", date = "Sept 15, 2026", description = "Bi-Weekly Payout (Sept 1 - Sept 14)", hours = 42, rate = "$45/hr", amount = "$1,890.00", status = "Paid" },
                new { id = "tx2", date = "Aug 31, 2026", description = "Bi-Weekly Payout (Aug 15 - Aug 30)", hours = 38, rate = "$45/hr", amount = "$1,710.00", status = "Paid" },
                new { id = "tx3", date = "Aug 15, 2026", description = "Bi-Weekly Payout (Aug 1 - Aug 14)", hours = 45, rate = "$45/hr", amount = "$2,025.00", status = "Paid" }
            }
        };

        return Ok(ApiResponse<object>.Ok(response, "Fetched teacher earnings."));
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
