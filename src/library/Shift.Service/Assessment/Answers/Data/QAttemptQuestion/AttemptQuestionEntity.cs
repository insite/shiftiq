namespace Shift.Service.Assessment;

public partial class AttemptQuestionEntity
{
    public Guid? AnswerFileIdentifier { get; set; }
    public Guid? AnswerSolutionIdentifier { get; set; }
    public Guid AttemptIdentifier { get; set; }
    public Guid? CompetencyAreaIdentifier { get; set; }
    public Guid? CompetencyItemIdentifier { get; set; }
    public Guid? OrganizationIdentifier { get; set; }
    public Guid? ParentQuestionIdentifier { get; set; }
    public Guid QuestionIdentifier { get; set; }

    public bool? ShowShapes { get; set; }

    public string? AnswerText { get; set; }
    public string? CompetencyAreaCode { get; set; }
    public string? CompetencyAreaLabel { get; set; }
    public string? CompetencyAreaTitle { get; set; }
    public string? CompetencyItemCode { get; set; }
    public string? CompetencyItemLabel { get; set; }
    public string? CompetencyItemTitle { get; set; }
    public string? HotspotImage { get; set; }
    public string? QuestionBottomLabel { get; set; }
    public string QuestionCalculationMethod { get; set; } = null!;
    public string? QuestionMatchDistractors { get; set; }
    public string? QuestionText { get; set; }
    public string? QuestionTopLabel { get; set; }
    public string QuestionType { get; set; } = null!;
    public string? RubricRatingPoints { get; set; }

    public int? AnswerAttemptLimit { get; set; }
    public int? AnswerOptionKey { get; set; }
    public int? AnswerOptionSequence { get; set; }
    public int? AnswerRequestAttempt { get; set; }
    public int? AnswerSubmitAttempt { get; set; }
    public int? AnswerTimeLimit { get; set; }
    public int? PinLimit { get; set; }
    public int? QuestionNumber { get; set; }
    public int QuestionSequence { get; set; }
    public int? SectionIndex { get; set; }

    public decimal? AnswerPoints { get; set; }
    public decimal? QuestionCutScore { get; set; }
    public decimal? QuestionPoints { get; set; }
}