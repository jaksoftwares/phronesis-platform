using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Phronesis.Application.Common.Interfaces;

namespace Phronesis.Api.Controllers.Administration;

[ApiController]
[Route("api/v1/admin/classes")]
[Authorize(Roles = "Admin")]
public class AdminClassesController : ControllerBase
{
    private readonly IApplicationDbContext _context;

    public AdminClassesController(IApplicationDbContext context)
    {
        _context = context;
    }

    [HttpGet("sessions")]
    public async Task<IActionResult> GetSessions()
    {
        var sessions = await _context.ClassSessions
            .Include(s => s.VirtualClass)
            .ThenInclude(vc => vc.Teacher)
            .OrderByDescending(s => s.StartTime)
            .Select(s => new
            {
                s.Id,
                s.Title,
                s.Status,
                s.StartTime,
                s.EndTime,
                s.MeetingLink, // Exposed so admin can shadow/audit
                ClassName = s.VirtualClass.Name,
                Teacher = new { s.VirtualClass.Teacher.FirstName, s.VirtualClass.Teacher.LastName, s.VirtualClass.Teacher.Email }
            })
            .ToListAsync();

        return Ok(new { data = sessions });
    }
}
