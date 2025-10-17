using System;

namespace InSite.Application.Issues.Read
{
    public class QIssueAttachment
    {
        public Guid AttachmentIdentifier { get; set; }
        public Guid FileIdentifier { get; set; }
        public Guid IssueIdentifier { get; set; }
        public Guid? OrganizationIdentifier { get; set; }
        public Guid PosterIdentifier { get; set; }

        public string FileName { get; set; }
        public string FileType { get; set; }

        public DateTimeOffset AttachmentPosted { get; set; }

        public QIssueAttachment Clone()
            => (QIssueAttachment)MemberwiseClone();
    }
}
