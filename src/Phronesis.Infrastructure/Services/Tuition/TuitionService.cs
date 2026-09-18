using Microsoft.EntityFrameworkCore;
using Phronesis.Application.Common.Interfaces;
using Phronesis.Domain.Tuition;

namespace Phronesis.Infrastructure.Services.Tuition;

public class TuitionService : ITuitionService
{
    private readonly IApplicationDbContext _context;
    private readonly ISubscriptionService _subscriptionService;
    private readonly IVideoMeetingProvider _videoProvider;

    public TuitionService(IApplicationDbContext context, ISubscriptionService subscriptionService, IVideoMeetingProvider videoProvider)
    {
        _context = context;
        _subscriptionService = subscriptionService;
        _videoProvider = videoProvider;
    }

    public async Task<VirtualClass> CreateClassAsync(CreateClassRequest request, CancellationToken cancellationToken = default)
    {
        var vClass = new VirtualClass(
            request.Name,
            request.Description,
            request.ClassType,
            request.MaxCapacity,
            request.TeacherId,
            request.SubjectId,
            request.IsSubscriptionIncluded,
            request.Price
        );

        _context.VirtualClasses.Add(vClass);
        await _context.SaveChangesAsync(cancellationToken);
        return vClass;
    }

    public async Task<ClassSession> CreateSessionAsync(Guid classId, string title, DateTime startTime, DateTime endTime, CancellationToken cancellationToken = default)
    {
        var vClass = await _context.VirtualClasses.FindAsync(new object[] { classId }, cancellationToken);
        if (vClass == null || !vClass.IsActive)
            throw new ArgumentException("Class not found or inactive.");

        var session = new ClassSession(classId, title, startTime, endTime);

        var duration = (int)(endTime - startTime).TotalMinutes;
        var meeting = await _videoProvider.CreateMeetingAsync(session.Title, session.StartTime, duration, cancellationToken);
        
        session.AttachVideoMeeting(meeting.MeetingId, meeting.MeetingPassword, meeting.JoinUrl, meeting.HostUrl);

        _context.ClassSessions.Add(session);
        await _context.SaveChangesAsync(cancellationToken);

        return session;
    }

    public async Task<bool> EnrollLearnerAsync(Guid learnerId, Guid classId, CancellationToken cancellationToken = default)
    {
        var vClass = await _context.VirtualClasses
            .Include(vc => vc.Enrollments)
            .FirstOrDefaultAsync(vc => vc.Id == classId, cancellationToken);

        if (vClass == null || !vClass.IsActive)
            throw new ArgumentException("Class not found or inactive.");

        if (vClass.ClassType == ClassType.Group && vClass.Enrollments.Count >= vClass.MaxCapacity)
            throw new InvalidOperationException("Class has reached maximum capacity.");

        if (vClass.Enrollments.Any(e => e.LearnerId == learnerId && e.Status == EnrollmentStatus.Active))
            throw new InvalidOperationException("Learner is already enrolled in this class.");

        // Check Entitlements
        if (vClass.IsSubscriptionIncluded)
        {
            var hasPremium = await _subscriptionService.HasActivePremiumSubscriptionAsync(learnerId, cancellationToken);
            if (!hasPremium)
                throw new UnauthorizedAccessException("This class requires an active Premium subscription.");
        }
        else if (vClass.Price > 0)
        {
            // For independent paid classes, we check if they have a successful PaymentTransaction for this class
            var hasPaid = await _context.PaymentTransactions
                .AnyAsync(pt => pt.UserId == learnerId 
                                && pt.ReferenceId == classId.ToString() 
                                && pt.Status == Phronesis.Domain.Commerce.PaymentStatus.Successful, cancellationToken);
            
            if (!hasPaid)
                throw new UnauthorizedAccessException("This class requires independent payment.");
        }

        var enrollment = new ClassEnrollment(learnerId, classId);
        _context.ClassEnrollments.Add(enrollment);
        await _context.SaveChangesAsync(cancellationToken);

        return true;
    }

    public async Task<IEnumerable<ClassSession>> GetLearnerScheduleAsync(Guid learnerId, DateTime startDate, DateTime endDate, CancellationToken cancellationToken = default)
    {
        return await _context.ClassSessions
            .Include(cs => cs.VirtualClass)
            .Where(cs => cs.VirtualClass.Enrollments.Any(e => e.LearnerId == learnerId && e.Status == EnrollmentStatus.Active))
            .Where(cs => cs.StartTime >= startDate && cs.StartTime <= endDate)
            .OrderBy(cs => cs.StartTime)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<ClassSession>> GetTeacherScheduleAsync(Guid teacherId, DateTime startDate, DateTime endDate, CancellationToken cancellationToken = default)
    {
        var sessions = await _context.ClassSessions
            .Include(cs => cs.VirtualClass)
            .Where(cs => cs.VirtualClass.TeacherId == teacherId && cs.StartTime >= startDate && cs.StartTime <= endDate)
            .OrderBy(cs => cs.StartTime)
            .ToListAsync(cancellationToken);

        return sessions;
    }

    public async Task RecordJoinAsync(Guid sessionId, Guid learnerId, CancellationToken cancellationToken = default)
    {
        var session = await _context.ClassSessions
            .Include(cs => cs.Attendances)
            .FirstOrDefaultAsync(cs => cs.Id == sessionId, cancellationToken);
        
        if (session == null) return;

        var attendance = session.Attendances.FirstOrDefault(a => a.LearnerId == learnerId);
        if (attendance == null)
        {
            attendance = new ClassAttendance(sessionId, learnerId);
            session.Attendances.Add(attendance);
        }

        attendance.RecordJoin(DateTime.UtcNow);
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task RecordLeaveAsync(Guid sessionId, Guid learnerId, CancellationToken cancellationToken = default)
    {
        var attendance = await _context.ClassAttendances
            .FirstOrDefaultAsync(a => a.ClassSessionId == sessionId && a.LearnerId == learnerId, cancellationToken);
        
        if (attendance != null)
        {
            attendance.RecordLeave(DateTime.UtcNow);
            await _context.SaveChangesAsync(cancellationToken);
        }
    }

    public async Task SubmitFeedbackAsync(Guid sessionId, Guid learnerId, int teacherRating, int contentRating, int techRating, string? wentWell, string? improve, string? complaints, CancellationToken cancellationToken = default)
    {
        var attendance = await _context.ClassAttendances
            .FirstOrDefaultAsync(a => a.ClassSessionId == sessionId && a.LearnerId == learnerId, cancellationToken);

        if (attendance == null || attendance.TotalDurationMinutes < 5)
            throw new ArgumentException("You must attend the session for at least 5 minutes to submit feedback.");

        var feedback = new SessionFeedback(sessionId, learnerId, teacherRating, contentRating, techRating);
        feedback.AddWrittenFeedback(wentWell, improve, complaints);

        _context.SessionFeedbacks.Add(feedback);
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task CompleteSessionAsync(Guid sessionId, Guid teacherId, string? recordingUrl, string? teacherNotes, CancellationToken cancellationToken = default)
    {
        var session = await _context.ClassSessions
            .Include(cs => cs.VirtualClass)
            .FirstOrDefaultAsync(cs => cs.Id == sessionId, cancellationToken);

        if (session == null || session.VirtualClass.TeacherId != teacherId)
            throw new ArgumentException("Session not found or you do not have permission.");

        session.CompleteSession(recordingUrl, teacherNotes);
        await _context.SaveChangesAsync(cancellationToken);
    }
}
