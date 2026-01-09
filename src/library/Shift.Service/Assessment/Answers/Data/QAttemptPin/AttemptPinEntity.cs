namespace Shift.Service.Assessment;

public partial class AttemptPinEntity
{
    public AttemptEntity? Attempt { get; set; }

    public Guid AttemptIdentifier { get; set; }
    public Guid QuestionIdentifier { get; set; }

    public string? OptionText { get; set; }

    public int? OptionKey { get; set; }
    public int? OptionSequence { get; set; }
    public int PinSequence { get; set; }
    public int PinX { get; set; }
    public int PinY { get; set; }
    public int QuestionSequence { get; set; }

    public decimal? OptionPoints { get; set; }
}