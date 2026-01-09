using System;

using Shift.Common;

namespace Shift.Contract
{
    public interface IPeriodCriteria
    {
        QueryFilter Filter { get; set; }

        Guid OrganizationId { get; set; }

        string Name { get; set; }

        DateTimeOffset? EndBefore { get; set; }
        DateTimeOffset? EndSince { get; set; }
        DateTimeOffset? StartBefore { get; set; }
        DateTimeOffset? StartSince { get; set; }
    }
}