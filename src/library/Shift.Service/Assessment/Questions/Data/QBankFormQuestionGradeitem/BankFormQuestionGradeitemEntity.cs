namespace Shift.Service.Assessment;

public partial class BankFormQuestionGradeitemEntity
{
    public Guid FormIdentifier { get; set; }
    public Guid GradeItemIdentifier { get; set; }
    public Guid OrganizationIdentifier { get; set; }
    public Guid QuestionIdentifier { get; set; }
}