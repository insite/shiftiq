using System;

using Shift.Common;

namespace Shift.Contract
{
    public class AssertAttemptQuestion : Query<bool>
    {
        public Guid AttemptIdentifier { get; set; }
        public Guid QuestionIdentifier { get; set; }
    }
}