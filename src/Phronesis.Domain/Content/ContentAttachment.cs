using Phronesis.Domain.Common;

namespace Phronesis.Domain.Content;

public class ContentAttachment : BaseEntity
{
    public Guid EducationalContentId { get; private set; }
    public string FileName { get; private set; }
    public string FileUri { get; private set; }
    public string MimeType { get; private set; }
    public long SizeInBytes { get; private set; }
    public bool IsPrimary { get; private set; }

    public EducationalContent EducationalContent { get; private set; } = null!;

    private ContentAttachment() { }

    public ContentAttachment(
        Guid educationalContentId, 
        string fileName, 
        string fileUri, 
        string mimeType, 
        long sizeInBytes, 
        bool isPrimary)
    {
        EducationalContentId = educationalContentId;
        FileName = fileName;
        FileUri = fileUri;
        MimeType = mimeType;
        SizeInBytes = sizeInBytes;
        IsPrimary = isPrimary;
    }
}
