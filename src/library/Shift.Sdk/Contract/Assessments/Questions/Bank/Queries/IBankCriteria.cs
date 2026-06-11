using System;

using Shift.Common;

namespace Shift.Contract
{
    public interface IBankCriteria : IQueryByOrganization
    {
        QueryFilter Filter { get; set; }

        DateTimeOffset? LastChangeTimeSince { get; set; }
        DateTimeOffset? LastChangeTimeBefore { get; set; }
        bool? IsActive { get; set; }
    }
}