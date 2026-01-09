using System;

namespace Shift.Contract
{
    public partial class BankQuestionAttachmentMatch
    {
        public Guid QuestionIdentifier { get; set; }
        public Guid UploadIdentifier { get; set; }
    }
}