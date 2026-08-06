using System;
using System.Collections.Generic;
using System.Linq;

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
    public partial class ModifyStatus : AdminBasePage, IHasParentLinkParameters
    {
        #region Properties

        private Guid CaseIdentifier => Guid.Parse(Request["case"]);

        #endregion

        #region Fields

        private VIssue _issue;
        private static readonly IReadOnlyCollection<string> _validCategories =
            new HashSet<string>(StringComparer.OrdinalIgnoreCase) { "Open", "Closed" };

        #endregion

        #region Initialization

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            _issue = ServiceLocator.IssueSearch.GetIssue(CaseIdentifier);

            if (_issue == null || !CaseVisibilityHelper.IsCaseVisible(_issue))
                RedirectToSearch();

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

            var statusCategory = Request.QueryString["category"];
            if (_validCategories.Contains(statusCategory))
                IssueStatus.StatusCategory = statusCategory;

            IssueStatus.IssueType = _issue.IssueType;

            IssueStatusEffective.Value = TimeZones.ConvertFromUtc(DateTimeOffset.UtcNow, User.TimeZone);

            CancelButton.NavigateUrl = GetOutlineUrl(CaseIdentifier);
        }

        #endregion

        #region Event handlers

        private void SaveButton_Click(object sender, EventArgs e)
        {
            if (!Page.IsValid)
                return;

            var issue = ServiceLocator.IssueSearch.GetIssue(CaseIdentifier);

            var statusId = IssueStatus.ValueAsGuid;
            if (statusId.HasValue && issue.IssueStatusIdentifier != statusId)
            {
                var statusCategory = ServiceLocator.IssueSearch.GetStatus(statusId.Value)?.StatusCategory;
                ServiceLocator.SendCommand(new ChangeIssueStatus(
                    CaseIdentifier, statusId.Value, IssueStatusEffective.Value ?? DateTimeOffset.UtcNow, statusCategory));
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
