using System;

using Shift.Common;

namespace Shift.Contract
{
    public class AssertBankFormQuestionGradeitem : Query<bool>
    {
        public Guid FormIdentifier { get; set; }
        public Guid QuestionIdentifier { get; set; }
    }
}