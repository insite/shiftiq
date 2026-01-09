using System;
using System.Collections.Generic;

using Shift.Common;

namespace Shift.Contract
{
    public class CollectCaseDocumentRequests : Query<IEnumerable<CaseDocumentRequestModel>>, ICaseDocumentRequestCriteria
    {
        public Guid? OrganizationIdentifier { get; set; }
        public Guid? RequestedUserIdentifier { get; set; }

        public string RequestedFileDescription { get; set; }
        public string RequestedFileSubcategory { get; set; }
        public string RequestedFrom { get; set; }

        public DateTimeOffset? RequestedTime { get; set; }
    }
}