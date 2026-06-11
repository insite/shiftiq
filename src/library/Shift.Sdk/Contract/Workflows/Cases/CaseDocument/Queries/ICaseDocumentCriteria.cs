using System;

using Shift.Common;

namespace Shift.Contract
{
    public interface ICaseDocumentCriteria : IQueryByOrganization
    {
        QueryFilter Filter { get; set; }

        DateTimeOffset? AttachmentPostedSince { get; set; }
        DateTimeOffset? AttachmentPostedBefore { get; set; }
    }
}
