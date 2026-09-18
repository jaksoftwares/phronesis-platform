using Phronesis.Domain.Academic;
using Phronesis.Domain.Common;
using Phronesis.Domain.Identity;

namespace Phronesis.Domain.Learning;

public class Certificate : BaseEntity
{
    public Guid LearnerId { get; private set; }
    public Guid SubjectId { get; private set; }
    public string CertificateCode { get; private set; }
    public DateTime IssuedAt { get; private set; }

    public User Learner { get; private set; } = null!;
    public Subject Subject { get; private set; } = null!;

    private Certificate() { }

    public Certificate(Guid learnerId, Guid subjectId)
    {
        LearnerId = learnerId;
        SubjectId = subjectId;
        CertificateCode = GenerateUniqueCode();
        IssuedAt = DateTime.UtcNow;
    }

    private string GenerateUniqueCode()
    {
        // Generates a random alphanumeric code like PHR-ABCD-1234
        var random = new Random();
        const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
        var segment1 = new string(Enumerable.Repeat(chars, 4).Select(s => s[random.Next(s.Length)]).ToArray());
        var segment2 = new string(Enumerable.Repeat(chars, 4).Select(s => s[random.Next(s.Length)]).ToArray());
        return $"PHR-{segment1}-{segment2}";
    }
}
