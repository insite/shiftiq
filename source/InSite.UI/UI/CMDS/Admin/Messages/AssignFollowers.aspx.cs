using System;
using System.Collections.Generic;
using System.Web.UI.WebControls;

using InSite.Persistence;
using InSite.UI.Layout.Admin;

using Shift.Sdk.UI;

namespace InSite.Cmds.Admin.Workflows.Followers
{
    public partial class Assign : AdminBasePage, ICmdsUserControl
    {
        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            OrganizationIdentifier.AutoPostBack = true;
            OrganizationIdentifier.ValueChanged += OrganizationIdentifier_ValueChanged;

            DepartmentIdentifier.AutoPostBack = true;
            DepartmentIdentifier.ValueChanged += (s, a) =>
            {
                FollowerID.Filter.GroupDepartmentIdentifiers = DepartmentIdentifier.Value.HasValue
                    ? new[] { DepartmentIdentifier.Value.Value }
                    : null;

                FollowerID.Filter.GroupDepartmentFunctions = GetSelectedEmploymentTypes();

                OnFilterChanged();
            };

            EmploymentType.AutoPostBack = true;
            EmploymentType.SelectedIndexChanged += (s, a) =>
            {
                FollowerID.Filter.GroupDepartmentFunctions = GetSelectedEmploymentTypes();

                OnFilterChanged();
            };

            MessageIdentifier.AutoPostBack = true;
            MessageIdentifier.ValueChanged += (s, a) => OnFilterChanged();

            FollowerID.AutoPostBack = true;
            FollowerID.ValueChanged += (s, a) => OnFilterChanged();

            SearchButton.Click += (s, a) => OnSearch();
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            if (IsPostBack)
                return;

            PageHelper.AutoBindHeader(this);

            OrganizationIdentifier.ValueAsGuid = Organization.Identifier;
            DepartmentIdentifier.OrganizationIdentifier = Organization.Identifier;

            FollowerID.Filter.OrganizationIdentifier = Organization.Identifier;
            FollowerID.Filter.IsArchived = false;
            FollowerID.Filter.IsCmds = true;

            LoadEmploymentTypes();
        }

        private void OrganizationIdentifier_ValueChanged(object sender, EventArgs e)
        {
            var organizationId = OrganizationIdentifier.ValueAsGuid ?? Guid.Empty;

            DepartmentIdentifier.OrganizationIdentifier = organizationId;
            DepartmentIdentifier.Value = null;

            FollowerID.Filter.OrganizationIdentifier = organizationId;
            FollowerID.Value = null;

            LoadEmploymentTypes();

            ResultCard.Visible = false;

            ResultUpdatePanel.Update();
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

            SubscriberGrid.LoadData(OrganizationIdentifier.ValueAsGuid.Value, DepartmentIdentifier.Value, GetSelectedEmploymentTypes(), MessageIdentifier.ValueAsGuid.Value, FollowerID.Value.Value);
        }

        private void LoadEmploymentTypes()
        {
            var employmentTypes = MembershipSearch.SelectEmploymentTypes(Organization.Identifier);

            EmploymentTypeField.Visible = employmentTypes.Length > 0;

            if (employmentTypes.Length == 0)
                return;

            EmploymentType.Items.Clear();

            foreach (var employmentType in employmentTypes)
            {
                var item = new ListItem
                {
                    Value = employmentType,
                    Text = employmentType
                };

                if (item.Text == "Administration")
                    item.Text = "Data Access";

                EmploymentType.Items.Add(item);
            }

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