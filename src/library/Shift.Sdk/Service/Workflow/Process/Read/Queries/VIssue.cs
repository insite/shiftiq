using System;
using System.Collections.Generic;
using System.Text;

using InSite.Application.Contacts.Read;

using Shift.Common;

namespace InSite.Application.Issues.Read
{
    public class VIssue
    {
        public Guid? AdministratorUserIdentifier { get; set; }
        public Guid? IssueClosedBy { get; set; }
        public Guid? IssueEmployerGroupIdentifier { get; set; }
        public Guid? IssueEmployerGroupParentGroupIdentifier { get; set; }
        public Guid IssueIdentifier { get; set; }
        public Guid? IssueOpenedBy { get; set; }
        public Guid? IssueStatusIdentifier { get; set; }
        public Guid? LawyerUserIdentifier { get; set; }
        public Guid OrganizationIdentifier { get; set; }
        public Guid? OwnerUserIdentifier { get; set; }
        public Guid? TopicUserIdentifier { get; set; }
        public Guid? ResponseSessionIdentifier { get; set; }

        public string AdministratorUserName { get; set; }
        public string IssueDescription { get; set; }
        public string IssueEmployerGroupName { get; set; }
        public string IssueEmployerGroupParentGroupName { get; set; }
        public string IssueSource { get; set; }
        public string IssueStatusCategory { get; set; }
        public string IssueStatusName { get; set; }
        public string IssueStatusDescription { get; set; }
        public string IssueStatusDisplay { get; set; }
        public string IssueTitle { get; set; }
        public string IssueType { get; set; }
        public string LawyerUserName { get; set; }

        public string OwnerUserEmail { get; set; }
        public string OwnerUserFirstName { get; set; }
        public string OwnerUserLastName { get; set; }
        public string OwnerUserName { get; set; }

        public string TopicAccountStatus { get; set; }
        public string TopicEmployerGroupName { get; set; }
        public string TopicGroupNames { get; set; }
        public string TopicUserName { get; set; }
        public string TopicUserEmail { get; set; }

        public int AttachmentCount { get; set; }
        public int CommentCount { get; set; }
        public int IssueNumber { get; set; }
        public int? IssueStatusSequence { get; set; }
        public int PersonCount { get; set; }

        public DateTimeOffset? IssueClosed { get; set; }
        public DateTimeOffset IssueOpened { get; set; }
        public DateTimeOffset? IssueReported { get; set; }
        public DateTimeOffset? IssueStatusEffective { get; set; }

        public DateTimeOffset LastChangeTime { get; set; }
        public string LastChangeType { get; set; }
        public Guid LastChangeUser { get; set; }
        public string LastChangeUserName { get; set; }

        // Navigation Properties

        public virtual VUser Administrator { get; set; }
        public virtual VUser Lawyer { get; set; }
        public virtual VUser Owner { get; set; }
        public virtual VUser Topic { get; set; }

        public virtual ICollection<VIssueUser> VUsers { get; set; }

        public string IssueDescriptionHtml
        {
            get
            {
                return Markdown.ToHtml(IssueDescription);
            }
        }

        public string IssueStatusCategoryHtml
        {
            get
            {
                var sb = new StringBuilder();

                if (IssueStatusCategory == "Open")
                    sb.Append("<span class='badge bg-success'><i class='far fa-circle me-1'></i>Open</span>");
                else if (IssueStatusCategory == "Closed")
                    sb.Append("<span class='badge bg-danger'><i class='fas fa-circle me-1'></i>Closed</span>");
                else
                    sb.Append($"<span class='badge bg-custom-default'>{IssueStatusCategory}</span>");

                return sb.ToString();
            }
        }

        public VIssue()
        {
            VUsers = new HashSet<VIssueUser>();
        }
    }
}
