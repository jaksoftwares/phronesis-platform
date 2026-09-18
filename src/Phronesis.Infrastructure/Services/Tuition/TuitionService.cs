using Microsoft.EntityFrameworkCore;
using Phronesis.Application.Common.Interfaces;
using Phronesis.Domain.Tuition;

namespace Phronesis.Infrastructure.Services.Tuition;

public class TuitionService : ITuitionService
{
    private readonly IApplicationDbContext _context;
    private readonly ISubscriptionService _subscriptionService;

    public TuitionService(IApplicationDbContext context, ISubscriptionService subscriptionService)
    {
        _context = context;
        _subscriptionService = subscriptionService;
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
        return await _context.ClassSessions
            .Include(cs => cs.VirtualClass)
            .Where(cs => cs.VirtualClass.TeacherId == teacherId)
            .Where(cs => cs.StartTime >= startDate && cs.StartTime <= endDate)
            .OrderBy(cs => cs.StartTime)
            .ToListAsync(cancellationToken);
    }
}
