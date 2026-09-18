using Phronesis.Domain.Common;
using Phronesis.Domain.Identity;

namespace Phronesis.Domain.Tuition;

public class TeacherAvailability : BaseEntity
{
    public Guid TeacherId { get; private set; }
    
    // If DayOfWeek is set, it's a recurring weekly slot. 
    // If SpecificDate is set, it's a one-off slot.
    public DayOfWeek? DayOfWeek { get; private set; }
    public DateTime? SpecificDate { get; private set; }
    
    public TimeSpan StartTime { get; private set; }
    public TimeSpan EndTime { get; private set; }
    
    public bool IsBooked { get; private set; }

    public User Teacher { get; private set; } = null!;

    private TeacherAvailability() { } // EF Core

    public TeacherAvailability(Guid teacherId, DayOfWeek? dayOfWeek, DateTime? specificDate, TimeSpan startTime, TimeSpan endTime)
    {
        if (endTime <= startTime)
            throw new ArgumentException("EndTime must be after StartTime.");

        if (dayOfWeek == null && specificDate == null)
            throw new ArgumentException("Must specify either DayOfWeek or SpecificDate.");

        TeacherId = teacherId;
        DayOfWeek = dayOfWeek;
        SpecificDate = specificDate?.ToUniversalTime().Date;
        StartTime = startTime;
        EndTime = endTime;
        IsBooked = false;
    }

    public void MarkAsBooked()
    {
        IsBooked = true;
    }
}
