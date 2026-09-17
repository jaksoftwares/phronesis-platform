using Phronesis.Domain.Common;

namespace Phronesis.Domain.Content;

public class ContentTag : BaseEntity
{
    public Guid EducationalContentId { get; private set; }
    public string Name { get; private set; }

    public EducationalContent EducationalContent { get; private set; } = null!;

    private ContentTag() { }

    public ContentTag(Guid educationalContentId, string name)
    {
        EducationalContentId = educationalContentId;
        Name = name.ToLowerInvariant().Trim();
    }
}
