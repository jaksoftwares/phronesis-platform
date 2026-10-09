using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Phronesis.Application.Common.Interfaces;
using Phronesis.Shared.Responses;

namespace Phronesis.Api.Controllers.Communication;

[ApiController]
[Route("api/v1/communication")]
[Authorize]
public class MessagesController : ControllerBase
{
    public MessagesController()
    {
    }

    [HttpGet("guardian/inbox")]
    [AllowAnonymous] // MVP simplification
    public async Task<IActionResult> GetGuardianInbox(CancellationToken cancellationToken)
    {
        // Mock inbox contacts for Guardian
        var contacts = new[]
        {
            new { id = "1", name = "Mr. Davis", role = "Mathematics Teacher", child = "Alex Mercer", unread = 2, lastMsg = "Alex did great on the recent algebra quiz!" },
            new { id = "2", name = "Mrs. Smith", role = "Physics Teacher", child = "Alex Mercer", unread = 0, lastMsg = "Please remind Alex to submit the lab report." },
            new { id = "3", name = "Ms. Johnson", role = "Algebra I Teacher", child = "Mia Mercer", unread = 0, lastMsg = "Thanks for checking in." }
        };

        return Ok(ApiResponse<object>.Ok(contacts, "Inbox fetched successfully."));
    }

    [HttpGet("messages/{contactId}")]
    [AllowAnonymous] // MVP simplification
    public async Task<IActionResult> GetMessagesHistory(string contactId, CancellationToken cancellationToken)
    {
        // Mock chat history based on the contact ID
        var messages = new[]
        {
            new { id = 1, senderId = contactId, content = "Hello! I wanted to give you a quick update. Alex did great on the recent algebra quiz. He scored a 92%.", timestamp = "4:30 PM", isSender = false },
            new { id = 2, senderId = "me", content = "That's wonderful news! Thank you for letting me know. We worked on those equations all weekend.", timestamp = "5:15 PM", isSender = true },
            new { id = 3, senderId = contactId, content = "The practice clearly paid off! Let me know if you need any additional resources for the upcoming mid-term.", timestamp = "9:00 AM", isSender = false }
        };

        return Ok(ApiResponse<object>.Ok(messages, "Messages fetched successfully."));
    }

    [HttpPost("messages")]
    [AllowAnonymous] // MVP simplification
    public async Task<IActionResult> SendMessage([FromBody] SendMessageDto request, CancellationToken cancellationToken)
    {
        return Ok(ApiResponse<object>.Ok(new { success = true }, "Message sent successfully."));
    }
}

public class SendMessageDto
{
    public string ReceiverId { get; set; } = string.Empty;
    public string Content { get; set; } = string.Empty;
}
