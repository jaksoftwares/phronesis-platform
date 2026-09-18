using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Phronesis.Application.Common.Interfaces;

namespace Phronesis.Api.Controllers.V1;

[ApiController]
[Route("api/v1/[controller]")]
[Authorize]
public class BookingController : ControllerBase
{
    private readonly IBookingService _bookingService;

    public BookingController(IBookingService bookingService)
    {
        _bookingService = bookingService;
    }

    [HttpPost("availability")]
    [Authorize(Policy = "RequireTeacher")]
    public async Task<IActionResult> SetAvailability([FromBody] AvailabilitySlotRequest request, CancellationToken cancellationToken)
    {
        var userIdString = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
        if (!Guid.TryParse(userIdString, out var teacherId))
            return Unauthorized();

        try
        {
            var result = await _bookingService.SetAvailabilityAsync(teacherId, request, cancellationToken);
            return Ok(result);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { Message = ex.Message });
        }
    }

    [HttpGet("teachers/{teacherId}/slots")]
    public async Task<IActionResult> GetAvailableSlots(Guid teacherId, [FromQuery] DateTime startDate, [FromQuery] DateTime endDate, CancellationToken cancellationToken)
    {
        var slots = await _bookingService.GetAvailableSlotsAsync(teacherId, startDate, endDate, cancellationToken);
        return Ok(slots);
    }

    [HttpPost("request")]
    public async Task<IActionResult> RequestBooking([FromBody] CreateBookingDto dto, CancellationToken cancellationToken)
    {
        var userIdString = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
        if (!Guid.TryParse(userIdString, out var learnerId))
            return Unauthorized();

        try
        {
            var request = await _bookingService.RequestBookingAsync(learnerId, dto.TeacherId, dto.SubjectId, dto.StartTime, dto.EndTime, cancellationToken);
            return Ok(new { Message = "Booking requested successfully.", RequestId = request.Id });
        }
        catch (InvalidOperationException ex)
        {
            return Conflict(new { Message = ex.Message });
        }
    }

    [HttpPost("requests/{requestId}/approve")]
    [Authorize(Policy = "RequireTeacher")]
    public async Task<IActionResult> ApproveBooking(Guid requestId, CancellationToken cancellationToken)
    {
        var userIdString = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
        if (!Guid.TryParse(userIdString, out var teacherId))
            return Unauthorized();

        try
        {
            var success = await _bookingService.ApproveBookingAsync(teacherId, requestId, cancellationToken);
            return Ok(new { Success = success, Message = "Booking approved and session generated." });
        }
        catch (ArgumentException ex)
        {
            return NotFound(new { Message = ex.Message });
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { Message = ex.Message });
        }
    }
}

public class CreateBookingDto
{
    public Guid TeacherId { get; set; }
    public Guid SubjectId { get; set; }
    public DateTime StartTime { get; set; }
    public DateTime EndTime { get; set; }
}
