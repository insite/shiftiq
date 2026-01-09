namespace Shift.Service.Assessment;

public partial class BankQuestionAttachmentEntity
{
    public Guid? OrganizationIdentifier { get; set; }
    public Guid QuestionIdentifier { get; set; }
    public Guid UploadIdentifier { get; set; }
}