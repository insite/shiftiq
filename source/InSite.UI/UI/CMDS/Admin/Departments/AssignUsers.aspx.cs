using System;
using System.Data;
using System.Web.UI.WebControls;

using InSite.Common.Web.UI;
using InSite.Persistence;
using InSite.Persistence.Plugin.CMDS;
using InSite.UI.Layout.Admin;

using Shift.Constant;
using Shift.Sdk.UI;

using PersonFilter = InSite.Persistence.Plugin.CMDS.CmdsPersonFilter;

namespace InSite.Cmds.Actions.BulkTool.Assign
{
    public partial class PeopleToDepartments : AdminBasePage, ICmdsUserControl
    {
        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            Department.AutoPostBack = true;
            Department.ValueChanged += (s, a) => LoadEmployees();

            SaveButton.Click += (s, a) => Save();
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            if (IsPostBack)
                return;

            PageHelper.AutoBindHeader(this);

            Department.Filter.OrganizationIdentifier = CurrentIdentityFactory.ActiveOrganizationIdentifier;

            LoadEmployees();
        }

        private void LoadEmployees()
        {
            EmployeesPanel.Visible = Department.HasValue;

            if (!Department.HasValue)
                return;

            var filter = new PersonFilter();
            filter.OrganizationIdentifier = CurrentIdentityFactory.ActiveOrganizationIdentifier;

            var table = ContactRepository3.SelectSearchResultsWithDepartment(filter, Department.Value.Value, Organization.Identifier);

            EmployeeList.Items.Clear();

            foreach (DataRow row in table.Rows)
            {
                var userKey = (Guid)row["UserIdentifier"];
                var name = (string)row["FullName"];
                var isSelected = (bool)row["IsSelected"];

                var item = new ListItem(name, userKey.ToString());
                item.Selected = isSelected;

                EmployeeList.Items.Add(item);
            }
        }

        private void Save()
        {
            if (!Page.IsValid)
                return;

            foreach (ListItem item in EmployeeList.Items)
            {
                var department = Department.Value.Value;
                var user = Guid.Parse(item.Value);

                if (item.Selected)
                {
                    MembershipStore.Save(MembershipFactory.Create(user, department, Organization.Identifier, "Department"));
                    PersonStore.Insert(PersonFactory.Create(user, Organization.Identifier, null, true, null));
                }
                else
                {
                    MembershipStore.Delete(MembershipSearch.Select(department, user));
                }
            }

            LoadEmployees();

            ScreenStatus.AddMessage(AlertType.Success, "Your changes have been saved.");
        }
    }
}