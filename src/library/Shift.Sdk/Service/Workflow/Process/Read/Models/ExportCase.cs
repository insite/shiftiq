using System;

namespace InSite.Application.Issues.Read
{
    public class ExportCase
    {
        public Guid? IssueClosedBy { get; set; }
        public Guid IssueIdentifier { get; set; }
        public Guid? IssueOpenedBy { get; set; }
        public Guid? TopicUserIdentifier { get; set; }
        public string AdministratorUserName { get; set; }
        public string IssueDescription { get; set; }
        public string IssueEmployerGroupName { get; set; }
        public string IssueEmployerGroupParentGroupName { get; set; }
        public string IssueSource { get; set; }
        public string IssueStatusCategory { get; set; }
        public string IssueStatusName { get; set; }
        public string IssueTitle { get; set; }
        public string IssueType { get; set; }
        public string LawyerUserName { get; set; }

        public string OwnerUserEmail { get; set; }
        public string OwnerUserName { get; set; }

        public string TopicAccountStatus { get; set; }
        public string TopicEmployerGroupName { get; set; }
        public string TopicUserName { get; set; }
        public string TopicUserEmail { get; set; }

        public int IssueNumber { get; set; }
        public int? IssueStatusSequence { get; set; }

        public DateTimeOffset? IssueClosed { get; set; }
        public DateTimeOffset IssueOpened { get; set; }
        public DateTimeOffset? IssueReported { get; set; }

        public DateTimeOffset LastChangeTime { get; set; }
        public string LastChangeType { get; set; }
        public Guid LastChangeUser { get; set; }
        public string LastChangeUserName { get; set; }
    }
}
