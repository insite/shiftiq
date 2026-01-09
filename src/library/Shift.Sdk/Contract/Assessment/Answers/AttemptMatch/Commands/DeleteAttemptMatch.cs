using System;

namespace Shift.Contract
{
    public class DeleteAttemptMatch
    {
        public Guid AttemptIdentifier { get; set; }
        public Guid QuestionIdentifier { get; set; }

        public int MatchSequence { get; set; }
    }
}