using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Phronesis.Application.Common.Interfaces;
using Phronesis.Shared.Responses;

namespace Phronesis.Api.Controllers.V1;

[ApiController]
[Route("api/v1/support")]
[Authorize]
public class SupportController : ControllerBase
{
    private readonly IHelpdeskService _helpdeskService;

    public SupportController(IHelpdeskService helpdeskService)
    {
        _helpdeskService = helpdeskService;
    }

    private Guid GetUserId()
    {
        var userIdString = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
        if (!Guid.TryParse(userIdString, out var userId))
            throw new UnauthorizedAccessException("Invalid user token.");
        return userId;
    }

    [HttpPost("tickets")]
    public async Task<IActionResult> CreateTicket([FromBody] CreateTicketDto dto, CancellationToken cancellationToken)
    {
        try
        {
            var userId = GetUserId();
            var ticket = await _helpdeskService.CreateTicketAsync(userId, dto.Category, dto.Subject, dto.Description, dto.Priority, cancellationToken);
            return Ok(ApiResponse<object>.Ok(ticket, "Ticket created successfully."));
        }
        catch (UnauthorizedAccessException ex) { return Forbid(ex.Message); }
        catch (Exception ex) { return BadRequest(new { Message = ex.Message }); }
    }

    [HttpGet("tickets")]
    public async Task<IActionResult> GetMyTickets(CancellationToken cancellationToken)
    {
        try
        {
            var userId = GetUserId();
            var tickets = await _helpdeskService.GetUserTicketsAsync(userId, cancellationToken);
            return Ok(ApiResponse<object>.Ok(tickets));
        }
        catch (UnauthorizedAccessException ex) { return Forbid(ex.Message); }
        catch (Exception ex) { return BadRequest(new { Message = ex.Message }); }
    }

    [HttpGet("tickets/all-open")]
    [Authorize(Policy = "RequireAdmin")] // Restrict to admin/support staff
    public async Task<IActionResult> GetAllOpenTickets(CancellationToken cancellationToken)
    {
        try
        {
            var tickets = await _helpdeskService.GetAllOpenTicketsAsync(cancellationToken);
            return Ok(ApiResponse<object>.Ok(tickets));
        }
        catch (Exception ex) { return BadRequest(new { Message = ex.Message }); }
    }

    [HttpGet("tickets/{id}")]
    public async Task<IActionResult> GetTicket(Guid id, CancellationToken cancellationToken)
    {
        try
        {
            var userId = GetUserId();
            // TODO: In a real app, an admin shouldn't be restricted by passing userId.
            var ticket = await _helpdeskService.GetTicketByIdAsync(id, userId, cancellationToken);
            if (ticket == null) return NotFound(ApiResponse<object>.Failure("Ticket not found."));

            return Ok(ApiResponse<object>.Ok(ticket));
        }
        catch (UnauthorizedAccessException ex) { return Forbid(ex.Message); }
        catch (Exception ex) { return BadRequest(new { Message = ex.Message }); }
    }

    [HttpPost("tickets/{id}/messages")]
    public async Task<IActionResult> AddMessage(Guid id, [FromBody] AddMessageDto dto, CancellationToken cancellationToken)
    {
        try
        {
            var userId = GetUserId();
            var message = await _helpdeskService.AddMessageAsync(id, userId, dto.Content, cancellationToken);
            return Ok(ApiResponse<object>.Ok(message, "Message added successfully."));
        }
        catch (UnauthorizedAccessException ex) { return Forbid(ex.Message); }
        catch (ArgumentException ex) { return NotFound(ApiResponse<object>.Failure(ex.Message)); }
        catch (Exception ex) { return BadRequest(new { Message = ex.Message }); }
    }

    [HttpPatch("tickets/{id}/status")]
    [Authorize(Policy = "RequireAdmin")]
    public async Task<IActionResult> UpdateStatus(Guid id, [FromBody] UpdateStatusDto dto, CancellationToken cancellationToken)
    {
        try
        {
            await _helpdeskService.UpdateTicketStatusAsync(id, dto.Status, cancellationToken);
            return Ok(ApiResponse<object>.Ok(null, "Ticket status updated."));
        }
        catch (ArgumentException ex) { return NotFound(ApiResponse<object>.Failure(ex.Message)); }
        catch (Exception ex) { return BadRequest(new { Message = ex.Message }); }
    }
}

public class CreateTicketDto
{
    public string Category { get; set; } = string.Empty;
    public string Subject { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Priority { get; set; } = string.Empty;
}

public class AddMessageDto
{
    public string Content { get; set; } = string.Empty;
}

public class UpdateStatusDto
{
    public string Status { get; set; } = string.Empty;
}
