using Phronesis.Domain.Academic;
using Phronesis.Domain.Common;
using Phronesis.Domain.Identity;

namespace Phronesis.Domain.Content;

public class EducationalContent : BaseEntity
{
    public string Title { get; private set; }
    public string Description { get; private set; }
    public string Version { get; private set; }
    public bool IsPremium { get; private set; }
    
    public ContentType ContentType { get; private set; }
    public ContentStatus Status { get; private set; }
    
    public Guid AuthorId { get; private set; }
    
    public Guid GradeLevelId { get; private set; }
    public Guid SubjectId { get; private set; }
    
    public Guid? StrandId { get; private set; }
    public Guid? SubStrandId { get; private set; }
    public Guid? LearningObjectiveId { get; private set; }

    // Navigations
    public User Author { get; private set; } = null!;
    public GradeLevel GradeLevel { get; private set; } = null!;
    public Subject Subject { get; private set; } = null!;
    public Strand? Strand { get; private set; }
    public SubStrand? SubStrand { get; private set; }
    public LearningObjective? LearningObjective { get; private set; }

    private readonly List<ContentTag> _tags = new();
    public IReadOnlyCollection<ContentTag> Tags => _tags.AsReadOnly();

    private readonly List<ContentReview> _reviews = new();
    public IReadOnlyCollection<ContentReview> Reviews => _reviews.AsReadOnly();

    private readonly List<ContentAttachment> _attachments = new();
    public IReadOnlyCollection<ContentAttachment> Attachments => _attachments.AsReadOnly();

    private EducationalContent() { }

    public EducationalContent(
        string title, 
        string description, 
        string version, 
        bool isPremium, 
        ContentType contentType, 
        Guid authorId, 
        Guid gradeLevelId, 
        Guid subjectId)
    {
        Title = title;
        Description = description;
        Version = version;
        IsPremium = isPremium;
        ContentType = contentType;
        AuthorId = authorId;
        GradeLevelId = gradeLevelId;
        SubjectId = subjectId;
        
        Status = ContentStatus.Draft;
    }

    public void UpdateMetadata(string title, string description, string version, bool isPremium)
    {
        Title = title;
        Description = description;
        Version = version;
        IsPremium = isPremium;
    }

    public void SetTaxonomy(Guid? strandId, Guid? subStrandId, Guid? learningObjectiveId)
    {
        StrandId = strandId;
        SubStrandId = subStrandId;
        LearningObjectiveId = learningObjectiveId;
    }

    public void SubmitForReview()
    {
        if (Status != ContentStatus.Draft)
            throw new InvalidOperationException("Only Draft content can be submitted for review.");
        Status = ContentStatus.InReview;
    }

    public void AddReview(Guid reviewerId, ReviewOutcome outcome, string feedback)
    {
        if (Status != ContentStatus.InReview)
            throw new InvalidOperationException("Content must be InReview to receive a review.");

        var review = new ContentReview(Id, reviewerId, outcome, feedback);
        _reviews.Add(review);

        if (outcome == ReviewOutcome.Approved)
        {
            Status = ContentStatus.Published;
        }
        else
        {
            Status = ContentStatus.Draft; // Send back to author for revision
        }
    }

    public void Archive()
    {
        Status = ContentStatus.Archived;
    }

    public void AddTag(string name)
    {
        if (!_tags.Any(t => t.Name.Equals(name, StringComparison.OrdinalIgnoreCase)))
        {
            _tags.Add(new ContentTag(Id, name));
        }
    }
    
    public void RemoveTag(string name)
    {
        var tag = _tags.FirstOrDefault(t => t.Name.Equals(name, StringComparison.OrdinalIgnoreCase));
        if (tag != null)
        {
            _tags.Remove(tag);
        }
    }

    public ContentAttachment AddAttachment(string fileName, string fileUri, string mimeType, long sizeInBytes, bool isPrimary)
    {
        if (Status != ContentStatus.Draft)
            throw new InvalidOperationException("Attachments can only be modified while the content is in Draft status.");

        var attachment = new ContentAttachment(Id, fileName, fileUri, mimeType, sizeInBytes, isPrimary);
        _attachments.Add(attachment);
        return attachment;
    }

    public void RemoveAttachment(Guid attachmentId)
    {
        if (Status != ContentStatus.Draft)
            throw new InvalidOperationException("Attachments can only be modified while the content is in Draft status.");

        var attachment = _attachments.FirstOrDefault(a => a.Id == attachmentId);
        if (attachment != null)
        {
            _attachments.Remove(attachment);
        }
    }
}
