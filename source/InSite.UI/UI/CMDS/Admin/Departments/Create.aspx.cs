using System;
using System.Collections.Generic;

using Shift.Common.Timeline.Commands;

using InSite.Application.Groups.Write;
using InSite.Common.Web;
using InSite.Persistence;
using InSite.UI.Layout.Admin;
using InSite.Web.Data;

using Shift.Common;

using BreadcrumbItem = Shift.Contract.BreadcrumbItem;

namespace InSite.Cmds.Admin.Departments.Forms
{
    public partial class Create : AdminBasePage
    {
        private const string EditUrl = "/ui/cmds/admin/departments/edit";
        private const string SearchUrl = "/cmds/admin/departments/search";

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            if (!IsPostBack)
            {
                var organization = OrganizationIdentifier.HasValue ? OrganizationSearch.Select(OrganizationIdentifier.Value) : null;
                if (organization == null)
                    HttpResponseHelper.Redirect(SearchUrl, true);

                var orgUrl = $"/ui/cmds/admin/organizations/edit?id={OrganizationIdentifier}";

                PageHelper.BindHeader(this, new BreadcrumbItem[]
                {
                    new BreadcrumbItem("Organizations", "/ui/cmds/admin/organizations/search"),
                    new BreadcrumbItem("Edit", orgUrl),
                    new BreadcrumbItem("Department", null)
                }, null, organization.CompanyName);

                Details.SetDefaultInputValues(OrganizationIdentifier.Value);

                CancelButton.NavigateUrl = orgUrl;
            }
        }

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            SaveButton.Click += SaveButton_Click;
        }

        private void SaveButton_Click(object sender, EventArgs e)
        {
            if (IsValid)
                Save();
        }

        private void Save()
        {
            var department = DepartmentFactory.Create(UniqueIdentifier.Create());

            GetInputValues(department);

            var id = UniqueIdentifier.Create();

            var commands = new List<Command>
            {
                new CreateGroup(id, department.OrganizationIdentifier, "Department", department.DepartmentName),
                new DescribeGroup(id, null, department.DepartmentCode, department.DepartmentDescription, department.DepartmentLabel),
                new ChangeGroupParent(id, department.DivisionIdentifier)
            };

            ServiceLocator.SendCommands(commands);

            HttpResponseHelper.Redirect($"{EditUrl}?id={id}");
        }

        private void GetInputValues(Department department)
        {
            Details.GetInputValues(department);
            department.OrganizationIdentifier = OrganizationIdentifier.Value;
        }

        private Guid? OrganizationIdentifier => Guid.TryParse(Request["organizationID"], out var key) ? key : (Guid?)null;
    }
}
