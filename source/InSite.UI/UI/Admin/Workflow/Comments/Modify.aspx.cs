using System;

using InSite.Application.Cases.Write;
using InSite.Common.Web.UI;
using InSite.UI.Admin.Workflow.Cases.Utilities;
using InSite.UI.Layout.Admin;

using Shift.Common;
using Shift.Constant;

namespace InSite.Admin.Issues.Comments.Forms
{
    public partial class Revise : AdminBasePage, IHasParentLinkParameters
    {
        private Guid? CaseIdentifier => Guid.TryParse(Request["case"], out var result) ? result : (Guid?)null;

        private Guid? CommentIdentifier => Guid.TryParse(Request["comment"], out var result) ? result : (Guid?)null;

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            SaveButton.Click += SaveButton_Click;
            CommentInfo.IssueIdentifier = CaseIdentifier ?? Guid.Empty;
            CommentInfo.CommentIdentifier = CommentIdentifier ?? Guid.Empty;
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
            Save();

            CommentInfo.RedirectToReader();
        }

        private void Open()
        {
            var issue = CaseIdentifier.HasValue ? ServiceLocator.IssueSearch.GetIssue(CaseIdentifier.Value) : null;
            if (issue == null || !CaseVisibilityHelper.IsCaseVisible(issue.OrganizationIdentifier, issue.TopicUserIdentifier))
                CommentInfo.RedirectToSearch();

            var comment = CommentIdentifier.HasValue ? ServiceLocator.IssueSearch.GetComment(CommentIdentifier.Value) : null;
            if (comment == null)
                CommentInfo.RedirectToReader();

            CaseInfo.BindIssue(issue, User.TimeZone);
            CommentInfo.SetInputValues(issue, comment);
            RemoveButton.Visible = true;
            RemoveButton.NavigateUrl = $"/ui/admin/workflow/comments/delete?case={CaseIdentifier}&comment={CommentIdentifier}";
            CancelButton.NavigateUrl = CommentInfo.GetReaderUrl();
        }

        private ReviseComment Save()
        {
            if (!IsValid)
                return null;

            if (string.IsNullOrEmpty(CommentInfo.CommentTextValue))
            {
                EditorStatus.AddMessage(AlertType.Error, "Text of comment can not be empty");
                return null;
            }

            var cmd = new ReviseComment(
                CaseIdentifier.Value, CommentIdentifier.Value,
                CommentInfo.CommentTextValue,
                CommentInfo.CommentCategoryValue,
                CommentInfo.CommentFlagValue,
                User.UserIdentifier,
                DateTimeOffset.UtcNow,
                CommentInfo.CommentAssignedTo,
                null,
                CommentInfo.CommentSubCategoryValue,
                CommentInfo.CommentTageValue,
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
