using System;
using System.Collections.Generic;
using System.Web.UI.WebControls;

using InSite.Common.Web;
using InSite.Persistence;
using InSite.UI.Layout.Admin;

using Shift.Constant;
using Shift.Sdk.UI;

namespace InSite.Custom.CMDS.Admin.Workflows.Departments.Forms
{
    public partial class Assign : AdminBasePage, ICmdsUserControl
    {
        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            AssetOrganizationIdentifier.AutoPostBack = true;
            AssetOrganizationIdentifier.ValueChanged += AssetOrganizationIdentifier_ValueChanged;

            DepartmentIdentifier.AutoPostBack = true;
            DepartmentIdentifier.ValueChanged += (s, a) => OnFilterChanged();

            MessageIdentifier.AutoPostBack = true;
            MessageIdentifier.ValueChanged += (s, a) => OnFilterChanged();

            SearchButton.Click += (s, a) => OnSearch();

            TriggerButton.Click += (s, a) =>
                HttpResponseHelper.Redirect($"/ui/cmds/admin/messages/send?message={MessageIdentifier.ValueAsGuid.Value}");
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            if (IsPostBack)
                return;

            PageHelper.AutoBindHeader(this);

            AssetOrganizationIdentifier.ValueAsGuid = Organization.Identifier;
            AssetOrganizationIdentifier.Enabled = false;

            DepartmentIdentifier.OrganizationIdentifier = Organization.Identifier;

            LoadEmploymentTypes();

            TriggerButton.Visible = Identity.IsInRole(CmdsRole.SystemAdministrators)
                || Identity.IsInRole(CmdsRole.Programmers);
        }

        private void AssetOrganizationIdentifier_ValueChanged(object sender, EventArgs e)
        {
            DepartmentIdentifier.OrganizationIdentifier = Organization.Identifier;

            LoadEmploymentTypes();
            OnFilterChanged();
        }

        private void OnFilterChanged()
        {
            Page.Validate("Assignment");

            OnSearch();
        }

        private void OnSearch()
        {
            ResultUpdatePanel.Update();

            ResultCard.Visible = false;

            if (!Page.IsValid)
                return;

            ResultCard.Visible = true;

            SubscriberGrid.LoadData(
                Organization.Identifier,
                DepartmentIdentifier.Value.Value,
                GetSelectedEmploymentTypes(), MessageIdentifier.ValueAsGuid.Value);
        }

        private void LoadEmploymentTypes()
        {
            var employmentTypes = MembershipSearch.SelectEmploymentTypes(Organization.Identifier, true);

            EmploymentTypeField.Visible = employmentTypes.Length > 0;

            if (employmentTypes.Length == 0)
                return;

            EmploymentType.Items.Clear();

            foreach (var employmentType in employmentTypes)
                EmploymentType.Items.Add(employmentType);

            var organization = OrganizationSearch.Select(Organization.Identifier);

            if (ServiceLocator.Partition.IsE03())
            {
                var department = EmploymentType.Items.FindByValue("Department");

                if (department != null)
                    department.Selected = true;
            }
            else
            {
                foreach (ListItem item in EmploymentType.Items)
                    item.Selected = true;
            }
        }

        private string[] GetSelectedEmploymentTypes()
        {
            if (!EmploymentTypeField.Visible)
                return null;

            var result = new List<string>();

            foreach (ListItem item in EmploymentType.Items)
                if (item.Selected)
                    result.Add(item.Value);

            return result.Count > 0 ? result.ToArray() : null;
        }
    }
}