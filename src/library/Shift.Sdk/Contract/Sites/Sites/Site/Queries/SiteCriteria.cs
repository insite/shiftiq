using System;

using Shift.Common;

namespace Shift.Contract
{
    public interface ISiteCriteria : IQueryByOrganization
    {
        QueryFilter Filter { get; set; }

        DateTimeOffset? LastChangeTimeSince { get; set; }
        DateTimeOffset? LastChangeTimeBefore { get; set; }
    }
}
