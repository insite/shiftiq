using System;

using Shift.Common;

namespace Shift.Contract
{
    public interface IGroupCriteria : IQueryByOrganization
    {
        QueryFilter Filter { get; set; }

        string GroupCode { get; set; }
        string[] GroupNames { get; set; }
        DateTimeOffset? GroupCreatedSince { get; set; }
        DateTimeOffset? GroupCreatedBefore { get; set; }
        DateTimeOffset? GroupExpirySince { get; set; }
        DateTimeOffset? GroupExpiryBefore { get; set; }
        DateTimeOffset? LastChangeTimeSince { get; set; }
        DateTimeOffset? LastChangeTimeBefore { get; set; }
    }
}
