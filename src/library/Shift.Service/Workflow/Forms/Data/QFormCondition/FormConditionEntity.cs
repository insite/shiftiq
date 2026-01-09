namespace Shift.Service.Workflow;

public partial class FormConditionEntity
{
    public Guid MaskedSurveyQuestionIdentifier { get; set; }
    public Guid MaskingSurveyOptionItemIdentifier { get; set; }
    public Guid? OrganizationIdentifier { get; set; }
}