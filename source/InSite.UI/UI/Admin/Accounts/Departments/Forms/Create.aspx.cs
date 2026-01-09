using System;

using Shift.Common.Timeline.Commands;

using InSite.Application.Groups.Write;
using InSite.Common.Web;
using InSite.UI.Layout.Admin;
using InSite.Web.Data;

using Shift.Common;

namespace InSite.Admin.Identities.Departments.Forms
{
    public partial class Create : AdminBasePage
    {
        private const string EditUrl = "/ui/admin/accounts/departments/edit";
        private const string SearchUrl = "/ui/admin/accounts/departments/search";

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            SaveButton.Click += SaveButton_Click;
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            if (IsPostBack)
                return;

            if (!CanCreate)
                HttpResponseHelper.Redirect(SearchUrl);

            PageHelper.AutoBindHeader(Page);

            DepartmentDetails.SetDefaultInputValues();

            CancelButton.NavigateUrl = SearchUrl;
        }

        private void SaveButton_Click(object sender, EventArgs e)
        {
            if (!IsValid)
                return;

            var department = DepartmentFactory.Create(UniqueIdentifier.Create());

            DepartmentDetails.GetInputValues(department);

            var id = UniqueIdentifier.Create();

            var commands = new ICommand[]
            {
                new CreateGroup(id, department.OrganizationIdentifier, "Department", department.DepartmentName),
                new DescribeGroup(id, null, department.DepartmentCode, department.DepartmentDescription, department.DepartmentLabel),
                new ChangeGroupParent(id, department.DivisionIdentifier),
            };

            ServiceLocator.SendCommands(commands);

            HttpResponseHelper.Redirect($"{EditUrl}?id={id}&status=saved");
        }
    }
}