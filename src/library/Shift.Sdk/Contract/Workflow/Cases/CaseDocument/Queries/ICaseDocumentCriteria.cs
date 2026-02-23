using System;

using Shift.Common;

namespace Shift.Contract
{
    public interface ICaseDocumentCriteria : IQueryByOrganization
    {
        QueryFilter Filter { get; set; }

        Guid? FileId { get; set; }
        Guid? CaseId { get; set; }
        Guid? PosterId { get; set; }

        string FileName { get; set; }
        string FileType { get; set; }

        DateTimeOffset? AttachmentPosted { get; set; }
    }
}