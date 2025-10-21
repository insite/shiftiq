using System;

using InSite.Common.Web.UI;
using InSite.Persistence;
using InSite.Persistence.Plugin.CMDS;

using Shift.Constant;

namespace InSite.Cmds.Controls.Employees.Employments
{
    public partial class EmploymentGrid : BaseUserControl
    {
        #region Events

        public event EventHandler Deleted;

        private void OnDeleted() => Deleted?.Invoke(this, EventArgs.Empty);

        #endregion

        #region Security

        public bool ApplySecurityPermissions()
        {
            var hasPersmissions = CurrentSessionState.Identity.IsGranted(PermissionNames.Custom_CMDS_Fields);

            Visible = hasPersmissions;

            return hasPersmissions;
        }

        #endregion

        #region Properties

        private Guid UserIdentifier
        {
            get { return ViewState[nameof(UserIdentifier)] == null ? Guid.Empty : (Guid)ViewState[nameof(UserIdentifier)]; }
            set { ViewState[nameof(UserIdentifier)] = value; }
        }

        #endregion

        #region Initialization

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            Grid.DataBinding += Grid_DataBinding;
            Grid.RowCommand += Grid_RowCommand;
        }

        #endregion

        #region Event handler

        private void Grid_RowCommand(object sender, System.Web.UI.WebControls.GridViewCommandEventArgs e)
        {
            if (e.CommandName == "Delete")
            {
                var grid = (Grid)sender;
                var organizationId = grid.GetDataKey<Guid>(e);

                var entities = DepartmentProfileUserSearch.Select(
                    x => x.Department.OrganizationIdentifier == organizationId && x.UserIdentifier == UserIdentifier && x.IsPrimary);
                DepartmentProfileUserStore.Delete(
                    entities, CurrentSessionState.Identity.User.UserIdentifier, CurrentSessionState.Identity.Organization.OrganizationIdentifier);

                LoadData(UserIdentifier);

                OnDeleted();
            }
        }

        private void Grid_DataBinding(object sender, EventArgs e)
        {
            var webUser = CurrentSessionState.Identity;
            var key = webUser.IsInRole(CmdsRole.Programmers) || webUser.IsInRole(CmdsRole.SystemAdministrators)
                ? (Guid?)null
                : CurrentIdentityFactory.ActiveOrganizationIdentifier;

            Grid.DataSource = UserProfileRepository.SelectEmployments(UserIdentifier, key, Grid.PageIndex, Grid.PageSize);
        }

        #endregion

        #region Public methods

        public void LoadData(Guid userKey)
        {
            UserIdentifier = userKey;

            var webUser = CurrentSessionState.Identity;

            var limitOrganizationIdentifier = webUser.IsInRole(CmdsRole.Programmers) || webUser.IsInRole(CmdsRole.SystemAdministrators)
                ? (Guid?)null
                : CurrentIdentityFactory.ActiveOrganizationIdentifier;

            var rowCount = UserProfileRepository.SelectEmployments(UserIdentifier, limitOrganizationIdentifier, null, null).Rows.Count;

            Grid.VirtualItemCount = rowCount;
            Grid.DataBind();

            PrimaryProfilePanel.Visible = rowCount > 0;

            NoPrimaryProfilePanel.Visible = rowCount == 0;
            EmployeeProfileEditorLink1.NavigateUrl = string.Format("/ui/cmds/portal/validations/profiles/search?userID={0}&panel=primary-profile", UserIdentifier);
            EmployeeProfileEditorLink2.NavigateUrl = string.Format("/ui/cmds/portal/validations/profiles/search?userID={0}", UserIdentifier);

            // If the current screen is the Employee Profile Finder then the parent control should be visible only if there is data to display.
            var action = Route.Name;

            EmployeeProfileEditorLink2Panel.Visible = action != "ui/cmds/portal/validations/profiles/search";
        }

        #endregion
    }
}