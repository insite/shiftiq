using System;

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
    public partial class ChangeStatus : AdminBasePage, IHasParentLinkParameters
    {
        #region Properties

        private Guid CaseIdentifier => Guid.Parse(Request["case"]);

        #endregion

        #region Initialization

        private VIssue _issue;

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            _issue = ServiceLocator.IssueSearch.GetIssue(CaseIdentifier);

            if (_issue == null || !CaseVisibilityHelper.IsCaseVisible(_issue.OrganizationIdentifier, _issue.TopicUserIdentifier))
                RedirectToSearch();

            IssueStatus.IssueType = _issue.IssueType;
            IssueStatusEffective.Value = TimeZones.ConvertFromUtc(DateTimeOffset.UtcNow, User.TimeZone);

            SaveButton.Click += SaveButton_Click;
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            if (IsPostBack)
                return;

            var issue = _issue;

            PageHelper.AutoBindHeader(this, qualifier: $"{issue.IssueTitle} <span class='fw-normal fs-md text-body-secondary'>Case #{issue.IssueNumber} - {issue.IssueType}</span>");

            CaseInfo.BindIssue(_issue, User.TimeZone, true, false);

            CancelButton.NavigateUrl = GetOutlineUrl(CaseIdentifier);
        }

        #endregion

        #region Event handlers

        private void SaveButton_Click(object sender, EventArgs e)
        {
            if (!Page.IsValid)
                return;

            var issue = ServiceLocator.IssueSearch.GetIssue(CaseIdentifier);

            var status = IssueStatus.ValueAsGuid;
            if (status.HasValue && issue.IssueStatusIdentifier != status)
            {
                var command = new ChangeIssueStatus(CaseIdentifier, status.Value, IssueStatusEffective.Value ?? DateTimeOffset.UtcNow);
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