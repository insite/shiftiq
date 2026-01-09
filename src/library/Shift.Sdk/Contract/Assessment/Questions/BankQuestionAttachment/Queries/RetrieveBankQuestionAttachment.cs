using System;

using Shift.Common;

namespace Shift.Contract
{
    public class RetrieveBankQuestionAttachment : Query<BankQuestionAttachmentModel>
    {
        public Guid QuestionIdentifier { get; set; }
        public Guid UploadIdentifier { get; set; }
    }
}