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
    
    // Will be populated in M23
    public string? MeetingLink { get; private set; }

    public VirtualClass VirtualClass { get; private set; } = null!;

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

    public void StartSession(string meetingLink)
    {
        Status = SessionStatus.InProgress;
        MeetingLink = meetingLink;
    }

    public void CompleteSession()
    {
        Status = SessionStatus.Completed;
    }

    public void CancelSession()
    {
        Status = SessionStatus.Cancelled;
    }
}
