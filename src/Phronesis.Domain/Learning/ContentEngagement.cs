using Phronesis.Domain.Common;
using Phronesis.Domain.Content;
using Phronesis.Domain.Identity;

namespace Phronesis.Domain.Learning;

public class ContentEngagement : BaseEntity
{
    public Guid LearnerId { get; private set; }
    public Guid EducationalContentId { get; private set; }
    
    public DateTime LastAccessedAt { get; private set; }
    public long TimeSpentSeconds { get; private set; }
    public bool IsCompleted { get; private set; }

    public User Learner { get; private set; } = null!;
    public EducationalContent EducationalContent { get; private set; } = null!;

    private ContentEngagement() { }

    public ContentEngagement(Guid learnerId, Guid educationalContentId)
    {
        LearnerId = learnerId;
        EducationalContentId = educationalContentId;
        LastAccessedAt = DateTime.UtcNow;
        TimeSpentSeconds = 0;
        IsCompleted = false;
    }

    public void RecordHeartbeat(long deltaSeconds, bool isCompleted)
    {
        LastAccessedAt = DateTime.UtcNow;
        TimeSpentSeconds += deltaSeconds;
        
        // Once completed, it cannot be un-completed by simply watching more
        if (isCompleted)
        {
            IsCompleted = true;
        }
    }
}
