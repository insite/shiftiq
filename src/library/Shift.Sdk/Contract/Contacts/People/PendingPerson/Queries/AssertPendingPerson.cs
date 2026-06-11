using System;

using Shift.Common;

namespace Shift.Contract
{
    public class AssertPendingPerson : Query<bool>
    {
        public Guid PendingId { get; set; }
    }
}
