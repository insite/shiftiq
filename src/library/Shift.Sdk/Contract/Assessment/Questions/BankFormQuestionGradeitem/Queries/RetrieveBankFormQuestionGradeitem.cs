using System;

using Shift.Common;

namespace Shift.Contract
{
    public class RetrieveBankFormQuestionGradeitem : Query<BankFormQuestionGradeitemModel>
    {
        public Guid FormIdentifier { get; set; }
        public Guid QuestionIdentifier { get; set; }
    }
}