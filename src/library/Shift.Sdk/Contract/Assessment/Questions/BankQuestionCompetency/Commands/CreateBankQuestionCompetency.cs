using System;

namespace Shift.Contract
{
    public class CreateBankQuestionCompetency
    {
        public Guid QuestionIdentifier { get; set; }
        public Guid SubCompetencyIdentifier { get; set; }
    }
}