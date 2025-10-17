using System;

using Shift.Common;

namespace Shift.Contract
{
    public class AssertMembership : Query<bool>
    {
        public Guid MembershipIdentifier { get; set; }
    }
}