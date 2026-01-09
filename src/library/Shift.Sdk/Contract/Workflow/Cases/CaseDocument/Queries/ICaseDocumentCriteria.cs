using System;

using Shift.Common;

namespace Shift.Contract
{
    public interface ICaseDocumentCriteria
    {
        QueryFilter Filter { get; set; }

        Guid? FileIdentifier { get; set; }
        Guid? CaseIdentifier { get; set; }
        Guid? OrganizationIdentifier { get; set; }
        Guid? PosterIdentifier { get; set; }

        string FileName { get; set; }
        string FileType { get; set; }

        DateTimeOffset? AttachmentPosted { get; set; }
    }
}