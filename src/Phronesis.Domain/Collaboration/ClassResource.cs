namespace Phronesis.Domain.Collaboration;

using Phronesis.Domain.Tuition;
using Phronesis.Domain.Identity;

public class ClassResource
{
    public Guid Id { get; private set; }
    public Guid VirtualClassId { get; private set; }
    public Guid TeacherId { get; private set; }
    public Guid? ClassSessionId { get; private set; }
    
    public string Title { get; private set; }
    public string Description { get; private set; }
    public string FileUrl { get; private set; }
    public string MimeType { get; private set; }
    public long SizeInBytes { get; private set; }
    
    public DateTime UploadedAt { get; private set; }

    // Navigations
    public VirtualClass VirtualClass { get; private set; } = null!;
    public User Teacher { get; private set; } = null!;
    public ClassSession? ClassSession { get; private set; }

    private ClassResource() { } // EF Core

    public ClassResource(Guid virtualClassId, Guid teacherId, string title, string description, string fileUrl, string mimeType, long sizeInBytes, Guid? classSessionId = null)
    {
        Id = Guid.NewGuid();
        VirtualClassId = virtualClassId;
        TeacherId = teacherId;
        Title = title;
        Description = description;
        FileUrl = fileUrl;
        MimeType = mimeType;
        SizeInBytes = sizeInBytes;
        ClassSessionId = classSessionId;
        UploadedAt = DateTime.UtcNow;
    }
}
