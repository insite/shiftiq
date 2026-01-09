namespace Shift.Service.Assessment;

public partial class AttemptSectionEntity
{
    public AttemptEntity? Attempt { get; set; }

    public Guid AttemptIdentifier { get; set; }
    public Guid? SectionIdentifier { get; set; }

    public bool IsBreakTimer { get; set; }
    public bool ShowWarningNextTab { get; set; }

    public string? TimerType { get; set; }

    public int? SectionDuration { get; set; }
    public int SectionIndex { get; set; }
    public int? TimeLimit { get; set; }

    public DateTimeOffset? SectionCompleted { get; set; }
    public DateTimeOffset? SectionStarted { get; set; }
}