namespace Phronesis.Application.Common.Interfaces;

public class MeetingDetails
{
    public string MeetingId { get; set; } = string.Empty;
    public string MeetingPassword { get; set; } = string.Empty;
    public string JoinUrl { get; set; } = string.Empty;
    public string HostUrl { get; set; } = string.Empty;
}

public interface IVideoMeetingProvider
{
    Task<MeetingDetails> CreateMeetingAsync(string title, DateTime startTime, int durationMinutes, CancellationToken cancellationToken = default);
    Task<bool> DeleteMeetingAsync(string meetingId, CancellationToken cancellationToken = default);
}
