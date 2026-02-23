using System;
using System.Collections.Generic;

using Shift.Common;

namespace Shift.Contract
{
    public class SearchCaseDocumentRequests : Query<IEnumerable<CaseDocumentRequestMatch>>, ICaseDocumentRequestCriteria
    {
        public Guid? OrganizationId { get; set; }
        public Guid? RequestedUserId { get; set; }

        public string RequestedFileDescription { get; set; }
        public string RequestedFileSubcategory { get; set; }
        public string RequestedFrom { get; set; }

        public DateTimeOffset? RequestedTime { get; set; }
    }
}