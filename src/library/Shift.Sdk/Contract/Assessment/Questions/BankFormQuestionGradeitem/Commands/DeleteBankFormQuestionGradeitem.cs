using System;

namespace Shift.Contract
{
    public class DeleteBankFormQuestionGradeitem
    {
        public Guid FormIdentifier { get; set; }
        public Guid QuestionIdentifier { get; set; }
    }
}