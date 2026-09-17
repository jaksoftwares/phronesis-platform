using Phronesis.Domain.Common;
using Phronesis.Domain.Identity;

namespace Phronesis.Domain.Content;

public class SavedContent : BaseEntity
{
    public Guid UserId { get; private set; }
    public Guid EducationalContentId { get; private set; }

    public User User { get; private set; } = null!;
    public EducationalContent EducationalContent { get; private set; } = null!;

    private SavedContent() { }

    public SavedContent(Guid userId, Guid educationalContentId)
    {
        UserId = userId;
        EducationalContentId = educationalContentId;
    }
}
