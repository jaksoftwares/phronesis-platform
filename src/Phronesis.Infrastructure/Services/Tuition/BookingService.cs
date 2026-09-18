using Microsoft.EntityFrameworkCore;
using Phronesis.Application.Common.Interfaces;
using Phronesis.Domain.Tuition;

namespace Phronesis.Infrastructure.Services.Tuition;

public class BookingService : IBookingService
{
    private readonly IApplicationDbContext _context;
    private readonly ITuitionService _tuitionService;
    private readonly IVideoMeetingProvider _videoProvider;

    public BookingService(IApplicationDbContext context, ITuitionService tuitionService, IVideoMeetingProvider videoProvider)
    {
        _context = context;
        _tuitionService = tuitionService;
        _videoProvider = videoProvider;
    }

    public async Task<TeacherAvailability> SetAvailabilityAsync(Guid teacherId, AvailabilitySlotRequest request, CancellationToken cancellationToken = default)
    {
        var availability = new TeacherAvailability(
            teacherId,
            request.DayOfWeek,
            request.SpecificDate,
            request.StartTime,
            request.EndTime
        );

        _context.TeacherAvailabilities.Add(availability);
        await _context.SaveChangesAsync(cancellationToken);

        return availability;
    }

    public async Task<IEnumerable<TimeSlot>> GetAvailableSlotsAsync(Guid teacherId, DateTime startDate, DateTime endDate, CancellationToken cancellationToken = default)
    {
        var availabilities = await _context.TeacherAvailabilities
            .Where(ta => ta.TeacherId == teacherId && !ta.IsBooked)
            .ToListAsync(cancellationToken);

        var existingSessions = await _context.ClassSessions
            .Include(cs => cs.VirtualClass)
            .Where(cs => cs.VirtualClass.TeacherId == teacherId 
                         && cs.StartTime >= startDate 
                         && cs.EndTime <= endDate 
                         && cs.Status != SessionStatus.Cancelled)
            .ToListAsync(cancellationToken);

        var freeSlots = new List<TimeSlot>();
        var currentDate = startDate.Date;
        var end = endDate.Date;

        while (currentDate <= end)
        {
            var dayOfWeek = currentDate.DayOfWeek;
            
            var dailyAvailabilities = availabilities
                .Where(ta => ta.DayOfWeek == dayOfWeek || (ta.SpecificDate.HasValue && ta.SpecificDate.Value.Date == currentDate))
                .ToList();

            foreach (var block in dailyAvailabilities)
            {
                var blockStart = currentDate.Add(block.StartTime);
                var blockEnd = currentDate.Add(block.EndTime);
                
                var overlappingSessions = existingSessions
                    .Where(cs => cs.StartTime < blockEnd && cs.EndTime > blockStart)
                    .OrderBy(cs => cs.StartTime)
                    .ToList();

                var currentSlotStart = blockStart;

                foreach (var session in overlappingSessions)
                {
                    // If there's space before the session (plus 15 min buffer)
                    if (currentSlotStart.AddMinutes(15) <= session.StartTime)
                    {
                        freeSlots.Add(new TimeSlot { StartTime = currentSlotStart, EndTime = session.StartTime.AddMinutes(-15) });
                    }
                    currentSlotStart = session.EndTime.AddMinutes(15);
                }

                // Remaining time after the last session
                if (currentSlotStart < blockEnd)
                {
                    freeSlots.Add(new TimeSlot { StartTime = currentSlotStart, EndTime = blockEnd });
                }
            }

            currentDate = currentDate.AddDays(1);
        }

        return freeSlots;
    }

    public async Task<BookingRequest> RequestBookingAsync(Guid learnerId, Guid teacherId, Guid subjectId, DateTime startTime, DateTime endTime, CancellationToken cancellationToken = default)
    {
        // Prevent double booking at request time
        var conflictingSession = await _context.ClassSessions
            .Include(cs => cs.VirtualClass)
            .AnyAsync(cs => cs.VirtualClass.TeacherId == teacherId 
                         && cs.StartTime < endTime 
                         && cs.EndTime > startTime 
                         && cs.Status != SessionStatus.Cancelled, cancellationToken);

        if (conflictingSession)
            throw new InvalidOperationException("This time slot is no longer available.");

        var request = new BookingRequest(learnerId, teacherId, subjectId, startTime, endTime);
        _context.BookingRequests.Add(request);
        await _context.SaveChangesAsync(cancellationToken);

        return request;
    }

    public async Task<bool> ApproveBookingAsync(Guid teacherId, Guid bookingRequestId, CancellationToken cancellationToken = default)
    {
        var request = await _context.BookingRequests
            .Include(br => br.Learner)
            .Include(br => br.Teacher)
            .FirstOrDefaultAsync(br => br.Id == bookingRequestId && br.TeacherId == teacherId, cancellationToken);

        if (request == null)
            throw new ArgumentException("Booking request not found.");

        if (request.Status != BookingStatus.Pending && request.Status != BookingStatus.PendingPayment)
            throw new InvalidOperationException($"Cannot approve a request in {request.Status} status.");

        // Create VirtualClass for this 1-on-1 if it doesn't exist
        var vClass = await _context.VirtualClasses
            .Include(vc => vc.Enrollments)
            .FirstOrDefaultAsync(vc => vc.TeacherId == teacherId 
                                       && vc.SubjectId == request.SubjectId 
                                       && vc.ClassType == ClassType.OneOnOne 
                                       && vc.Enrollments.Any(e => e.LearnerId == request.LearnerId), cancellationToken);

        if (vClass == null)
        {
            var createRequest = new CreateClassRequest
            {
                Name = $"1-on-1: {request.Teacher.FirstName} & {request.Learner.FirstName}",
                Description = "Private Tutoring Session",
                ClassType = ClassType.OneOnOne,
                MaxCapacity = 1,
                TeacherId = request.TeacherId,
                SubjectId = request.SubjectId,
                IsSubscriptionIncluded = false, // Configurable later
                Price = 0 // Assuming free for MVP or paid independently
            };
            vClass = await _tuitionService.CreateClassAsync(createRequest, cancellationToken);
            await _tuitionService.EnrollLearnerAsync(request.LearnerId, vClass.Id, cancellationToken);
        }

        // Generate the Session
        var session = new ClassSession(vClass.Id, "Private Tutoring", request.RequestedStartTime, request.RequestedEndTime);
        
        // Generate Video Meeting automatically
        var duration = (int)(request.RequestedEndTime - request.RequestedStartTime).TotalMinutes;
        var meeting = await _videoProvider.CreateMeetingAsync(session.Title, session.StartTime, duration, cancellationToken);
        
        session.AttachVideoMeeting(meeting.MeetingId, meeting.MeetingPassword, meeting.JoinUrl, meeting.HostUrl);

        _context.ClassSessions.Add(session);

        request.Approve();
        await _context.SaveChangesAsync(cancellationToken);

        return true;
    }
}
