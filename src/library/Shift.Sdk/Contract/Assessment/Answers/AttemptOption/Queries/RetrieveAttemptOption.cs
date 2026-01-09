using System;

using Shift.Common;

namespace Shift.Contract
{
    public class RetrieveAttemptOption : Query<AttemptOptionModel>
    {
        public Guid AttemptIdentifier { get; set; }
        public Guid QuestionIdentifier { get; set; }

        public int OptionKey { get; set; }
    }
}