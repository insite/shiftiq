namespace Shift.Service.Workflow;

public partial class SubmissionAnswerEntity
{
    public Guid? OrganizationIdentifier { get; set; }
    public Guid RespondentUserIdentifier { get; set; }
    public Guid ResponseSessionIdentifier { get; set; }
    public Guid SurveyQuestionIdentifier { get; set; }

    public string? ResponseAnswerText { get; set; }
}