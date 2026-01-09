using System;

namespace Shift.Contract
{
    public class ModifyBankQuestionCompetency
    {
        public Guid QuestionIdentifier { get; set; }
        public Guid SubCompetencyIdentifier { get; set; }
    }
}