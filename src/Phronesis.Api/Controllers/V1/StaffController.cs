using Microsoft.AspNetCore.Mvc;
using Phronesis.Application.Common.Interfaces;
using Phronesis.Domain.Identity;
using Phronesis.Domain.Organization;
using Phronesis.Application.Authentication;
using Phronesis.Infrastructure.Authentication;
using Phronesis.Shared.Responses;
using System.Security.Cryptography;

namespace Phronesis.Api.Controllers.V1;

[ApiController]
[Route("api/v1/[controller]")]
public class StaffController : ControllerBase
{
    private readonly IApplicationDbContext _context;
    private readonly IPasswordHasher _passwordHasher;

    public StaffController(IApplicationDbContext context, IPasswordHasher passwordHasher)
    {
        _context = context;
        _passwordHasher = passwordHasher;
    }

    [HttpPost("onboard")]
    [HasPermission(Permissions.Users.Create)]
    public async Task<IActionResult> OnboardStaff([FromBody] OnboardStaffRequest request, CancellationToken cancellationToken)
    {
        // Check if email already exists
        if (_context.Users.Any(u => u.Email == request.Email))
        {
            return BadRequest(ApiResponse.Failure("User with this email already exists."));
        }

        var role = await _context.Roles.FindAsync(new object[] { request.RoleId }, cancellationToken);
        if (role == null)
        {
            return BadRequest(ApiResponse.Failure("Specified role does not exist."));
        }

        // Generate a secure temporary password
        var temporaryPassword = GenerateTemporaryPassword();
        var passwordHash = _passwordHasher.Hash(temporaryPassword);

        var user = new User(request.Email, passwordHash, request.FirstName, request.LastName);
        user.RequirePasswordChange(); // MustChangePassword = true

        var staffProfile = new StaffProfile(
            user.Id,
            request.EmployeeId,
            request.Department,
            request.JobTitle,
            DateTime.UtcNow
        );

        var userRole = new UserRole(user.Id, role.Id);

        _context.Users.Add(user);
        _context.StaffProfiles.Add(staffProfile);
        _context.UserRoles.Add(userRole);

        // Atomic transaction handled by EF Core SaveChangesAsync
        await _context.SaveChangesAsync(cancellationToken);

        return Ok(ApiResponse<OnboardStaffResponse>.Ok(
            new OnboardStaffResponse(user.Id, temporaryPassword), 
            "Staff onboarded successfully. Provide the temporary password securely to the employee."
        ));
    }

    private string GenerateTemporaryPassword()
    {
        // Generate a random secure 12 character password
        var bytes = new byte[9];
        RandomNumberGenerator.Fill(bytes);
        return Convert.ToBase64String(bytes) + "A1!"; // Ensure complexity requirements
    }
}

public record OnboardStaffRequest(
    string Email,
    string FirstName,
    string LastName,
    string EmployeeId,
    Department Department,
    string JobTitle,
    Guid RoleId
);

public record OnboardStaffResponse(Guid UserId, string TemporaryPassword);
