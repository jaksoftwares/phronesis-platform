namespace Phronesis.Domain.Collaboration;

using Phronesis.Domain.Tuition;
using Phronesis.Domain.Identity;
using System.Collections.Generic;

public class ClassDiscussion
{
    public Guid Id { get; private set; }
    public Guid VirtualClassId { get; private set; }
    public Guid AuthorId { get; private set; }
    
    public string Title { get; private set; }
    public string Content { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public bool IsResolved { get; private set; }

    // Navigations
    public VirtualClass VirtualClass { get; private set; } = null!;
    public User Author { get; private set; } = null!;
    
    private readonly List<DiscussionReply> _replies = new();
    public IReadOnlyCollection<DiscussionReply> Replies => _replies.AsReadOnly();

    private ClassDiscussion() { } // EF Core

    public ClassDiscussion(Guid virtualClassId, Guid authorId, string title, string content)
    {
        Id = Guid.NewGuid();
        VirtualClassId = virtualClassId;
        AuthorId = authorId;
        Title = title;
        Content = content;
        CreatedAt = DateTime.UtcNow;
        IsResolved = false;
    }

    public void MarkAsResolved()
    {
        IsResolved = true;
    }

    public void AddReply(DiscussionReply reply)
    {
        _replies.Add(reply);
    }
}
