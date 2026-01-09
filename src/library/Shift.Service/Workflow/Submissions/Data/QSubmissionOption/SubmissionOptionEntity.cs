namespace Shift.Service.Workflow;

public partial class SubmissionOptionEntity
{
    public Guid? OrganizationIdentifier { get; set; }
    public Guid ResponseSessionIdentifier { get; set; }
    public Guid SurveyOptionIdentifier { get; set; }
    public Guid SurveyQuestionIdentifier { get; set; }

    public bool ResponseOptionIsSelected { get; set; }

    public int OptionSequence { get; set; }
}