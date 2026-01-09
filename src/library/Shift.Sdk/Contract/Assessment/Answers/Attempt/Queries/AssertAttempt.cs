using System;

using Shift.Common;

namespace Shift.Contract
{
    public class AssertAttempt : Query<bool>
    {
        public Guid AttemptIdentifier { get; set; }
    }
}