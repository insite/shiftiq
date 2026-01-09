using System;

using Shift.Common;

namespace Shift.Contract
{
    public class RetrieveBankQuestionCompetency : Query<BankQuestionCompetencyModel>
    {
        public Guid QuestionIdentifier { get; set; }
        public Guid SubCompetencyIdentifier { get; set; }
    }
}