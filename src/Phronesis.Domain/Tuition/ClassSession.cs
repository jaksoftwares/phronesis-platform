using Phronesis.Domain.Common;

namespace Phronesis.Domain.Tuition;

public enum SessionStatus
{
    Scheduled,
    InProgress,
    Completed,
    Cancelled
}

public class ClassSession : BaseEntity
{
    public Guid VirtualClassId { get; private set; }
    public string Title { get; private set; }
    
    // Stored in UTC
    public DateTime StartTime { get; private set; }
    public DateTime EndTime { get; private set; }
    
    public SessionStatus Status { get; private set; }
    
    // Video Integration
    public string? MeetingId { get; private set; }
    public string? MeetingPassword { get; private set; }
    public string? MeetingLink { get; private set; } // The Join URL for Learners
    public string? HostUrl { get; private set; } // The Host URL for Teachers

    // Post-Class Artifacts
    public string? RecordingUrl { get; private set; }
    public string? TeacherNotes { get; private set; }

    public VirtualClass VirtualClass { get; private set; } = null!;

    // Navigation Properties
    public ICollection<ClassAttendance> Attendances { get; private set; } = new List<ClassAttendance>();
    public ICollection<SessionFeedback> Feedbacks { get; private set; } = new List<SessionFeedback>();

    private ClassSession() { } // EF Core

    public ClassSession(Guid virtualClassId, string title, DateTime startTime, DateTime endTime)
    {
        if (endTime <= startTime)
            throw new ArgumentException("EndTime must be after StartTime.");

        VirtualClassId = virtualClassId;
        Title = title;
        StartTime = startTime.ToUniversalTime();
        EndTime = endTime.ToUniversalTime();
        Status = SessionStatus.Scheduled;
    }

    public void AttachVideoMeeting(string meetingId, string meetingPassword, string joinUrl, string hostUrl)
    {
        MeetingId = meetingId;
        MeetingPassword = meetingPassword;
        MeetingLink = joinUrl;
        HostUrl = hostUrl;
    }

    public void StartSession()
    {
        Status = SessionStatus.InProgress;
    }

    public void CompleteSession(string? recordingUrl, string? teacherNotes)
    {
        Status = SessionStatus.Completed;
        RecordingUrl = recordingUrl;
        TeacherNotes = teacherNotes;
    }

    public void CancelSession()
    {
        Status = SessionStatus.Cancelled;
    }
}
