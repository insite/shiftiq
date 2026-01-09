using System;

using Shift.Common;

namespace Shift.Contract
{
    public class RetrieveAttempt : Query<AttemptModel>
    {
        public Guid AttemptIdentifier { get; set; }
    }
}