using Phronesis.Domain.Common;

namespace Phronesis.Domain.Users;

public class TeacherDocument : BaseEntity
{
    public Guid TeacherApplicationId { get; private set; }
    public DocumentType DocumentType { get; private set; }
    public string FileUri { get; private set; } = string.Empty;
    public DocumentVerificationStatus VerificationStatus { get; private set; }
    public string? RejectionReason { get; private set; }

    public TeacherApplication TeacherApplication { get; private set; } = null!;

    private TeacherDocument() { }

    public TeacherDocument(Guid teacherApplicationId, DocumentType type, string fileUri)
    {
        TeacherApplicationId = teacherApplicationId;
        DocumentType = type;
        FileUri = fileUri;
        VerificationStatus = DocumentVerificationStatus.Pending;
    }

    public void Verify()
    {
        VerificationStatus = DocumentVerificationStatus.Verified;
    }

    public void Reject(string reason)
    {
        VerificationStatus = DocumentVerificationStatus.Rejected;
        RejectionReason = reason;
    }
}
