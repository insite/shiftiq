using System;

using Shift.Common;

namespace Shift.Contract
{
    public class RetrieveAttemptPin : Query<AttemptPinModel>
    {
        public Guid AttemptIdentifier { get; set; }
        public Guid QuestionIdentifier { get; set; }

        public int PinSequence { get; set; }
    }
}