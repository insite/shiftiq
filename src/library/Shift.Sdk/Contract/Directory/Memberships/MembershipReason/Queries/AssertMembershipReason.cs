using System;

using Shift.Common;

namespace Shift.Contract
{
    public class AssertMembershipReason : Query<bool>
    {
        public Guid ReasonIdentifier { get; set; }
    }
}