using System;

using Shift.Common;

namespace Shift.Contract
{
    public interface IMembershipCriteria : IQueryByOrganization
    {
        QueryFilter Filter { get; set; }

        string AccountScope { get; set; }

        Guid? GroupId { get; set; }
        Guid? UserId { get; set; }

        DateTimeOffset? MembershipEffectiveSince { get; set; }
        DateTimeOffset? MembershipEffectiveBefore { get; set; }
        DateTimeOffset? MembershipExpirySince { get; set; }
        DateTimeOffset? MembershipExpiryBefore { get; set; }
        DateTimeOffset? ModifiedSince { get; set; }
        DateTimeOffset? ModifiedBefore { get; set; }
    }
}
