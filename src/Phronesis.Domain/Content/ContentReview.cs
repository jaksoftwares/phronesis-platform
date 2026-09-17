using Phronesis.Domain.Common;
using Phronesis.Domain.Identity;

namespace Phronesis.Domain.Content;

public class ContentReview : BaseEntity
{
    public Guid EducationalContentId { get; private set; }
    public Guid ReviewerId { get; private set; }
    public ReviewOutcome Outcome { get; private set; }
    public string Feedback { get; private set; }

    public EducationalContent EducationalContent { get; private set; } = null!;
    public User Reviewer { get; private set; } = null!;

    private ContentReview() { }

    public ContentReview(Guid educationalContentId, Guid reviewerId, ReviewOutcome outcome, string feedback)
    {
        EducationalContentId = educationalContentId;
        ReviewerId = reviewerId;
        Outcome = outcome;
        Feedback = feedback;
    }
}
