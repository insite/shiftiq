using System;

using Shift.Common;

namespace Shift.Contract
{
    public class RetrieveAttemptMatch : Query<AttemptMatchModel>
    {
        public Guid AttemptIdentifier { get; set; }
        public Guid QuestionIdentifier { get; set; }

        public int MatchSequence { get; set; }
    }
}