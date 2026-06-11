using System;

using Shift.Common;

namespace Shift.Contract
{
    public class CountCaseDocumentRequests : Query<int>, ICaseDocumentRequestCriteria
    {
        public Guid? OrganizationId { get; set; }

        public DateTimeOffset? RequestedTimeSince { get; set; }
        public DateTimeOffset? RequestedTimeBefore { get; set; }
    }
}
