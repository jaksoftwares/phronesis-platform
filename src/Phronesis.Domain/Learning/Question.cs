using Phronesis.Domain.Common;

namespace Phronesis.Domain.Learning;

public enum QuestionType
{
    MultipleChoice,
    TrueFalse
}

public class Question : BaseEntity
{
    public Guid AssessmentId { get; private set; }
    public string Text { get; private set; }
    public double Points { get; private set; }
    public QuestionType Type { get; private set; }

    public Assessment Assessment { get; private set; } = null!;
    
    private readonly List<QuestionOption> _options = new();
    public IReadOnlyCollection<QuestionOption> Options => _options.AsReadOnly();

    private Question() { }

    internal Question(Guid assessmentId, string text, double points, QuestionType type)
    {
        AssessmentId = assessmentId;
        Text = text;
        Points = points;
        Type = type;
    }

    public QuestionOption AddOption(string text, bool isCorrect)
    {
        // Enforce True/False logic optionally
        if (Type == QuestionType.TrueFalse && _options.Count >= 2)
            throw new InvalidOperationException("True/False questions can only have 2 options.");

        var option = new QuestionOption(Id, text, isCorrect);
        _options.Add(option);
        return option;
    }
}

public class QuestionOption : BaseEntity
{
    public Guid QuestionId { get; private set; }
    public string Text { get; private set; }
    public bool IsCorrect { get; private set; }

    public Question Question { get; private set; } = null!;

    private QuestionOption() { }

    internal QuestionOption(Guid questionId, string text, bool isCorrect)
    {
        QuestionId = questionId;
        Text = text;
        IsCorrect = isCorrect;
    }
}
