using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Phronesis.Application.Common.Interfaces;
using Phronesis.Domain.Operations.Reporting;
using Phronesis.Domain.Tuition; // Assuming AttendanceStatus exists here

namespace Phronesis.Infrastructure.Services.Operations;

public class ReportingService : IReportingService
{
    private readonly IApplicationDbContext _context;
    private readonly IMemoryCache _cache;
    private static readonly TimeSpan AdminCacheDuration = TimeSpan.FromMinutes(10);

    public ReportingService(IApplicationDbContext context, IMemoryCache cache)
    {
        _context = context;
        _cache = cache;
    }

    public async Task<AdminDashboardStats> GetAdminStatsAsync(CancellationToken cancellationToken = default)
    {
        const string cacheKey = "AdminDashboardStats";
        if (_cache.TryGetValue(cacheKey, out AdminDashboardStats? cachedStats) && cachedStats != null)
        {
            return cachedStats;
        }

        var totalRevenue = await _context.Orders
            .Where(o => o.Status == Domain.Commerce.OrderStatus.Completed)
            .SumAsync(o => o.TotalAmount, cancellationToken);

        var activeSubs = await _context.UserSubscriptions
            .CountAsync(s => s.Status == Domain.Commerce.SubscriptionStatus.Active, cancellationToken);

        // We assume we have users in a separate identity store or a local Users table.
        // For this MVP, if we don't have a Users table in DbContext, we might mock this or query another table.
        // We will return 0 if no local Users table exists. We'll use Subscriptions as a proxy for active users for now.
        var totalUsers = activeSubs; // Proxy if no raw Users DbSet exists.

        var totalClasses = await _context.ClassSessions.CountAsync(cancellationToken);

        var stats = new AdminDashboardStats
        {
            TotalRevenue = totalRevenue,
            ActiveSubscriptions = activeSubs,
            TotalUsers = totalUsers,
            TotalClasses = totalClasses
        };

        _cache.Set(cacheKey, stats, AdminCacheDuration);

        return stats;
    }

    public async Task<TeacherDashboardStats> GetTeacherStatsAsync(string teacherId, CancellationToken cancellationToken = default)
    {
        var now = DateTime.UtcNow;

        var upcomingClasses = await _context.ClassSessions
            .CountAsync(c => c.VirtualClass.TeacherId.ToString() == teacherId && c.StartTime > now, cancellationToken);

        // Distinct count of students who are enrolled in classes with this teacher
        var totalStudents = await _context.ClassEnrollments
            .Where(e => e.VirtualClass.TeacherId.ToString() == teacherId)
            .Select(e => e.LearnerId)
            .Distinct()
            .CountAsync(cancellationToken);

        // Attendance rate calculation
        var totalAttendancesExpected = await _context.ClassAttendances
            .Where(a => a.ClassSession.VirtualClass.TeacherId.ToString() == teacherId && a.ClassSession.StartTime <= now)
            .CountAsync(cancellationToken);

        var presentCount = await _context.ClassAttendances
            .Where(a => a.ClassSession.VirtualClass.TeacherId.ToString() == teacherId && a.ClassSession.StartTime <= now && a.Status == AttendanceStatus.Present)
            .CountAsync(cancellationToken);

        double avgAttendance = totalAttendancesExpected > 0 ? (double)presentCount / totalAttendancesExpected * 100 : 0;

        return new TeacherDashboardStats
        {
            UpcomingClasses = upcomingClasses,
            TotalStudentsTaught = totalStudents,
            AverageAttendanceRate = Math.Round(avgAttendance, 2)
        };
    }

    public async Task<LearnerDashboardStats> GetLearnerStatsAsync(string learnerId, CancellationToken cancellationToken = default)
    {
        var now = DateTime.UtcNow;

        var classesAttended = await _context.ClassAttendances
            .CountAsync(a => a.LearnerId.ToString() == learnerId && a.Status == AttendanceStatus.Present, cancellationToken);

        var upcomingClasses = await _context.ClassEnrollments
            .Where(e => e.LearnerId.ToString() == learnerId)
            .SelectMany(e => e.VirtualClass.Sessions)
            .CountAsync(s => s.StartTime > now, cancellationToken);

        // We can proxy Courses Enrolled by counting active subscriptions or unique subjects booked.
        // Let's count active subscriptions for now.
        var coursesEnrolled = await _context.UserSubscriptions
            .CountAsync(s => s.UserId.ToString() == learnerId && s.Status == Domain.Commerce.SubscriptionStatus.Active, cancellationToken);

        return new LearnerDashboardStats
        {
            ClassesAttended = classesAttended,
            CoursesEnrolled = coursesEnrolled,
            UpcomingClasses = upcomingClasses
        };
    }
}
