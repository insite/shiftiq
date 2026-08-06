using System;

using Shift.Common;

namespace Shift.Contract
{
    public class CountMemberships : Query<int>, IMembershipCriteria
    {
        public Guid? GroupId { get; set; }
        public Guid? OrganizationId { get; set; }
        public Guid? UserId { get; set; }

        public string AccountScope { get; set; }

        public DateTimeOffset? MembershipEffectiveSince { get; set; }
        public DateTimeOffset? MembershipEffectiveBefore { get; set; }
        public DateTimeOffset? MembershipExpirySince { get; set; }
        public DateTimeOffset? MembershipExpiryBefore { get; set; }
        public DateTimeOffset? ModifiedSince { get; set; }
        public DateTimeOffset? ModifiedBefore { get; set; }
        public DateTimeOffset? LastChangeTimeSince { get; set; }
        public DateTimeOffset? LastChangeTimeBefore { get; set; }
    }
}
