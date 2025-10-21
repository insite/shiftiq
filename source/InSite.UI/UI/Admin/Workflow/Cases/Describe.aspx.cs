using System;
using System.Web.UI;

using InSite.Application.Cases.Write;
using InSite.Application.Issues.Read;
using InSite.Common.Web;
using InSite.Common.Web.UI;
using InSite.Persistence;
using InSite.UI.Admin.Workflow.Cases.Utilities;
using InSite.UI.Layout.Admin;

using Shift.Common;

namespace InSite.Admin.Issues.Forms
{
    public partial class Describe : AdminBasePage, IHasParentLinkParameters
    {
        #region Properties

        private Guid? CaseIdentifier => Guid.TryParse(Request["case"], out var result) ? result : (Guid?)null;

        #endregion

        #region Initialization

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            SaveButton.Click += SaveButton_Click;
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            if (IsPostBack)
                return;

            var issue = CaseIdentifier.HasValue ? ServiceLocator.IssueSearch.GetIssue(CaseIdentifier.Value) : null;
            if (issue == null || !CaseVisibilityHelper.IsCaseVisible(issue.OrganizationIdentifier, issue.TopicUserIdentifier))
                RedirectToSearch();

            PageHelper.AutoBindHeader(this, qualifier: $"{issue.IssueTitle} <span class='fw-normal fs-md text-body-secondary'>Case #{issue.IssueNumber} - {issue.IssueType}</span>");

            CaseInfo.BindIssue(issue, User.TimeZone, true, true, false);
            SetInputValues(issue);

            CancelButton.NavigateUrl = GetOutlineUrl(CaseIdentifier.Value);
        }

        #endregion

        #region Setting/getting input values

        public void SetInputValues(VIssue issue)
        {
            IssueDescription.Text = issue.IssueDescription;
        }

        #endregion

        #region Event handlers

        private void SaveButton_Click(object sender, EventArgs e)
        {
            if (!Page.IsValid)
                return;

            var command = new DescribeIssue(
                CaseIdentifier.Value,
                IssueDescription.Text);

            ServiceLocator.SendCommand(command);

            RedirectToOutline();
        }

        #endregion

        #region Methods (redirect)

        private void RedirectToOutline() =>
            HttpResponseHelper.Redirect(GetOutlineUrl(CaseIdentifier.Value), true);

        private void RedirectToSearch() =>
            HttpResponseHelper.Redirect($"/ui/admin/workflow/cases/search", true);

        private string GetOutlineUrl(Guid issueIdentifier) =>
            $"/ui/admin/workflow/cases/outline?case={issueIdentifier}";

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