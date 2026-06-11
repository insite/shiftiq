using System;

using Shift.Common;

namespace Shift.Contract
{
    public class CountCaseDocuments : Query<int>, ICaseDocumentCriteria
    {
        public Guid? OrganizationId { get; set; }

        public DateTimeOffset? AttachmentPostedSince { get; set; }
        public DateTimeOffset? AttachmentPostedBefore { get; set; }
    }
}
