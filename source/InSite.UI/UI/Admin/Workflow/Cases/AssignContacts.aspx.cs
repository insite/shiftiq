using System;
using System.Web.UI;

using Shift.Common.Timeline.Commands;

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
    public partial class AssignUsers : AdminBasePage, IHasParentLinkParameters
    {
        #region Properties

        private Guid? CaseIdentifier => Guid.TryParse(Request["case"], out var result) ? result : (Guid?)null;

        #endregion

        #region Initialization

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            AssigneeID.AutoPostBack = true;
            AssigneeID.ValueChanged += AssigneeID_ValueChanged;

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

            OwnerUserIdentifier.Filter.GroupIdentifier = Organization.AdministratorGroupIdentifier;
            OwnerUserIdentifier.Filter.IsAdministrator = true;

            CaseInfo.BindIssue(issue, User.TimeZone);
            SetInputValues(issue);

            SetupHelps();

            CancelButton.NavigateUrl = GetOutlineUrl(CaseIdentifier.Value);
        }

        private void SetupHelps()
        {
            AssigneeHelp.InnerText = "The Member of the case.";

            if (AssigneeID.HasValue)
            {
                var assignee = PersonCriteria.BindFirst(
                    x => new { x.MembershipStatus, x.User.Email },
                    new PersonFilter
                    {
                        OrganizationIdentifier = Organization.Identifier,
                        UserIdentifier = AssigneeID.Value.Value
                    });

                if (assignee != null)
                {
                    AssigneeHelp.InnerText = $"{assignee.Email} - {assignee.MembershipStatus?.ItemName}";
                }
            }
        }

        #endregion

        #region Setting/getting input values

        public void SetInputValues(VIssue issue)
        {
            ManagerIdentifier.Value = issue.AdministratorUserIdentifier;
            AssigneeID.Value = issue.TopicUserIdentifier;
            LawyerID.ValueAsGuid = issue.LawyerUserIdentifier;
            OwnerUserIdentifier.Value = issue.OwnerUserIdentifier;
        }

        #endregion

        #region Event handlers

        private void AssigneeID_ValueChanged(object sender, EventArgs e)
        {
            SetupHelps();
        }

        private void SaveButton_Click(object sender, EventArgs e)
        {
            if (!Page.IsValid)
                return;

            var issue = ServiceLocator.IssueSearch.GetIssue(CaseIdentifier.Value);

            {
                var command = GetChangeUserCommand(issue.AdministratorUserIdentifier, ManagerIdentifier.Value, CaseIdentifier.Value, "Administrator");

                if (command != null) ServiceLocator.SendCommand(command);
            }

            {
                var command = GetChangeUserCommand(issue.TopicUserIdentifier, AssigneeID.Value, CaseIdentifier.Value, "Topic");

                if (command != null) ServiceLocator.SendCommand(command);
            }

            {
                var command = GetChangeUserCommand(issue.LawyerUserIdentifier, LawyerID.ValueAsGuid, CaseIdentifier.Value, "Lawyer");

                if (command != null) ServiceLocator.SendCommand(command);
            }

            {
                var command = GetChangeUserCommand(issue.OwnerUserIdentifier, OwnerUserIdentifier.Value, CaseIdentifier.Value, "Owner");

                if (command != null) ServiceLocator.SendCommand(command);
            }

            RedirectToOutline();
        }

        private Command GetChangeUserCommand(Guid? oldUserID, Guid? newUserID, Guid issueID, string role)
        {
            if (oldUserID != newUserID)
            {
                if (newUserID.HasValue)
                    return new AssignUser(
                        issueID,
                        newUserID.Value,
                        role);

                if (oldUserID.HasValue)
                    return new UnassignUser(
                        issueID,
                        oldUserID.Value,
                        role);
            }

            return null;
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