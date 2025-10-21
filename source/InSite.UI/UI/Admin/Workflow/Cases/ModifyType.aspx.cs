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
using Shift.Constant;

namespace InSite.Admin.Issues.Issues.Forms
{
    public partial class ChangeType : AdminBasePage, IHasParentLinkParameters
    {
        #region Properties

        private Guid CaseIdentifier => Guid.Parse(Request["case"]);

        #endregion

        #region Initialization

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            SaveButton.Click += SaveButton_Click;

            IssueType.Settings.CollectionName = CollectionName.Cases_Classification_Type;
            IssueType.Settings.OrganizationIdentifier = Organization.OrganizationIdentifier;
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            if (IsPostBack)
                return;

            var issue = ServiceLocator.IssueSearch.GetIssue(CaseIdentifier);
            if (issue == null || !CaseVisibilityHelper.IsCaseVisible(issue.OrganizationIdentifier, issue.TopicUserIdentifier))
                RedirectToSearch();

            PageHelper.AutoBindHeader(this, qualifier: $"{issue.IssueTitle} <span class='fw-normal fs-md text-body-secondary'>Case #{issue.IssueNumber} - {issue.IssueType}</span>");

            CaseInfo.BindIssue(issue, User.TimeZone, true, false);
            SetInputValues(issue);

            CancelButton.NavigateUrl = GetOutlineUrl(CaseIdentifier);
        }

        #endregion

        #region Setting/getting input values

        public void SetInputValues(VIssue issue)
        {
            IssueType.Value = issue.IssueType;
        }

        #endregion

        #region Event handlers

        private void SaveButton_Click(object sender, EventArgs e)
        {
            if (!Page.IsValid)
                return;

            var issue = ServiceLocator.IssueSearch.GetIssue(CaseIdentifier);

            var issueType = IssueType.Value;
            if (issueType.IsNotEmpty() && issue.IssueType != issueType)
            {
                var command = new ChangeIssueType(
                    CaseIdentifier,
                    issueType);

                ServiceLocator.SendCommand(command);
            }

            RedirectToOutline();
        }

        #endregion

        #region Methods (redirect)

        private void RedirectToOutline() =>
            HttpResponseHelper.Redirect(GetOutlineUrl(CaseIdentifier), true);

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