using System;

using Shift.Common;

namespace Shift.Contract
{
    public class AssertBankQuestionAttachment : Query<bool>
    {
        public Guid QuestionIdentifier { get; set; }
        public Guid UploadIdentifier { get; set; }
    }
}