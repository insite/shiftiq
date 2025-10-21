using System;

using InSite.Application.Cases.Write;
using InSite.Application.Issues.Read;
using InSite.Common.Web;
using InSite.Common.Web.UI;
using InSite.UI.Admin.Workflow.Cases.Utilities;
using InSite.UI.Layout.Admin;

using Shift.Common;

namespace InSite.UI.Admin.Issues.Issues
{
    public partial class Delete : AdminBasePage, IHasParentLinkParameters
    {
        private Guid? CaseIdentifier => Guid.TryParse(Request["case"], out var value) ? value : (Guid?)null;

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            DeleteButton.Click += DeleteButton_Click;
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            if (!IsPostBack)
            {
                var issue = CaseIdentifier.HasValue ? ServiceLocator.IssueSearch.GetIssue(CaseIdentifier.Value) : null;

                if (issue == null || !CaseVisibilityHelper.IsCaseVisible(issue.OrganizationIdentifier, issue.TopicUserIdentifier))
                {
                    HttpResponseHelper.Redirect($"/ui/admin/workflow/cases/search");
                    return;
                }

                PageHelper.AutoBindHeader(this, qualifier: $"{issue.IssueTitle} <span class='fw-normal fs-md text-body-secondary'>Case #{issue.IssueNumber} - {issue.IssueType}</span>");

                CaseInfo.BindIssue(issue, User.TimeZone);

                var attachmentCount = ServiceLocator.IssueSearch.CountAttachments(new QIssueAttachmentFilter
                {
                    IssueIdentifier = CaseIdentifier.Value
                });

                var commentCount = ServiceLocator.IssueSearch.CountComments(new QIssueCommentFilter
                {
                    IssueIdentifier = CaseIdentifier.Value
                });

                var userCount = ServiceLocator.IssueSearch.CountUsers(new QIssueUserFilter
                {
                    Issuedentifier = CaseIdentifier.Value
                });

                AttachmentCount.Text = $"{attachmentCount:n0}";
                CommentCount.Text = $"{commentCount:n0}";
                UserCount.Text = $"{userCount:n0}";

                var hasUsers = userCount > 0;
                Confirm1Panel.Visible = hasUsers;

                var backUrl = $"/ui/admin/workflow/cases/outline?case={CaseIdentifier}";

                CancelButton.NavigateUrl = backUrl;
            }
        }

        private void DeleteButton_Click(object sender, EventArgs e)
        {
            var issue = ServiceLocator.IssueSearch.GetIssue(CaseIdentifier.Value);

            if (issue != null)
                DeleteIssue(issue);

            HttpResponseHelper.Redirect("/ui/admin/workflow/cases/search");
        }

        private void DeleteIssue(VIssue issue)
        {
            var comments = ServiceLocator.IssueSearch.GetComments(new QIssueCommentFilter
            {
                OrganizationIdentifier = Organization.Identifier,
                IssueIdentifier = issue.IssueIdentifier
            });

            foreach (var comment in comments)
                ServiceLocator.SendCommand(new DeleteComment(issue.IssueIdentifier, comment.CommentIdentifier));

            var attachments = ServiceLocator.IssueSearch.GetAttachments(new QIssueAttachmentFilter
            {
                OrganizationIdentifier = Organization.Identifier,
                IssueIdentifier = issue.IssueIdentifier
            });

            foreach (var attachment in attachments)
            {
                ServiceLocator.StorageService.Delete(attachment.FileIdentifier);
                ServiceLocator.SendCommand(new DeleteAttachment(issue.IssueIdentifier, attachment.FileName));
            }

            var users = ServiceLocator.IssueSearch.GetUsers(
                new QIssueUserFilter
                {
                    Issuedentifier = CaseIdentifier
                });

            foreach (var user in users)
                ServiceLocator.SendCommand(new UnassignUser(user.IssueIdentifier, user.UserIdentifier, user.IssueRole));

            ServiceLocator.SendCommand(new DeleteIssue(CaseIdentifier.Value));
        }

        public string GetParentLinkParameters(IWebRoute parent)
        {
            return parent.Name.EndsWith("/outline")
                ? $"case={CaseIdentifier}"
                : null;
        }

    }
}