using Phronesis.Domain.Academic;
using Phronesis.Domain.Common;

namespace Phronesis.Domain.Learning;

public enum AssessmentType
{
    Quiz,
    MockExam,
    TopicalExercise
}

public class Assessment : BaseEntity
{
    public string Title { get; private set; }
    public string Description { get; private set; }
    public int DurationMinutes { get; private set; }
    public double PassingScorePercentage { get; private set; }
    public AssessmentType Type { get; private set; }
    public bool IsPublished { get; private set; }

    public Guid SubjectId { get; private set; }
    public Guid? TopicId { get; private set; } // Can be mapped to Strand/SubStrand conceptually

    public Subject Subject { get; private set; } = null!;
    
    private readonly List<Question> _questions = new();
    public IReadOnlyCollection<Question> Questions => _questions.AsReadOnly();

    private Assessment() { }

    public Assessment(
        string title, 
        string description, 
        int durationMinutes, 
        double passingScorePercentage, 
        AssessmentType type, 
        Guid subjectId, 
        Guid? topicId = null)
    {
        Title = title;
        Description = description;
        DurationMinutes = durationMinutes;
        PassingScorePercentage = passingScorePercentage;
        Type = type;
        SubjectId = subjectId;
        TopicId = topicId;
        IsPublished = false;
    }

    public void Publish()
    {
        if (!_questions.Any())
            throw new InvalidOperationException("Cannot publish an assessment with no questions.");
        IsPublished = true;
    }

    public void Unpublish()
    {
        IsPublished = false;
    }

    public Question AddQuestion(string text, double points, QuestionType type)
    {
        if (IsPublished)
            throw new InvalidOperationException("Cannot modify questions on a published assessment.");

        var question = new Question(Id, text, points, type);
        _questions.Add(question);
        return question;
    }
}
