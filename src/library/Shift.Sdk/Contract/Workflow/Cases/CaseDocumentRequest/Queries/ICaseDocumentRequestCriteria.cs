using System;

using Shift.Common;

namespace Shift.Contract
{
    public interface ICaseDocumentRequestCriteria
    {
        QueryFilter Filter { get; set; }
        
        Guid? OrganizationIdentifier { get; set; }
        Guid? RequestedUserIdentifier { get; set; }

        string RequestedFileDescription { get; set; }
        string RequestedFileSubcategory { get; set; }
        string RequestedFrom { get; set; }

        DateTimeOffset? RequestedTime { get; set; }
    }
}