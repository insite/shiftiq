using System;

namespace InSite.Application.Issues.Read
{
    public class QIssueFileRequirement
    {
        public Guid IssueIdentifier { get; set; }
        public Guid OrganizationIdentifier { get; set; }
        public Guid RequestedUserIdentifier { get; set; }
        public string RequestedFileCategory { get; set; }
        public string RequestedFileSubcategory { get; set; }
        public string RequestedFileDescription { get; set; }
        public string RequestedFileStatus { get; set; }
        public DateTimeOffset RequestedTime { get; set; }
        public string RequestedFrom { get; set; }
    
        public QIssueFileRequirement Clone()
            => (QIssueFileRequirement)MemberwiseClone();
    }
}