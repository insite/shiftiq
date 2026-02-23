using System;

using Shift.Common;

namespace Shift.Contract
{
    public class CountCaseDocuments : Query<int>, ICaseDocumentCriteria
    {
        public Guid? OrganizationId { get; set; }

        public Guid? FileId { get; set; }
        public Guid? CaseId { get; set; }
        public Guid? PosterId { get; set; }

        public string FileName { get; set; }
        public string FileType { get; set; }

        public DateTimeOffset? AttachmentPosted { get; set; }
    }
}