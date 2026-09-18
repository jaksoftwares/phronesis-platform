using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Phronesis.Application.Common.Interfaces;
using Phronesis.Shared.Responses;

namespace Phronesis.Api.Controllers.V1;

[ApiController]
[Route("api/v1/notifications")]
[Authorize]
public class NotificationController : ControllerBase
{
    private readonly INotificationService _notificationService;

    public NotificationController(INotificationService notificationService)
    {
        _notificationService = notificationService;
    }

    private Guid GetUserId()
    {
        var userIdString = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
        if (!Guid.TryParse(userIdString, out var userId))
            throw new UnauthorizedAccessException("Invalid user token.");
        return userId;
    }

    [HttpGet]
    public async Task<IActionResult> GetNotifications([FromQuery] bool unreadOnly = false, CancellationToken cancellationToken = default)
    {
        try
        {
            var userId = GetUserId();
            var notifications = await _notificationService.GetUserNotificationsAsync(userId, unreadOnly, cancellationToken);
            return Ok(ApiResponse<object>.Ok(notifications));
        }
        catch (UnauthorizedAccessException ex) { return Forbid(ex.Message); }
        catch (Exception ex) { return BadRequest(new { Message = ex.Message }); }
    }

    [HttpPatch("{id}/read")]
    public async Task<IActionResult> MarkAsRead(Guid id, CancellationToken cancellationToken)
    {
        try
        {
            var userId = GetUserId();
            await _notificationService.MarkAsReadAsync(id, userId, cancellationToken);
            return Ok(ApiResponse<object>.Ok(null, "Notification marked as read."));
        }
        catch (UnauthorizedAccessException ex) { return Forbid(ex.Message); }
        catch (Exception ex) { return BadRequest(new { Message = ex.Message }); }
    }

    [HttpPatch("read-all")]
    public async Task<IActionResult> MarkAllAsRead(CancellationToken cancellationToken)
    {
        try
        {
            var userId = GetUserId();
            await _notificationService.MarkAllAsReadAsync(userId, cancellationToken);
            return Ok(ApiResponse<object>.Ok(null, "All notifications marked as read."));
        }
        catch (UnauthorizedAccessException ex) { return Forbid(ex.Message); }
        catch (Exception ex) { return BadRequest(new { Message = ex.Message }); }
    }

    [HttpGet("preferences")]
    public async Task<IActionResult> GetPreferences(CancellationToken cancellationToken)
    {
        try
        {
            var userId = GetUserId();
            var preferences = await _notificationService.GetUserPreferencesAsync(userId, cancellationToken);
            return Ok(ApiResponse<object>.Ok(preferences));
        }
        catch (UnauthorizedAccessException ex) { return Forbid(ex.Message); }
        catch (Exception ex) { return BadRequest(new { Message = ex.Message }); }
    }

    [HttpPut("preferences")]
    public async Task<IActionResult> UpdatePreference([FromBody] UpdatePreferenceDto dto, CancellationToken cancellationToken)
    {
        try
        {
            var userId = GetUserId();
            await _notificationService.UpdatePreferenceAsync(userId, dto.NotificationType, dto.EmailEnabled, dto.InAppEnabled, cancellationToken);
            return Ok(ApiResponse<object>.Ok(null, "Preference updated successfully."));
        }
        catch (UnauthorizedAccessException ex) { return Forbid(ex.Message); }
        catch (Exception ex) { return BadRequest(new { Message = ex.Message }); }
    }
}

public class UpdatePreferenceDto
{
    public string NotificationType { get; set; } = string.Empty;
    public bool EmailEnabled { get; set; }
    public bool InAppEnabled { get; set; }
}
