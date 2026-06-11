using System;
using System.Collections.Generic;

using Shift.Common;

namespace Shift.Contract
{
    public class CollectCaseDocumentRequests : Query<IEnumerable<CaseDocumentRequestModel>>, ICaseDocumentRequestCriteria
    {
        public Guid? OrganizationId { get; set; }

        public DateTimeOffset? RequestedTimeSince { get; set; }
        public DateTimeOffset? RequestedTimeBefore { get; set; }
    }
}
