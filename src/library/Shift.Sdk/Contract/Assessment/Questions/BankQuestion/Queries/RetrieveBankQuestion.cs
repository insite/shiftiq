using System;

using Shift.Common;

namespace Shift.Contract
{
    public class RetrieveBankQuestion : Query<BankQuestionModel>
    {
        public Guid QuestionIdentifier { get; set; }
    }
}