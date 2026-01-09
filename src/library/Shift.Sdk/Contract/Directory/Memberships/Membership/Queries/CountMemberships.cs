using System;

using Shift.Common;

namespace Shift.Contract
{
    public class CountMemberships : Query<int>, IMembershipCriteria
    {
        public Guid? GroupIdentifier { get; set; }
        public Guid? ModifiedBy { get; set; }
        public Guid? OrganizationIdentifier { get; set; }
        public Guid? UserIdentifier { get; set; }

        public string AccountScope { get; set; }
        public string MembershipFunction { get; set; }

        public DateTimeOffset? MembershipEffective { get; set; }
        public DateTimeOffset? MembershipExpiry { get; set; }
        public DateTimeOffset? Modified { get; set; }
    }
}