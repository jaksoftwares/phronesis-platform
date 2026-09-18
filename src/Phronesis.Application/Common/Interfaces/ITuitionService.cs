using Phronesis.Domain.Tuition;

namespace Phronesis.Application.Common.Interfaces;

public class CreateClassRequest
{
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public ClassType ClassType { get; set; }
    public int MaxCapacity { get; set; }
    public Guid TeacherId { get; set; }
    public Guid SubjectId { get; set; }
    public bool IsSubscriptionIncluded { get; set; }
    public decimal Price { get; set; }
}

public interface ITuitionService
{
    Task<VirtualClass> CreateClassAsync(CreateClassRequest request, CancellationToken cancellationToken = default);
    Task<bool> EnrollLearnerAsync(Guid learnerId, Guid classId, CancellationToken cancellationToken = default);
    Task<IEnumerable<ClassSession>> GetLearnerScheduleAsync(Guid learnerId, DateTime startDate, DateTime endDate, CancellationToken cancellationToken = default);
    Task<IEnumerable<ClassSession>> GetTeacherScheduleAsync(Guid teacherId, DateTime startDate, DateTime endDate, CancellationToken cancellationToken = default);
}
