using System;

namespace Shift.Contract
{
    public class CreateCaseDocument
    {
        public Guid AttachmentIdentifier { get; set; }
        public Guid FileIdentifier { get; set; }
        public Guid CaseIdentifier { get; set; }
        public Guid? OrganizationIdentifier { get; set; }
        public Guid PosterIdentifier { get; set; }

        public string FileName { get; set; }
        public string FileType { get; set; }

        public DateTimeOffset AttachmentPosted { get; set; }
    }
}