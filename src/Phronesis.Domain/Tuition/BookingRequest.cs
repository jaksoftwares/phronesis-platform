using Phronesis.Domain.Academic;
using Phronesis.Domain.Common;
using Phronesis.Domain.Identity;

namespace Phronesis.Domain.Tuition;

public enum BookingStatus
{
    Pending,
    PendingPayment,
    Approved,
    Rejected,
    Cancelled
}

public class BookingRequest : BaseEntity
{
    public Guid LearnerId { get; private set; }
    public Guid TeacherId { get; private set; }
    public Guid SubjectId { get; private set; }
    
    public DateTime RequestedStartTime { get; private set; }
    public DateTime RequestedEndTime { get; private set; }
    
    public BookingStatus Status { get; private set; }
    public string? TeacherNotes { get; private set; }

    public User Learner { get; private set; } = null!;
    public User Teacher { get; private set; } = null!;
    public Subject Subject { get; private set; } = null!;

    private BookingRequest() { } // EF Core

    public BookingRequest(Guid learnerId, Guid teacherId, Guid subjectId, DateTime startTime, DateTime endTime)
    {
        LearnerId = learnerId;
        TeacherId = teacherId;
        SubjectId = subjectId;
        RequestedStartTime = startTime.ToUniversalTime();
        RequestedEndTime = endTime.ToUniversalTime();
        Status = BookingStatus.Pending;
    }

    public void RequirePayment()
    {
        Status = BookingStatus.PendingPayment;
    }

    public void Approve()
    {
        Status = BookingStatus.Approved;
    }

    public void Reject(string notes)
    {
        Status = BookingStatus.Rejected;
        TeacherNotes = notes;
    }
}
