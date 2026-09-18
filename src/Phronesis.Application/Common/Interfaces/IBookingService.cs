using Phronesis.Domain.Tuition;

namespace Phronesis.Application.Common.Interfaces;

public class AvailabilitySlotRequest
{
    public DayOfWeek? DayOfWeek { get; set; }
    public DateTime? SpecificDate { get; set; }
    public TimeSpan StartTime { get; set; }
    public TimeSpan EndTime { get; set; }
}

public class TimeSlot
{
    public DateTime StartTime { get; set; }
    public DateTime EndTime { get; set; }
}

public interface IBookingService
{
    Task<TeacherAvailability> SetAvailabilityAsync(Guid teacherId, AvailabilitySlotRequest request, CancellationToken cancellationToken = default);
    Task<IEnumerable<TimeSlot>> GetAvailableSlotsAsync(Guid teacherId, DateTime startDate, DateTime endDate, CancellationToken cancellationToken = default);
    Task<BookingRequest> RequestBookingAsync(Guid learnerId, Guid teacherId, Guid subjectId, DateTime startTime, DateTime endTime, CancellationToken cancellationToken = default);
    Task<bool> ApproveBookingAsync(Guid teacherId, Guid bookingRequestId, CancellationToken cancellationToken = default);
}
