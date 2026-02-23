using System;

namespace InSite.Application.Issues.Read
{
    public class VIssueAttachment
    {
        public Guid? InputterUserIdentifier { get; set; }
        public Guid IssueIdentifier { get; set; }
        public Guid OrganizationIdentifier { get; set; }
        public Guid TopicUserIdentifier { get; set; }
        public Guid FileIdentifier { get; set; }

        public string FileName { get; set; }
        public string InputterUserName { get; set; }
        public string IssueTitle { get; set; }
        public string IssueType { get; set; }
        public string TopicUserEmail { get; set; }
        public string TopicUserName { get; set; }

        public int IssueNumber { get; set; }

        public DateTimeOffset FileUploaded { get; set; }
    }
}
