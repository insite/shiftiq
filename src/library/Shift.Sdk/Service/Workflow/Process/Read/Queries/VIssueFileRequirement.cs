using System;

namespace InSite.Application.Issues.Read
{
    public class VIssueFileRequirement
    {
        public Guid IssueIdentifier { get; set; }
        public Guid OrganizationIdentifier { get; set; }
        public string RequestedFileCategory { get; set; }
        public string RequestedFileSubcategory { get; set; }
        public DateTimeOffset RequestedTime { get; set; }
        public Guid RequestedUserIdentifier { get; set; }
        public string RequestedUserName { get; set; }
        public string RequestedFrom { get; set; }
        public string RequestedFileDescription { get; set; }
        public string RequestedFileStatus { get; set; }
    }
}