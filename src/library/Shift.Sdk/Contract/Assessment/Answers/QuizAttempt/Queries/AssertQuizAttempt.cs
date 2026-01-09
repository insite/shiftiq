using System;

using Shift.Common;

namespace Shift.Contract
{
    public class AssertQuizAttempt : Query<bool>
    {
        public Guid AttemptIdentifier { get; set; }
    }
}