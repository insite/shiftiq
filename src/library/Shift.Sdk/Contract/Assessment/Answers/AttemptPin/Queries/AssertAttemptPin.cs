using System;

using Shift.Common;

namespace Shift.Contract
{
    public class AssertAttemptPin : Query<bool>
    {
        public Guid AttemptIdentifier { get; set; }
        public Guid QuestionIdentifier { get; set; }

        public int PinSequence { get; set; }
    }
}