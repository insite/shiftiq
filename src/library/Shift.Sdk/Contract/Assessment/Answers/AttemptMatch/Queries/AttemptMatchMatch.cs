using System;

namespace Shift.Contract
{
    public partial class AttemptMatchMatch
    {
        public Guid AttemptIdentifier { get; set; }
        public Guid QuestionIdentifier { get; set; }

        public int MatchSequence { get; set; }
    }
}