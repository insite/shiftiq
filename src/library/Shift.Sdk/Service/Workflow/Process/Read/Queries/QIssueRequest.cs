using System;

namespace InSite.Application.Issues.Read
{
    public class QIssueRequest
    {
        public Guid IssueIdentifier { get; set; }
        public Guid OrganizationIdentifier { get; set; }
        public string RequestedFileCategory { get; set; }
        public DateTimeOffset RequestedTime { get; set; }
        public Guid RequestedUserIdentifier { get; set; }
    }
}
