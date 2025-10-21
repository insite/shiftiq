using System;
using System.Collections.Generic;
using System.Web.UI;
using System.Web.UI.WebControls;

using InSite.Application.Users.Write;
using InSite.Common.Web.UI;
using InSite.Persistence;
using InSite.Persistence.Plugin.CMDS;
using InSite.UI.Layout.Admin;

using Shift.Common;
using Shift.Constant;
using Shift.Sdk.UI;

using PersonFilter = InSite.Persistence.Plugin.CMDS.CmdsPersonFilter;

namespace InSite.Cmds.Actions.BulkTool.Assign
{
    public partial class AssignManagers : AdminBasePage, ICmdsUserControl
    {
        private PersonFinderSecurityInfoWrapper _finderSecurityInfo;
        private PersonFinderSecurityInfoWrapper FinderSecurityInfo =>
            _finderSecurityInfo ?? (_finderSecurityInfo = new PersonFinderSecurityInfoWrapper(ViewState));

        public override void ApplyAccessControl()
        {
            if (!Identity.IsGranted("cmds/users/assign-managers"))
                CreateAccessDeniedException();

            FinderSecurityInfo.LoadPermissions();

            var isDataAccessVisible = Identity.IsInRole(CmdsRole.Managers)
                                      || Identity.IsInRole(CmdsRole.OfficeAdministrators)
                                      || Identity.IsInRole(CmdsRole.Programmers)
                                      || Identity.IsInRole(CmdsRole.SystemAdministrators);

            if (!isDataAccessVisible)
                RoleTypeSelector.Items.Remove(
                    RoleTypeSelector.Items.FindByValue(MembershipType.Administration));
        }

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            Department.AutoPostBack = true;
            Department.ValueChanged += (s, a) => LoadManagerData();

            SubType.AutoPostBack = true;
            SubType.SelectedIndexChanged += (s, a) => LoadManagerData();

            ManagerSelector.AutoPostBack = true;
            ManagerSelector.ValueChanged += (s, a) => LoadEmployees();

            RoleTypeSelector.AutoPostBack = true;
            RoleTypeSelector.SelectedIndexChanged += (s, a) => LoadEmployees();

            SaveButton.Click += (s, a) => Save();
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            if (!IsPostBack)
                LoadData();
        }

        protected override void OnPreRender(EventArgs e)
        {
            SelectAllButton.OnClientClick =
                string.Format("return setCheckboxes('{0}', true);", EmployeesColumn.ClientID);
            UnselectAllButton.OnClientClick =
                string.Format("return setCheckboxes('{0}', false);", EmployeesColumn.ClientID);

            base.OnPreRender(e);
        }

        private void LoadData()
        {
            PageHelper.AutoBindHeader(this);

            Department.Filter.OrganizationIdentifier = CurrentIdentityFactory.ActiveOrganizationIdentifier;
            Department.Filter.UserIdentifier =
                FinderSecurityInfo.CanSeeAllDepartments || Identity.HasAccessToAllCompanies
                    ? (Guid?)null
                    : User.UserIdentifier;
            Department.Value = null;

            LoadManagerData();
        }

        private void LoadManagerData()
        {
            var category = SubType.SelectedValue.ToEnum(RelationCategory.Manager);

            ManagerSelector.Filter.RelationCategory = category;
            ManagerSelector.Filter.DepartmentIdentifier = Department.Value ?? Guid.Empty;

            if (category == RelationCategory.Manager || category == RelationCategory.Supervisor)
                ManagerSelector.Filter.RoleType = new[] { MembershipType.Organization, MembershipType.Department };
            else if (category == RelationCategory.Validator)
                ManagerSelector.Filter.RoleType = new[] { MembershipType.Organization, MembershipType.Department, MembershipType.Administration };

            if (!FinderSecurityInfo.CanSeeAllCompanyPeople && !Identity.HasAccessToAllCompanies)
                ManagerSelector.Filter.ParentUserIdentifier = User.UserIdentifier;

            ManagerSelector.Value = null;

            LoadEmployees();
        }

        private void LoadEmployees()
        {
            EmployeesColumn.Visible = ManagerSelector.HasValue && Department.HasValue;

            if (!EmployeesColumn.Visible)
                return;

            var membershipTypes = new List<string>();

            foreach (System.Web.UI.WebControls.ListItem item in RoleTypeSelector.Items)
                if (item.Selected)
                    membershipTypes.Add(item.Value);

            if (membershipTypes.Count == 0)
            {
                EmployeesColumn.Visible = false;
                return;
            }

            var filter = new PersonFilter
            {
                DepartmentIdentifier = Department.Value,
                KeyeraRoles = new[] { CmdsRole.Workers }
            };

            if (!FinderSecurityInfo.CanSeeAllCompanyPeople && !Identity.HasAccessToAllCompanies)
                filter.ParentUserIdentifier = User.UserIdentifier;

            filter.ExcludeUserIdentifier = ManagerSelector.Value;
            filter.RoleType = membershipTypes.ToArray();

            var relationCategory = SubType.SelectedValue.ToEnum<RelationCategory>();
            var table = ContactRepository3.SelectPersonsWithManagerInfo(filter, ManagerSelector.Value.Value, relationCategory, Organization.Identifier);

            NoEmployees.Visible = table.Rows.Count == 0;
            SelectEmployeesPanel.Visible = table.Rows.Count > 0;

            if (table.Rows.Count > 0)
            {
                Employees.DataSource = table;
                Employees.DataBind();
            }
        }

        private void Save()
        {
            if (!Page.IsValid || !EmployeesColumn.Visible)
                return;

            foreach (RepeaterItem item in Employees.Items)
            {
                var userKeyCtrl = (ITextControl)item.FindControl("UserIdentifier");
                var assignedCtrl = (ICheckBoxControl)item.FindControl("Assigned");

                var isAssigned = assignedCtrl.Checked;
                var fromUserIdentifier = ManagerSelector.Value.Value;
                var toUserIdentifier = Guid.Parse(userKeyCtrl.Text);

                if (isAssigned)
                    CreateUserConnection(fromUserIdentifier, toUserIdentifier);
                else
                    UserConnectionStore.Delete(fromUserIdentifier, toUserIdentifier);
            }

            LoadEmployees();

            ScreenStatus.AddMessage(AlertType.Success, "Your changes have been saved.");
        }

        private void CreateUserConnection(Guid from, Guid to)
        {
            var c = UserConnectionSearch.Select(from, to)
                ?? new UserConnection
                {
                    FromUserIdentifier = from,
                    ToUserIdentifier = to
                };

            if (SubType.SelectedValue == "Leader")
                c.IsLeader = true;

            if (SubType.SelectedValue == "Manager")
                c.IsManager = true;

            if (SubType.SelectedValue == "Supervisor")
                c.IsSupervisor = true;

            if (SubType.SelectedValue == "Validator")
                c.IsValidator = true;

            ServiceLocator.SendCommand(new ConnectUser(c.FromUserIdentifier, c.ToUserIdentifier, c.IsLeader, c.IsManager, c.IsSupervisor, c.IsValidator, c.Connected));
        }
    }
}