using System;

namespace Shift.Contract
{
    public partial class AttemptOptionMatch
    {
        public Guid AttemptIdentifier { get; set; }
        public Guid QuestionIdentifier { get; set; }

        public int OptionKey { get; set; }
    }
}