using System;

using InSite.Application.Cases.Write;
using InSite.Common.Web.UI;
using InSite.UI.Admin.Workflow.Cases.Utilities;
using InSite.UI.Layout.Admin;

using Shift.Common;
using Shift.Constant;

namespace InSite.Admin.Issues.Comments.Forms
{
    public partial class Author : AdminBasePage, IHasParentLinkParameters
    {
        private Guid? CaseIdentifier => Guid.TryParse(Request["case"], out var result) ? result : (Guid?)null;

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            SaveButton.Click += SaveButton_Click;
            CommentInfo.IssueIdentifier = CaseIdentifier ?? Guid.Empty;
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            if (IsPostBack)
                return;

            PageHelper.AutoBindHeader(this);
            Open();
        }

        private void SaveButton_Click(object sender, EventArgs e)
        {
            var cmd = Save();
            if (cmd != null)
                CommentInfo.RedirectToReader();
        }

        private void Open()
        {
            var issue = CaseIdentifier.HasValue ? ServiceLocator.IssueSearch.GetIssue(CaseIdentifier.Value) : null;
            if (issue == null || !CaseVisibilityHelper.IsCaseVisible(issue.OrganizationIdentifier, issue.TopicUserIdentifier))
                CommentInfo.RedirectToSearch();

            CaseInfo.BindIssue(issue, User.TimeZone);
            CommentInfo.SetInputValues(issue, null);
            CancelButton.NavigateUrl = CommentInfo.GetReaderUrl();
        }

        private AuthorComment Save()
        {
            if (!IsValid)
                return null;

            if (string.IsNullOrEmpty(CommentInfo.CommentTextValue))
            {
                EditorStatus.AddMessage(AlertType.Error, "Text of comment can not be empty");
                return null;
            }

            var cmd = new AuthorComment(
                CaseIdentifier.Value, UniqueIdentifier.Create(),
                CommentInfo.CommentTextValue,
                CommentInfo.CommentCategoryValue,
                CommentInfo.CommentFlagValue,
                User.UserIdentifier,
                "Administrator",
                CommentInfo.CommentAssignedTo,
                null,
                CommentInfo.CommentSubCategoryValue,
                CommentInfo.CommentTageValue,
                DateTimeOffset.UtcNow,
                null,
                null,
                null
            );

            ServiceLocator.SendCommand(cmd);

            return cmd;
        }

        public string GetParentLinkParameters(IWebRoute parent)
            => CommentInfo.GetParentLinkParameters(parent);
    }
}