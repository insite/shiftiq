namespace Shift.Service.Assessment;

public partial class AttemptMatchEntity
{
    public Guid AttemptIdentifier { get; set; }
    public Guid? OrganizationIdentifier { get; set; }
    public Guid QuestionIdentifier { get; set; }

    public string? AnswerText { get; set; }
    public string MatchLeftText { get; set; } = null!;
    public string MatchRightText { get; set; } = null!;

    public int MatchSequence { get; set; }
    public int QuestionSequence { get; set; }

    public decimal MatchPoints { get; set; }
}