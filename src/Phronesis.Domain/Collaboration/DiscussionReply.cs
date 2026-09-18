namespace Phronesis.Domain.Collaboration;

using Phronesis.Domain.Identity;

public class DiscussionReply
{
    public Guid Id { get; private set; }
    public Guid ClassDiscussionId { get; private set; }
    public Guid AuthorId { get; private set; }
    
    public string Content { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public bool IsEndorsedByTeacher { get; private set; }

    // Navigations
    public ClassDiscussion ClassDiscussion { get; private set; } = null!;
    public User Author { get; private set; } = null!;

    private DiscussionReply() { } // EF Core

    public DiscussionReply(Guid classDiscussionId, Guid authorId, string content)
    {
        Id = Guid.NewGuid();
        ClassDiscussionId = classDiscussionId;
        AuthorId = authorId;
        Content = content;
        CreatedAt = DateTime.UtcNow;
        IsEndorsedByTeacher = false;
    }

    public void Endorse()
    {
        IsEndorsedByTeacher = true;
    }

    public void RevokeEndorsement()
    {
        IsEndorsedByTeacher = false;
    }
}
