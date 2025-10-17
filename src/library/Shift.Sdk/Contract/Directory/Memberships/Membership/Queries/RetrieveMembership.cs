using System;

using Shift.Common;

namespace Shift.Contract
{
    public class RetrieveMembership : Query<MembershipModel>
    {
        public Guid MembershipIdentifier { get; set; }
    }
}