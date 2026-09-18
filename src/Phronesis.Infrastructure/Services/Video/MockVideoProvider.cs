using Phronesis.Application.Common.Interfaces;

namespace Phronesis.Infrastructure.Services.Video;

public class MockVideoProvider : IVideoMeetingProvider
{
    public Task<MeetingDetails> CreateMeetingAsync(string title, DateTime startTime, int durationMinutes, CancellationToken cancellationToken = default)
    {
        var meetingId = Guid.NewGuid().ToString("N").Substring(0, 10);
        
        var details = new MeetingDetails
        {
            MeetingId = meetingId,
            MeetingPassword = "mock-password-123",
            JoinUrl = $"https://phronesis.meet/join/{meetingId}",
            HostUrl = $"https://phronesis.meet/host/{meetingId}?token=mock_admin_token"
        };

        return Task.FromResult(details);
    }

    public Task<bool> DeleteMeetingAsync(string meetingId, CancellationToken cancellationToken = default)
    {
        // Mock cleanup
        return Task.FromResult(true);
    }
}
