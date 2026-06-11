using System;
using System.Collections.Generic;

using Shift.Common;

namespace Shift.Contract
{
    public class CollectCaseDocuments : Query<IEnumerable<CaseDocumentModel>>, ICaseDocumentCriteria
    {
        public Guid? OrganizationId { get; set; }

        public DateTimeOffset? AttachmentPostedSince { get; set; }
        public DateTimeOffset? AttachmentPostedBefore { get; set; }
    }
}
