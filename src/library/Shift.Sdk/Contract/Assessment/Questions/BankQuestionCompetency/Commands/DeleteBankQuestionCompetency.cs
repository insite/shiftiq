using System;

namespace Shift.Contract
{
    public class DeleteBankQuestionCompetency
    {
        public Guid QuestionIdentifier { get; set; }
        public Guid SubCompetencyIdentifier { get; set; }
    }
}