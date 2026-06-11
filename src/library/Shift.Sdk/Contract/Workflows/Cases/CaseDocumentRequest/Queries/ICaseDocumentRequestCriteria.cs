using System;

using Shift.Common;

namespace Shift.Contract
{
    public interface ICaseDocumentRequestCriteria : IQueryByOrganization
    {
        QueryFilter Filter { get; set; }

        DateTimeOffset? RequestedTimeSince { get; set; }
        DateTimeOffset? RequestedTimeBefore { get; set; }
    }
}
