namespace Shift.Service.Assessment;

public partial class BankOptionEntity
{
    public Guid BankIdentifier { get; set; }
    public Guid? CompetencyIdentifier { get; set; }
    public Guid OrganizationIdentifier { get; set; }
    public Guid QuestionIdentifier { get; set; }
    public Guid SetIdentifier { get; set; }

    public string? OptionText { get; set; }

    public int OptionKey { get; set; }
}