using System;
using System.Collections.Generic;
using System.Data;
using System.Web.UI;
using System.Web.UI.WebControls;

using InSite.Common.Web.UI;
using InSite.Persistence;
using InSite.Persistence.Plugin.CMDS;

using Shift.Constant;

using Paging = Shift.Common.Paging;

namespace InSite.Custom.CMDS.Admin.People.Controls
{
    public partial class UserDepartments : BaseUserControl
    {
        private Guid UserIdentifier
        {
            get => (Guid?)ViewState[nameof(UserIdentifier)] ?? Guid.Empty;
            set => ViewState[nameof(UserIdentifier)] = value;
        }

        private DepartmentUserFilter Filter
        {
            get => (DepartmentUserFilter)ViewState[nameof(Filter)];
            set => ViewState[nameof(Filter)] = value;
        }

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            AddRole.Click += AddRoleClicked;

            DepartmentUserGrid.DataBinding += DepartmentUserGrid_DataBinding;
            DepartmentUserGrid.RowCommand += DepartmentUserGrid_RowCommand;

            RoleTypeSelector.AutoPostBack = true;
            RoleTypeSelector.SelectedIndexChanged += FilterChanged;
            RoleTypeSelector.Items.Add(new ListItem("Department employment", "Department") { Selected = true });
            RoleTypeSelector.Items.Add(new ListItem("Organization employment", "Organization") { Selected = true });
            RoleTypeSelector.Items.Add(new ListItem("Data access", "Administration") { Selected = true });

            FilterButton.Click += FilterChanged;
        }

        private void DepartmentUserGrid_DataBinding(object sender, EventArgs e)
        {
            if (DepartmentUserGrid.VirtualItemCount > 0)
            {
                if (Filter != null)
                    Filter.Paging = Paging.SetPage(DepartmentUserGrid.PageIndex + 1, DepartmentUserGrid.PageSize);

                var table = Filter.RoleType == null
                    ? new DataTable()
                    : ContactRepository3.SelectDepartmentUsers(Filter);

                DepartmentUserGrid.DataSource = table;
            }
            else
                DepartmentUserGrid.DataSource = new DataTable();
        }

        private void DepartmentUserGrid_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            if (e.CommandName == "Delete")
            {
                var grid = (Grid)sender;
                var departmentIdentifier = grid.GetDataKey<Guid>(e);

                MembershipStore.Delete(MembershipSearch.Select(departmentIdentifier, UserIdentifier));

                if (OrganizationIdentifier.Enabled)
                    OrganizationIdentifier.Value = null;

                DepartmentIdentifier.Value = null;

                LoadDepartmentUsers(false);

                OnRefreshed();
            }
        }

        public void LoadData(Guid userId, string userName)
        {
            UserIdentifier = userId;
            PersonName.Text = userName;
            DepartmentIdentifier.Filter.ExcludeUserIdentifier = userId;
            FilterChanged(null, null);
        }

        private void AddRoleClicked(object sender, EventArgs e)
        {
            if (!Page.IsValid || !OrganizationIdentifier.HasValue)
                return;

            AddDepartmentToUser();
            LoadDepartmentUsers();

            if (OrganizationIdentifier.Enabled)
                OrganizationIdentifier.Value = null;

            DepartmentIdentifier.Value = null;

            OnRefreshed();
        }

        private void AddDepartmentToUser()
        {
            var organization = OrganizationIdentifier.Value.Value;
            var actor = User.FullName;
            var department = DepartmentIdentifier.Value;

            var membershipFunction = MembershipDepartment.Checked ? "Department"
                    : MembershipOrganization.Checked ? "Organization"
                    : MembershipAdministration.Checked ? "Administration"
                    : null;

            if (department.HasValue)
            {
                MembershipStore.Save(MembershipFactory.Create(UserIdentifier, department.Value, organization, membershipFunction));
                PersonStore.Insert(PersonFactory.Create(UserIdentifier, organization, null, true, actor));
            }
            else
            {
                AddAllDepartmentsToPerson(membershipFunction);
            }
        }

        private void AddAllDepartmentsToPerson(string membershipFunction)
        {
            var organization = OrganizationIdentifier.Value.Value;
            var actor = User.FullName;

            var departments = DepartmentSearch.SelectCompanyDepartments(organization);

            foreach (var department in departments)
            {
                MembershipStore.Save(MembershipFactory.Create(UserIdentifier, department.DepartmentIdentifier, organization, membershipFunction));
                PersonStore.Insert(PersonFactory.Create(UserIdentifier, organization, null, true, actor));
            }
        }

        #region Security

        private DataTable _accessibleCompanies;

        public void ApplyPermissions()
        {
            var webUser = CurrentSessionState.Identity;

            var seeAllCompanies = webUser.IsInRole(CmdsRole.Programmers) ||
                                  webUser.IsInRole(CmdsRole.SystemAdministrators);

            var seeMyCompanies = seeAllCompanies || webUser.IsInRole(CmdsRole.OfficeAdministrators) ||
                                 webUser.IsInRole(CmdsRole.FieldAdministrators);

            if (!seeAllCompanies)
            {
                if (seeMyCompanies)
                {
                    OrganizationIdentifier.Filter.RequireMembershipForUserEmail = CurrentSessionState.Identity.User.Email;
                    OrganizationIdentifier.Value = null;
                }
                else
                {
                    OrganizationIdentifier.Value = CurrentIdentityFactory.ActiveOrganizationIdentifier;
                    OrganizationIdentifier.Enabled = false;
                }
            }
        }

        private DataTable GetAccessibleCompanies()
        {
            var user = CurrentSessionState.Identity.User;
            return _accessibleCompanies ??
                   (_accessibleCompanies = ContactRepository3.SelectPersonOrganizations(user.UserIdentifier));
        }

        protected bool IsDeleteVisible(object organization)
        {
            var webUser = CurrentSessionState.Identity;
            return webUser.IsInRole(CmdsRole.Programmers)
                   || webUser.IsInRole(CmdsRole.SystemAdministrators)
                   || GetAccessibleCompanies().Select($"OrganizationIdentifier = '{organization}'").Length > 0;
        }

        #endregion

        #region Refresh/Filter

        public event EventHandler Refreshed;

        private void OnRefreshed()
        {
            Refreshed?.Invoke(this, EventArgs.Empty);
        }

        private void CreateFilter()
        {
            var roleTypes = new List<string>();

            foreach (ListItem item in RoleTypeSelector.Items)
                if (item.Selected)
                    roleTypes.Add(item.Value);

            var filter = new DepartmentUserFilter
            {
                UserIdentifier = UserIdentifier,
                SearchText = FilterText.Text,
                RoleType = roleTypes.Count == 0 ? null : roleTypes.ToArray()
            };

            Filter = filter;
        }

        private void FilterChanged(object sender, EventArgs e)
        {
            CreateFilter();
            LoadDepartmentUsers();
        }

        private void LoadDepartmentUsers(bool resetPageIndex = true)
        {
            var count = Filter.RoleType == null ? 0 : ContactRepository3.CountDepartmentUsers(Filter);

            DepartmentUserGrid.VirtualItemCount = count;
            DepartmentUserGrid.Visible = count > 0;
            DepartmentUserGrid.DataBind();

            if (resetPageIndex)
                DepartmentUserGrid.PageIndex = 0;
        }

        #endregion
    }
}