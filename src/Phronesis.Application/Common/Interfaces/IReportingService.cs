using Phronesis.Domain.Operations.Reporting;

namespace Phronesis.Application.Common.Interfaces;

public interface IReportingService
{
    Task<AdminDashboardStats> GetAdminStatsAsync(CancellationToken cancellationToken = default);
    Task<TeacherDashboardStats> GetTeacherStatsAsync(string teacherId, CancellationToken cancellationToken = default);
    Task<LearnerDashboardStats> GetLearnerStatsAsync(string learnerId, CancellationToken cancellationToken = default);
}
