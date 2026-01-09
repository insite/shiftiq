using System;

namespace InSite.Application.Issues.Read
{
    public class QIssue
    {
        public Guid? AdministratorUserIdentifier { get; set; }
        public Guid? EmployerGroupIdentifier { get; set; }
        public Guid IssueIdentifier { get; set; }
        public Guid? IssueStatusIdentifier { get; set; }
        public Guid? LawyerUserIdentifier { get; set; }
        public Guid OrganizationIdentifier { get; set; }
        public Guid? OwnerUserIdentifier { get; set; }
        public Guid? ResponseSessionIdentifier { get; set; }
        public Guid? TopicUserIdentifier { get; set; }

        public string IssueDescription { get; set; }
        public string IssueSource { get; set; }
        public string IssueStatusCategory { get; set; }
        public string IssueTitle { get; set; }
        public string IssueType { get; set; }

        public int AttachmentCount { get; set; }
        public int CommentCount { get; set; }
        public int GroupCount { get; set; }
        public int IssueNumber { get; set; }
        public int PersonCount { get; set; }

        public DateTimeOffset? IssueClosed { get; set; }
        public DateTimeOffset IssueOpened { get; set; }
        public DateTimeOffset? IssueReported { get; set; }
        public DateTimeOffset? IssueStatusEffective { get; set; }

        public DateTimeOffset LastChangeTime { get; set; }
        public string LastChangeType { get; set; }
        public Guid LastChangeUser { get; set; }
    }
}
