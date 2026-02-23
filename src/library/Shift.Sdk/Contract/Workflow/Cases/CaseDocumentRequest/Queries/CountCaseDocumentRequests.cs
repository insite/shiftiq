using System;

using Shift.Common;

namespace Shift.Contract
{
    public class CountCaseDocumentRequests : Query<int>, ICaseDocumentRequestCriteria
    {
        public Guid? OrganizationId { get; set; }
        public Guid? RequestedUserId { get; set; }

        public string RequestedFileDescription { get; set; }
        public string RequestedFileSubcategory { get; set; }
        public string RequestedFrom { get; set; }

        public DateTimeOffset? RequestedTime { get; set; }
    }
}