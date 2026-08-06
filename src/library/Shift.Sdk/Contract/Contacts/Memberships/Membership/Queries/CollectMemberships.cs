using System;
using System.Collections.Generic;

using Shift.Common;

namespace Shift.Contract
{
    public class CollectMemberships : Query<IEnumerable<MembershipModel>>, IMembershipCriteria
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
