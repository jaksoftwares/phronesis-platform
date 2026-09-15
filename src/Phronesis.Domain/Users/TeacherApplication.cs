using Phronesis.Domain.Common;
using Phronesis.Domain.Identity;

namespace Phronesis.Domain.Users;

public class TeacherApplication : BaseEntity
{
    public Guid TeacherProfileId { get; private set; }
    public ApplicationStatus Status { get; private set; }
    public Guid? ReviewerId { get; private set; } // FK to Staff/User
    public string? AdminNotes { get; private set; }
    
    // Interview Scheduling
    public DateTime? InterviewDate { get; private set; }
    public string? InterviewLink { get; private set; }
    public string? InterviewNotes { get; private set; }

    public DateTime? SubmittedAt { get; private set; }
    public DateTime? ReviewedAt { get; private set; }
    public bool PolicyAccepted { get; private set; }

    public TeacherProfile TeacherProfile { get; private set; } = null!;
    
    private readonly List<TeacherDocument> _documents = new();
    public IReadOnlyCollection<TeacherDocument> Documents => _documents.AsReadOnly();

    private TeacherApplication() { }

    public TeacherApplication(Guid teacherProfileId)
    {
        TeacherProfileId = teacherProfileId;
        Status = ApplicationStatus.Draft;
    }

    public void AddDocument(TeacherDocument document)
    {
        if (Status != ApplicationStatus.Draft)
            throw new InvalidOperationException("Documents can only be added while in Draft state.");
        
        _documents.Add(document);
    }

    public void Submit(bool policyAccepted)
    {
        if (!policyAccepted)
            throw new InvalidOperationException("Policy must be accepted to submit the application.");
        
        if (Status != ApplicationStatus.Draft)
            throw new InvalidOperationException("Only Draft applications can be submitted.");

        PolicyAccepted = true;
        Status = ApplicationStatus.Submitted;
        SubmittedAt = DateTime.UtcNow;
    }

    public void AssignReviewer(Guid reviewerId)
    {
        if (Status != ApplicationStatus.Submitted)
            throw new InvalidOperationException("Can only assign reviewers to Submitted applications.");

        ReviewerId = reviewerId;
        Status = ApplicationStatus.UnderReview;
    }

    public void ScheduleInterview(DateTime interviewDate, string interviewLink)
    {
        if (Status != ApplicationStatus.UnderReview)
            throw new InvalidOperationException("Can only schedule interviews for applications UnderReview.");

        InterviewDate = interviewDate;
        InterviewLink = interviewLink;
        Status = ApplicationStatus.InterviewScheduled;
    }

    public void CompleteInterview(string notes)
    {
        if (Status != ApplicationStatus.InterviewScheduled)
            throw new InvalidOperationException("Can only complete an interview if one is scheduled.");

        InterviewNotes = notes;
        Status = ApplicationStatus.InterviewCompleted;
    }

    public void Approve(string notes)
    {
        if (Status != ApplicationStatus.InterviewCompleted)
            throw new InvalidOperationException("Interview must be completed before final approval.");

        AdminNotes = notes;
        Status = ApplicationStatus.Approved;
        ReviewedAt = DateTime.UtcNow;
        
        // Ensure TeacherProfile VerificationState syncs (done in controller/handler)
    }

    public void Reject(string notes)
    {
        AdminNotes = notes;
        Status = ApplicationStatus.Rejected;
        ReviewedAt = DateTime.UtcNow;
    }
}
