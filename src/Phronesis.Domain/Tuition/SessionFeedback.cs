namespace Phronesis.Domain.Tuition;

public class SessionFeedback
{
    public Guid Id { get; private set; }
    public Guid ClassSessionId { get; private set; }
    public Guid LearnerId { get; private set; }
    public DateTime SubmittedAt { get; private set; }

    public int TeacherRating { get; private set; } // 1-5
    public int ContentRating { get; private set; } // 1-5
    public int TechnicalQualityRating { get; private set; } // 1-5

    public string? WhatWentWell { get; private set; }
    public string? AreasForImprovement { get; private set; }
    public string? Complaints { get; private set; }

    // Navigation Properties
    public ClassSession ClassSession { get; private set; } = null!;

    private SessionFeedback() { } // EF Core

    public SessionFeedback(Guid classSessionId, Guid learnerId, int teacherRating, int contentRating, int techRating)
    {
        if (teacherRating < 1 || teacherRating > 5 || contentRating < 1 || contentRating > 5 || techRating < 1 || techRating > 5)
            throw new ArgumentException("Ratings must be between 1 and 5");

        Id = Guid.NewGuid();
        ClassSessionId = classSessionId;
        LearnerId = learnerId;
        TeacherRating = teacherRating;
        ContentRating = contentRating;
        TechnicalQualityRating = techRating;
        SubmittedAt = DateTime.UtcNow;
    }

    public void AddWrittenFeedback(string? wentWell, string? improve, string? complaints)
    {
        WhatWentWell = wentWell;
        AreasForImprovement = improve;
        Complaints = complaints;
    }
}
