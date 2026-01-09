using System;

namespace Shift.Contract
{
    public partial class AttemptPinMatch
    {
        public Guid AttemptIdentifier { get; set; }
        public Guid QuestionIdentifier { get; set; }

        public int PinSequence { get; set; }
    }
}