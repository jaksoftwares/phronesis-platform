using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Phronesis.Application.Common.Interfaces;
using Phronesis.Shared.Responses;

namespace Phronesis.Api.Controllers.V1;

[ApiController]
[Route("api/v1/collaboration")]
[Authorize]
public class CollaborationController : ControllerBase
{
    private readonly ICollaborationService _collaborationService;

    public CollaborationController(ICollaborationService collaborationService)
    {
        _collaborationService = collaborationService;
    }

    private Guid GetUserId()
    {
        var userIdString = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
        if (!Guid.TryParse(userIdString, out var userId))
            throw new UnauthorizedAccessException("Invalid user token.");
        return userId;
    }

    // --- Resources ---

    [HttpPost("classes/{classId}/resources")]
    [Authorize(Policy = "RequireTeacher")]
    public async Task<IActionResult> UploadResource(Guid classId, [FromBody] UploadResourceDto dto, CancellationToken cancellationToken)
    {
        try
        {
            var userId = GetUserId();
            var resource = await _collaborationService.UploadClassResourceAsync(
                classId, userId, dto.Title, dto.Description, dto.FileUrl, dto.MimeType, dto.SizeInBytes, dto.ClassSessionId, cancellationToken);
            
            return Ok(ApiResponse<object>.Ok(resource, "Resource uploaded successfully."));
        }
        catch (UnauthorizedAccessException ex) { return Forbid(ex.Message); }
        catch (Exception ex) { return BadRequest(new { Message = ex.Message }); }
    }

    [HttpGet("classes/{classId}/resources")]
    public async Task<IActionResult> GetResources(Guid classId, CancellationToken cancellationToken)
    {
        try
        {
            var userId = GetUserId();
            var resources = await _collaborationService.GetClassResourcesAsync(classId, userId, cancellationToken);
            return Ok(ApiResponse<object>.Ok(resources));
        }
        catch (UnauthorizedAccessException ex) { return Forbid(ex.Message); }
        catch (Exception ex) { return BadRequest(new { Message = ex.Message }); }
    }

    [HttpDelete("resources/{resourceId}")]
    [Authorize(Policy = "RequireTeacher")]
    public async Task<IActionResult> DeleteResource(Guid resourceId, CancellationToken cancellationToken)
    {
        try
        {
            var userId = GetUserId();
            await _collaborationService.DeleteClassResourceAsync(resourceId, userId, cancellationToken);
            return Ok(ApiResponse<object>.Ok(null, "Resource deleted successfully."));
        }
        catch (UnauthorizedAccessException ex) { return Forbid(ex.Message); }
        catch (Exception ex) { return BadRequest(new { Message = ex.Message }); }
    }

    // --- Discussions ---

    [HttpPost("classes/{classId}/discussions")]
    public async Task<IActionResult> CreateDiscussion(Guid classId, [FromBody] CreateDiscussionDto dto, CancellationToken cancellationToken)
    {
        try
        {
            var userId = GetUserId();
            var discussion = await _collaborationService.CreateDiscussionAsync(classId, userId, dto.Title, dto.Content, cancellationToken);
            return Ok(ApiResponse<object>.Ok(discussion, "Discussion created successfully."));
        }
        catch (UnauthorizedAccessException ex) { return Forbid(ex.Message); }
        catch (Exception ex) { return BadRequest(new { Message = ex.Message }); }
    }

    [HttpGet("classes/{classId}/discussions")]
    public async Task<IActionResult> GetDiscussions(Guid classId, CancellationToken cancellationToken)
    {
        try
        {
            var userId = GetUserId();
            var discussions = await _collaborationService.GetClassDiscussionsAsync(classId, userId, cancellationToken);
            return Ok(ApiResponse<object>.Ok(discussions));
        }
        catch (UnauthorizedAccessException ex) { return Forbid(ex.Message); }
        catch (Exception ex) { return BadRequest(new { Message = ex.Message }); }
    }

    [HttpGet("discussions/{discussionId}")]
    public async Task<IActionResult> GetDiscussion(Guid discussionId, CancellationToken cancellationToken)
    {
        try
        {
            var userId = GetUserId();
            var discussion = await _collaborationService.GetDiscussionByIdAsync(discussionId, userId, cancellationToken);
            if (discussion == null) return NotFound("Discussion not found.");
            return Ok(ApiResponse<object>.Ok(discussion));
        }
        catch (UnauthorizedAccessException ex) { return Forbid(ex.Message); }
        catch (Exception ex) { return BadRequest(new { Message = ex.Message }); }
    }

    [HttpPatch("discussions/{discussionId}/resolve")]
    [Authorize(Policy = "RequireTeacher")]
    public async Task<IActionResult> ResolveDiscussion(Guid discussionId, CancellationToken cancellationToken)
    {
        try
        {
            var userId = GetUserId();
            await _collaborationService.ResolveDiscussionAsync(discussionId, userId, cancellationToken);
            return Ok(ApiResponse<object>.Ok(null, "Discussion resolved successfully."));
        }
        catch (UnauthorizedAccessException ex) { return Forbid(ex.Message); }
        catch (Exception ex) { return BadRequest(new { Message = ex.Message }); }
    }

    [HttpPost("discussions/{discussionId}/replies")]
    public async Task<IActionResult> AddReply(Guid discussionId, [FromBody] AddReplyDto dto, CancellationToken cancellationToken)
    {
        try
        {
            var userId = GetUserId();
            var reply = await _collaborationService.AddReplyAsync(discussionId, userId, dto.Content, cancellationToken);
            return Ok(ApiResponse<object>.Ok(reply, "Reply added successfully."));
        }
        catch (UnauthorizedAccessException ex) { return Forbid(ex.Message); }
        catch (Exception ex) { return BadRequest(new { Message = ex.Message }); }
    }

    [HttpPatch("replies/{replyId}/endorse")]
    [Authorize(Policy = "RequireTeacher")]
    public async Task<IActionResult> EndorseReply(Guid replyId, CancellationToken cancellationToken)
    {
        try
        {
            var userId = GetUserId();
            await _collaborationService.EndorseReplyAsync(replyId, userId, cancellationToken);
            return Ok(ApiResponse<object>.Ok(null, "Reply endorsed successfully."));
        }
        catch (UnauthorizedAccessException ex) { return Forbid(ex.Message); }
        catch (Exception ex) { return BadRequest(new { Message = ex.Message }); }
    }

    [HttpPatch("replies/{replyId}/revoke-endorsement")]
    [Authorize(Policy = "RequireTeacher")]
    public async Task<IActionResult> RevokeEndorsement(Guid replyId, CancellationToken cancellationToken)
    {
        try
        {
            var userId = GetUserId();
            await _collaborationService.RevokeEndorsementAsync(replyId, userId, cancellationToken);
            return Ok(ApiResponse<object>.Ok(null, "Endorsement revoked successfully."));
        }
        catch (UnauthorizedAccessException ex) { return Forbid(ex.Message); }
        catch (Exception ex) { return BadRequest(new { Message = ex.Message }); }
    }
}

public class UploadResourceDto
{
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string FileUrl { get; set; } = string.Empty;
    public string MimeType { get; set; } = string.Empty;
    public long SizeInBytes { get; set; }
    public Guid? ClassSessionId { get; set; }
}

public class CreateDiscussionDto
{
    public string Title { get; set; } = string.Empty;
    public string Content { get; set; } = string.Empty;
}

public class AddReplyDto
{
    public string Content { get; set; } = string.Empty;
}
