using System;

using Shift.Common;

namespace Shift.Contract
{
    public interface IGroupCriteria : IQueryByOrganization
    {
        QueryFilter Filter { get; set; }

        DateTimeOffset? GroupCreatedSince { get; set; }
        DateTimeOffset? GroupCreatedBefore { get; set; }
        DateTimeOffset? GroupExpirySince { get; set; }
        DateTimeOffset? GroupExpiryBefore { get; set; }
        DateTimeOffset? LastChangeTimeSince { get; set; }
        DateTimeOffset? LastChangeTimeBefore { get; set; }
    }
}
