using System;
using System.Text;

using InSite.Admin.Logs.Aggregates;
using InSite.Application.Issues.Read;
using InSite.Common.Web.UI;
using InSite.Persistence;

using Shift.Common;
using Shift.Sdk.UI;

namespace InSite.Admin.Issues.Outlines.Controls
{
    public partial class CaseSection : BaseUserControl
    {
        #region Properties

        protected Guid IssueIdentifier
        {
            get => (Guid)ViewState[nameof(IssueIdentifier)];
            set => ViewState[nameof(IssueIdentifier)] = value;
        }

        protected int IssueNumber
        {
            get => (int)ViewState[nameof(IssueNumber)];
            set => ViewState[nameof(IssueNumber)] = value;
        }

        #endregion

        public void LoadData(VIssue issue)
        {
            var person = issue.TopicUserIdentifier.HasValue
                ? PersonSearch.Select(issue.OrganizationIdentifier, issue.TopicUserIdentifier.Value)
                : null;
            var hasPerson = person != null;

            IssueIdentifier = issue.IssueIdentifier;
            IssueNumber = issue.IssueNumber;
            DetailsHeading.InnerText = $"Case #{issue.IssueNumber}";

            IssueStatus.Text = $"{issue.IssueStatusName} {issue.IssueStatusCategoryHtml}";

            IssueStatusTimestamp.Text = GetIssueTimestamp(issue.IssueIdentifier);

            IssueTitle.Text = issue.IssueTitle;
            IssueType.Text = issue.IssueType ?? "None";
            IssueDescription.Text = Markdown.ToHtml(issue.IssueDescription ?? "None");

            IssueReported.Text = issue.IssueReported.HasValue ? LocalizeTime(issue.IssueReported.Value) : "-";
            IssueOpened.Text = LocalizeTime(issue.IssueOpened);
            IssueClosed.Text = issue.IssueClosed.HasValue ? LocalizeTime(issue.IssueClosed.Value) : "-";

            AdministratorName.Text = issue.AdministratorUserName.HasValue() ? $"<a href=\"/ui/admin/contacts/people/edit?contact={issue.AdministratorUserIdentifier}\">{issue.AdministratorUserName}</a>" : "None";
            AssigneeName.Text = issue.TopicUserName.HasValue() ? $"<a href=\"/ui/admin/contacts/people/edit?contact={issue.TopicUserIdentifier}\">{issue.TopicUserName}</a>" : "None";
            EmployerName.Text = issue.IssueEmployerGroupName.HasValue() ? $"<a href=\"/ui/admin/contacts/groups/edit?contact={issue.IssueEmployerGroupIdentifier}\">{issue.IssueEmployerGroupName}</a>" : "None";
            OwnerName.Text = issue.OwnerUserName.HasValue() ? $"<a href=\"/ui/admin/contacts/people/edit?contact={issue.OwnerUserIdentifier}\">{issue.OwnerUserName}</a>" : "None";

            if (issue.TopicUserIdentifier.HasValue)
            {
                var status = PersonCriteria.BindFirst(
                    x => x.MembershipStatus,
                    new PersonFilter
                    {
                        OrganizationIdentifier = issue.OrganizationIdentifier,
                        UserIdentifier = issue.TopicUserIdentifier.Value
                    });

                if (status != null)
                {
                    if (status != null)
                        AssigneeMembershipStatus.Text = $"<span class='badge bg-info'>{status.ItemName}</span>";
                }
            }

            LawyerName.Text = issue.LawyerUserName.HasValue() ? $"<a href=\"/ui/admin/contacts/people/edit?contact={issue.LawyerUserIdentifier}\">{issue.LawyerUserName}</a>" : "None";

            ReopenIssueButton.Visible = issue.IssueStatusCategory == "Closed";
            if (ReopenIssueButton.Visible)
                ReopenIssueButton.NavigateUrl = $"/ui/admin/workflow/cases/modify-status?case={issue.IssueIdentifier}&category=open";

            CloseIssueLink.Visible = issue.IssueStatusCategory != "Closed";
            if (CloseIssueLink.Visible)
                CloseIssueLink.NavigateUrl = $"/ui/admin/workflow/cases/modify-status?case={issue.IssueIdentifier}&category=closed";

            MoreInfoButton.Visible = hasPerson;

            if (hasPerson)
            {
                foreach (var item in InSite.Admin.Contacts.People.Forms.Report.MoreInfoItems)
                    MoreInfoButton.Items.Add(new DropDownButtonLinkItem
                    {
                        Text = item.Text,
                        NavigateUrl = $"/ui/admin/contacts/people/report?contact={person.UserIdentifier}#{item.Anchor}",
                        Target = "_blank"
                    });
            }

            ChangeIssueType.NavigateUrl = $"/ui/admin/workflow/cases/modify-type?case={issue.IssueIdentifier}";
            ChangeIssueStatus.NavigateUrl = $"/ui/admin/workflow/cases/modify-status?case={issue.IssueIdentifier}";
            ChangeIssueTitle.NavigateUrl = $"/ui/admin/workflow/cases/modify-title?case={issue.IssueIdentifier}";
            ChangeIssueDescription.NavigateUrl = $"/ui/admin/workflow/cases/describe?case={issue.IssueIdentifier}";
            ChangeAdministrator.NavigateUrl = $"/ui/admin/workflow/cases/assign-contacts?case={issue.IssueIdentifier}";
            ChangeAssignee.NavigateUrl = $"/ui/admin/workflow/cases/assign-contacts?case={issue.IssueIdentifier}";
            ChangeLawyer.NavigateUrl = $"/ui/admin/workflow/cases/assign-contacts?case={issue.IssueIdentifier}";
            ChangeOwner.NavigateUrl = $"/ui/admin/workflow/cases/assign-contacts?case={issue.IssueIdentifier}";

            // Measurements

            ViewHistoryLink.NavigateUrl = Outline.GetUrl(issue.IssueIdentifier, $"/ui/admin/workflow/cases/outline?case={issue.IssueIdentifier}");
            DownloadLink.NavigateUrl = $"/ui/admin/workflow/cases/download?case={issue.IssueIdentifier}";
            DeleteLink.NavigateUrl = $"/ui/admin/workflow/cases/delete?case={issue.IssueIdentifier}";
            DuplicateLink.NavigateUrl = $"/ui/admin/workflow/cases/open?case={issue.IssueIdentifier}&action=duplicate";
            EmailSenderButton.NavigateUrl = $"/ui/admin/workflow/cases/send-email?contact={issue.TopicUserIdentifier}&type=correspondence&issueId={issue.IssueIdentifier}";
        }

        private static string GetIssueTimestamp(Guid id)
        {
            var issue = ServiceLocator.IssueSearch.GetIssue(id);

            var sb = new StringBuilder();

            sb.Append(UserSearch.GetTimestampHtml(issue.LastChangeType, issue.LastChangeTime, issue.LastChangeUserName));

            return sb.ToString();
        }
    }
}