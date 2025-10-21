using System;

using InSite.Application.Cases.Write;
using InSite.Application.Contents.Read;
using InSite.Application.Issues.Read;
using InSite.Common.Web;
using InSite.Common.Web.UI;
using InSite.UI.Admin.Workflow.Cases.Utilities;
using InSite.UI.Layout.Admin;

using Shift.Common;

namespace InSite.Admin.Issues.Comments.Forms
{
    public partial class Delete : AdminBasePage, IHasParentLinkParameters
    {
        #region Properties

        private Guid? CaseIdentifier => Guid.TryParse(Request["case"], out var result) ? result : (Guid?)null;

        private Guid? CommentIdentifier => Guid.TryParse(Request["comment"], out var result) ? result : (Guid?)null;

        #endregion

        #region Loading

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            DeleteButton.Click += DeleteButton_Click;
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            if (!IsPostBack)
                Open();
        }

        #endregion

        #region Event handlers

        private void DeleteButton_Click(object sender, EventArgs e)
        {
            DeleteComment();

            RedirectToReader();
        }

        #endregion

        #region Database operations

        private void Open()
        {
            var issue = CaseIdentifier.HasValue ? ServiceLocator.IssueSearch.GetIssue(CaseIdentifier.Value) : null;
            if (issue == null || !CaseVisibilityHelper.IsCaseVisible(issue.OrganizationIdentifier, issue.TopicUserIdentifier))
                RedirectToSearch();

            var comment = CommentIdentifier.HasValue ? ServiceLocator.IssueSearch.GetComment(CommentIdentifier.Value) : null;
            if (comment == null)
                RedirectToReader();

            PageHelper.AutoBindHeader(this, null, $"{issue.IssueTitle} <span class='form-text'>{issue.IssueType} Case #{issue.IssueNumber}</span>");

            SetInputValues(issue, comment);
        }

        private void DeleteComment()
        {
            var cmd = new DeleteComment(CaseIdentifier.Value, CommentIdentifier.Value);

            ServiceLocator.SendCommand(cmd);
        }

        #endregion

        #region Settings/getting input values

        private void SetInputValues(VIssue issue, VComment comment)
        {
            CommentText.Text = Markdown.ToHtml(comment.CommentText);
            CommentAuthor.Text = (comment.RevisorUserName != null) ? comment.RevisorUserName : comment.AuthorUserName;
            CommentPosted.Text = GetLocalTime((comment.CommentRevised.HasValue ? comment.CommentRevised : comment.CommentPosted), User.TimeZone);

            CancelButton.NavigateUrl = GetReaderUrl();
        }

        private string GetLocalTime(DateTimeOffset? item, TimeZoneInfo tz)
        {
            return item.Format(tz, nullValue: "None");
        }
        #endregion

        #region Methods (redirect)

        private static void RedirectToSearch() => HttpResponseHelper.Redirect($"/ui/admin/workflow/cases/search", true);

        private void RedirectToReader()
        {
            var url = GetReaderUrl();

            HttpResponseHelper.Redirect(url, true);
        }

        private string GetReaderUrl()
        {
            return $"/ui/admin/workflow/cases/outline?case={CaseIdentifier}&panel=comments";
        }

        #endregion

        #region IHasParentLinkParameters

        public string GetParentLinkParameters(IWebRoute parent)
        {
            return parent.Name.EndsWith("/outline")
                ? $"case={CaseIdentifier}"
                : null;
        }

        #endregion
    }
}