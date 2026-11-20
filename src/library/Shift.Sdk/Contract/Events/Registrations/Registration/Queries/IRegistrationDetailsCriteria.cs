using System;

using Shift.Common;

namespace Shift.Contract
{
    public interface IRegistrationDetailsCriteria : IQueryByOrganization
    {
        QueryFilter Filter { get; set; }

        string ApprovalStatus { get; set; }

        string EventTitle { get; set; }

        DateTimeOffset? EventScheduledStartSince { get; set; }
        DateTimeOffset? EventScheduledStartBefore { get; set; }
        DateTimeOffset? EventScheduledEndSince { get; set; }
        DateTimeOffset? EventScheduledEndBefore { get; set; }
    }
}
