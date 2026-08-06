using System;

using Shift.Common;

namespace Shift.Contract
{
    public interface IUserConnectionCriteria : IQueryByOrganization
    {
        QueryFilter Filter { get; set; }

        DateTimeOffset? ConnectedSince { get; set; }
        DateTimeOffset? ConnectedBefore { get; set; }
        DateTimeOffset? LastChangeTimeSince { get; set; }
        DateTimeOffset? LastChangeTimeBefore { get; set; }
    }
}
