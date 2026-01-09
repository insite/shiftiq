using System;

namespace Shift.Contract
{
    public partial class BankQuestionAttachmentModel
    {
        public Guid? OrganizationIdentifier { get; set; }
        public Guid QuestionIdentifier { get; set; }
        public Guid UploadIdentifier { get; set; }
    }
}