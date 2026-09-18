namespace Phronesis.Domain.Tuition;

public enum AttendanceStatus
{
    Absent = 0,
    Present = 1,
    Excused = 2
}

public class ClassAttendance
{
    public Guid Id { get; private set; }
    public Guid ClassSessionId { get; private set; }
    public Guid LearnerId { get; private set; }
    
    public AttendanceStatus Status { get; private set; }
    
    // Accumulating Duration Logic
    public DateTime? FirstJoinedAt { get; private set; }
    public DateTime? LastJoinedAt { get; private set; }
    public int TotalDurationMinutes { get; private set; }

    // Navigation Properties
    public ClassSession ClassSession { get; private set; } = null!;

    private ClassAttendance() { } // EF Core

    public ClassAttendance(Guid classSessionId, Guid learnerId)
    {
        Id = Guid.NewGuid();
        ClassSessionId = classSessionId;
        LearnerId = learnerId;
        Status = AttendanceStatus.Absent;
        TotalDurationMinutes = 0;
    }

    public void RecordJoin(DateTime joinTime)
    {
        Status = AttendanceStatus.Present;
        
        if (FirstJoinedAt == null)
        {
            FirstJoinedAt = joinTime;
        }
        
        LastJoinedAt = joinTime;
    }

    public void RecordLeave(DateTime leaveTime)
    {
        if (LastJoinedAt.HasValue)
        {
            // Calculate minutes since last join
            var duration = (int)(leaveTime - LastJoinedAt.Value).TotalMinutes;
            
            if (duration > 0)
            {
                TotalDurationMinutes += duration;
            }
            
            // Nullify LastJoinedAt so we don't double count if leave is called twice
            LastJoinedAt = null;
        }
    }
}
