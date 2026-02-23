using System;

namespace Shift.Contract
{
    public class ModifyCaseDocument
    {
        public Guid AttachmentId { get; set; }
        public Guid FileId { get; set; }
        public Guid CaseId { get; set; }
        public Guid? OrganizationId { get; set; }
        public Guid PosterId { get; set; }

        public string FileName { get; set; }
        public string FileType { get; set; }

        public DateTimeOffset AttachmentPosted { get; set; }
    }
}