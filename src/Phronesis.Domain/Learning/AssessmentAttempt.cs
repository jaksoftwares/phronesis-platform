using Phronesis.Domain.Common;
using Phronesis.Domain.Identity;

namespace Phronesis.Domain.Learning;

public enum AttemptStatus
{
    InProgress,
    Completed
}

public class AssessmentAttempt : BaseEntity
{
    public Guid LearnerId { get; private set; }
    public Guid AssessmentId { get; private set; }
    public DateTime StartedAt { get; private set; }
    public DateTime? CompletedAt { get; private set; }
    public double TotalScore { get; private set; }
    public double ScorePercentage { get; private set; }
    public AttemptStatus Status { get; private set; }

    public User Learner { get; private set; } = null!;
    public Assessment Assessment { get; private set; } = null!;

    private readonly List<AttemptAnswer> _answers = new();
    public IReadOnlyCollection<AttemptAnswer> Answers => _answers.AsReadOnly();

    private AssessmentAttempt() { }

    public AssessmentAttempt(Guid learnerId, Guid assessmentId)
    {
        LearnerId = learnerId;
        AssessmentId = assessmentId;
        StartedAt = DateTime.UtcNow;
        Status = AttemptStatus.InProgress;
        TotalScore = 0;
        ScorePercentage = 0;
    }

    public AttemptAnswer AddAnswer(Guid questionId, Guid selectedOptionId, bool isCorrect, double pointsAwarded)
    {
        if (Status == AttemptStatus.Completed)
            throw new InvalidOperationException("Cannot modify a completed attempt.");

        var answer = new AttemptAnswer(Id, questionId, selectedOptionId, isCorrect, pointsAwarded);
        _answers.Add(answer);
        return answer;
    }

    public void Complete(double maxPossibleScore)
    {
        if (Status == AttemptStatus.Completed)
            throw new InvalidOperationException("Attempt is already completed.");

        TotalScore = _answers.Sum(a => a.PointsAwarded);
        ScorePercentage = maxPossibleScore > 0 ? (TotalScore / maxPossibleScore) * 100 : 0;
        CompletedAt = DateTime.UtcNow;
        Status = AttemptStatus.Completed;
    }
}

public class AttemptAnswer : BaseEntity
{
    public Guid AttemptId { get; private set; }
    public Guid QuestionId { get; private set; }
    public Guid SelectedOptionId { get; private set; }
    public bool IsCorrect { get; private set; }
    public double PointsAwarded { get; private set; }

    public AssessmentAttempt Attempt { get; private set; } = null!;
    public Question Question { get; private set; } = null!;
    public QuestionOption SelectedOption { get; private set; } = null!;

    private AttemptAnswer() { }

    internal AttemptAnswer(Guid attemptId, Guid questionId, Guid selectedOptionId, bool isCorrect, double pointsAwarded)
    {
        AttemptId = attemptId;
        QuestionId = questionId;
        SelectedOptionId = selectedOptionId;
        IsCorrect = isCorrect;
        PointsAwarded = pointsAwarded;
    }
}
