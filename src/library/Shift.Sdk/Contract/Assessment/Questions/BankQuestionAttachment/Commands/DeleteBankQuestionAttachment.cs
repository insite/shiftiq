using System;

namespace Shift.Contract
{
    public class DeleteBankQuestionAttachment
    {
        public Guid QuestionIdentifier { get; set; }
        public Guid UploadIdentifier { get; set; }
    }
}