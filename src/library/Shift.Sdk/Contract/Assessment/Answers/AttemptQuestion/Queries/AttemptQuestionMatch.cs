using System;

namespace Shift.Contract
{
    public partial class AttemptQuestionMatch
    {
        public Guid AttemptIdentifier { get; set; }
        public Guid QuestionIdentifier { get; set; }
    }
}