using System;

using Shift.Common;

namespace Shift.Contract
{
    public interface ICaseDocumentRequestCriteria : IQueryByOrganization
    {
        QueryFilter Filter { get; set; }

        Guid? RequestedUserId { get; set; }

        string RequestedFileDescription { get; set; }
        string RequestedFileSubcategory { get; set; }
        string RequestedFrom { get; set; }

        DateTimeOffset? RequestedTime { get; set; }
    }
}