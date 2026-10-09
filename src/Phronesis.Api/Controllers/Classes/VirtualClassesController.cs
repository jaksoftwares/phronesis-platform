using Microsoft.AspNetCore.Mvc;

using Microsoft.EntityFrameworkCore;
using Phronesis.Application.Common.Interfaces;
using Phronesis.Shared.Responses;

namespace Phronesis.Api.Controllers.Classes;

[ApiController]
[Route("api/v1")]
public class VirtualClassesController : ControllerBase
{
    private readonly IApplicationDbContext _context;

    public VirtualClassesController(IApplicationDbContext context)
    {
        _context = context;
    }
    [HttpGet("classes")]
    public IActionResult ListOfferings() => StatusCode(501);

    [HttpPost("classes")]
    public IActionResult CreateClass() => StatusCode(501);

    [HttpGet("classes/{classId}")]
    public IActionResult GetClass(string classId) => StatusCode(501);

    [HttpPatch("classes/{classId}")]
    public IActionResult UpdateClass(string classId) => StatusCode(501);

    [HttpPost("classes/{classId}/publish")]
    public IActionResult PublishClass(string classId) => StatusCode(501);

    [HttpPost("classes/{classId}/unpublish")]
    public IActionResult UnpublishClass(string classId) => StatusCode(501);

    [HttpGet("classes/{classId}/sessions")]
    public IActionResult ListSessions(string classId) => StatusCode(501);

    [HttpPost("classes/{classId}/sessions")]
    public IActionResult CreateSession(string classId) => StatusCode(501);

    [HttpGet("sessions/{sessionId}")]
    public IActionResult GetSession(string sessionId) => StatusCode(501);

    [HttpPatch("sessions/{sessionId}")]
    public IActionResult UpdateSession(string sessionId) => StatusCode(501);

    [HttpPost("sessions/{sessionId}/cancel")]
    public IActionResult CancelSession(string sessionId) => StatusCode(501);

    [HttpGet("classes/{classId}/availability")]
    public IActionResult AvailableSlots(string classId) => StatusCode(501);

    [HttpGet("sessions/{sessionId}/availability")]
    public IActionResult SessionAvailability(string sessionId) => StatusCode(501);

    [HttpPost("bookings")]
    public IActionResult CreateBooking() => StatusCode(501);

    [HttpGet("bookings/{bookingId}")]
    public IActionResult GetBooking(string bookingId) => StatusCode(501);

    [HttpGet("me/bookings")]
    public async Task<IActionResult> OwnBookings(CancellationToken cancellationToken)
    {
        // Mock returning upcoming sessions for the calendar
        var sessions = await _context.ClassSessions
            .Include(cs => cs.VirtualClass)
            .OrderBy(cs => cs.StartTime)
            .Take(10)
            .Select(cs => new
            {
                Id = cs.Id,
                Title = cs.Title,
                ClassName = cs.VirtualClass.Name,
                StartTime = cs.StartTime,
                EndTime = cs.EndTime,
                IsLive = cs.StartTime <= DateTime.UtcNow && cs.EndTime > DateTime.UtcNow
            })
            .ToListAsync(cancellationToken);

        return Ok(ApiResponse<object>.Ok(sessions));
    }

    [HttpPost("bookings/{bookingId}/confirm")]
    public IActionResult ConfirmBooking(string bookingId) => StatusCode(501);

    [HttpPost("bookings/{bookingId}/reschedule")]
    public IActionResult Reschedule(string bookingId) => StatusCode(501);

    [HttpPost("bookings/{bookingId}/cancel")]
    public IActionResult CancelBooking(string bookingId) => StatusCode(501);

    [HttpPost("sessions/{sessionId}/join")]
    public async Task<IActionResult> JoinSession(Guid sessionId, CancellationToken cancellationToken)
    {
        var session = await _context.ClassSessions
            .Include(cs => cs.VirtualClass)
            .FirstOrDefaultAsync(cs => cs.Id == sessionId, cancellationToken);
            
        if (session == null) return NotFound("Session not found.");
        
        return Ok(ApiResponse<object>.Ok(new {
            session.Id,
            session.Title,
            session.MeetingLink,
            HostName = "Instructor",
            IsLive = session.StartTime <= DateTime.UtcNow && session.EndTime > DateTime.UtcNow
        }));
    }

    [HttpGet("sessions/{sessionId}/classroom")]
    public IActionResult ClassroomState(string sessionId) => StatusCode(501);

    [HttpPost("sessions/{sessionId}/start")]
    public IActionResult StartSession(string sessionId) => StatusCode(501);

    [HttpPost("sessions/{sessionId}/end")]
    public IActionResult EndSession(string sessionId) => StatusCode(501);

    [HttpPost("sessions/{sessionId}/recording")]
    public IActionResult ConfigureRecording(string sessionId) => StatusCode(501);

    [HttpGet("sessions/{sessionId}/recording")]
    public IActionResult RecordingMetadata(string sessionId) => StatusCode(501);

    [HttpGet("sessions/{sessionId}/attendance")]
    public IActionResult AttendanceList(string sessionId) => StatusCode(501);

    [HttpPost("sessions/{sessionId}/attendance")]
    public IActionResult RecordAttendance(string sessionId) => StatusCode(501);

    [HttpPatch("sessions/{sessionId}/attendance/{attendanceId}")]
    public IActionResult CorrectAttendance(string sessionId, string attendanceId) => StatusCode(501);

    [HttpPost("sessions/{sessionId}/feedback")]
    public IActionResult SubmitFeedback(string sessionId) => StatusCode(501);

    [HttpGet("sessions/{sessionId}/feedback")]
    public IActionResult ViewFeedback(string sessionId) => StatusCode(501);

    [HttpPost("sessions/{sessionId}/notes")]
    public IActionResult SubmitNotes(string sessionId) => StatusCode(501);

    [HttpGet("sessions/{sessionId}/notes")]
    public IActionResult GetNotes(string sessionId) => StatusCode(501);
}

