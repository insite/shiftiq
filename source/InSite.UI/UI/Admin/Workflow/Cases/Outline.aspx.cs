using System;

using InSite.Application.Issues.Read;
using InSite.Common.Web;
using InSite.UI.Admin.Workflow.Cases.Utilities;
using InSite.UI.Layout.Admin;

namespace InSite.Admin.Issues.Outlines.Forms
{
    public partial class Outline : AdminBasePage
    {
        protected Guid? CaseIdentifier => Guid.TryParse(Request["case"], out var value) ? (Guid?)value : null;

        protected string Panel => Request["panel"];

        private bool _isLoaded = false;
        private VIssue _issueQuery;

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            CommentsSection.LoadIssue = LoadIssue;
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            if (IsPostBack)
                return;

            if (!InitIssue())
            {
                RedirectToSearch();
                return;
            }

            SetInputValues();
        }

        private VIssue LoadIssue()
        {
            if (!InitIssue())
                RedirectToSearch();

            return _issueQuery;
        }

        private bool InitIssue()
        {
            if (CaseIdentifier == null)
                return false;

            if (!_isLoaded)
            {
                _issueQuery = ServiceLocator.IssueSearch.GetIssue(CaseIdentifier.Value);

                if (_issueQuery == null || !CaseVisibilityHelper.IsCaseVisible(_issueQuery.OrganizationIdentifier, _issueQuery.TopicUserIdentifier))
                    return false;

                _isLoaded = true;
            }

            return _issueQuery != null;
        }

        private void SetInputValues()
        {
            var issue = _issueQuery;

            PageHelper.AutoBindHeader(this, qualifier: $"{issue.IssueTitle} <span class='fw-normal fs-md text-body-secondary'>Case #{issue.IssueNumber} - {issue.IssueType}</span>");

            CaseSection.LoadData(_issueQuery);
            CommentsSection.LoadData(CaseIdentifier.Value);

            var respondentUserId = string.Equals(issue.IssueSource, "Survey Response")
                ? issue.TopicUserIdentifier
                : null;

            AttachmentsSection.BindModelToControls(CaseIdentifier.Value, respondentUserId);

            CommentNavItem.IsSelected = Request.QueryString["panel"] == "comments";
            AttachmentNavItem.IsSelected = Request.QueryString["panel"] == "attachments";

            if (respondentUserId == null || !issue.ResponseSessionIdentifier.HasValue)
                return;

            SurveyNavItem.IsSelected = Request.QueryString["panel"] == "forms";
            SurveyNavItem.Visible = CurrentSessionState.Identity.IsGranted("Admin/Forms/Submissions");
            SurveySection.BindModelToControls(CaseIdentifier.Value, issue.ResponseSessionIdentifier.Value);
        }

        private static void RedirectToSearch()
            => HttpResponseHelper.Redirect($"/ui/admin/workflow/cases/search", true);
    }
}