using System;

using Shift.Common;

namespace Shift.Contract
{
    public class AssertBankQuestionCompetency : Query<bool>
    {
        public Guid QuestionIdentifier { get; set; }
        public Guid SubCompetencyIdentifier { get; set; }
    }
}