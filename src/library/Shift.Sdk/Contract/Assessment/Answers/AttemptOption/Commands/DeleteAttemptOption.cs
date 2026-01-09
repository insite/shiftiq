using System;

namespace Shift.Contract
{
    public class DeleteAttemptOption
    {
        public Guid AttemptIdentifier { get; set; }
        public Guid QuestionIdentifier { get; set; }

        public int OptionKey { get; set; }
    }
}