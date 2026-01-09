using System;

namespace Shift.Contract
{
    public class ModifyBankQuestionAttachment
    {
        public Guid? OrganizationIdentifier { get; set; }
        public Guid QuestionIdentifier { get; set; }
        public Guid UploadIdentifier { get; set; }
    }
}