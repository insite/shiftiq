namespace Shift.Service.Workflow;

public partial class FormOptionListEntity
{
    public Guid? OrganizationIdentifier { get; set; }
    public Guid SurveyOptionListIdentifier { get; set; }
    public Guid SurveyQuestionIdentifier { get; set; }

    public int SurveyOptionListSequence { get; set; }
}