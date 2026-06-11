using System;

using Shift.Common;

namespace Shift.Contract
{
    public interface IFileActivityCriteria : IQueryByOrganization
    {
        QueryFilter Filter { get; set; }

        DateTimeOffset? ActivityTimeSince { get; set; }
        DateTimeOffset? ActivityTimeBefore { get; set; }
    }
}
