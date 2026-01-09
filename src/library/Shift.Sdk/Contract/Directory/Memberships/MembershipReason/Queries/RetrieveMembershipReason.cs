using System;

using Shift.Common;

namespace Shift.Contract
{
    public class RetrieveMembershipReason : Query<MembershipReasonModel>
    {
        public Guid ReasonIdentifier { get; set; }
    }
}