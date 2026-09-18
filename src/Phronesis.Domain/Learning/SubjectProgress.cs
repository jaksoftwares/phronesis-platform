using Phronesis.Domain.Academic;
using Phronesis.Domain.Common;
using Phronesis.Domain.Identity;

namespace Phronesis.Domain.Learning;

public class SubjectProgress : BaseEntity
{
    public Guid LearnerId { get; private set; }
    public Guid SubjectId { get; private set; }
    
    public int TotalContentItems { get; private set; }
    public int CompletedItems { get; private set; }
    public double ProgressPercentage { get; private set; }
    public DateTime LastCalculatedAt { get; private set; }

    public User Learner { get; private set; } = null!;
    public Subject Subject { get; private set; } = null!;

    private SubjectProgress() { }

    public SubjectProgress(Guid learnerId, Guid subjectId)
    {
        LearnerId = learnerId;
        SubjectId = subjectId;
        TotalContentItems = 0;
        CompletedItems = 0;
        ProgressPercentage = 0;
        LastCalculatedAt = DateTime.UtcNow;
    }

    public void UpdateProgress(int totalItems, int completedItems)
    {
        TotalContentItems = totalItems;
        CompletedItems = completedItems;
        ProgressPercentage = totalItems > 0 ? ((double)completedItems / totalItems) * 100 : 0;
        LastCalculatedAt = DateTime.UtcNow;
    }
}
