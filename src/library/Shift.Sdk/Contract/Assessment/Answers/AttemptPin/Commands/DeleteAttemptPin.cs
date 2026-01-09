using System;

namespace Shift.Contract
{
    public class DeleteAttemptPin
    {
        public Guid AttemptIdentifier { get; set; }
        public Guid QuestionIdentifier { get; set; }

        public int PinSequence { get; set; }
    }
}