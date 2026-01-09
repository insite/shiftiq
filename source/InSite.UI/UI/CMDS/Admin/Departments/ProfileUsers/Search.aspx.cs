using System;

using InSite.Common.Web.UI;
using InSite.Persistence;
using InSite.UI.Layout.Admin;

using Shift.Contract;

namespace InSite.Custom.CMDS.Admin.Standards.DepartmentProfileUsers.Forms
{
    public partial class Search : SearchPage<DepartmentProfileUserFilter>
    {
        public override string EntityName => "Department Profile User";

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            if (Page.IsPostBack)
                return;

            PageHelper.AutoBindHeader(
                this,
                new BreadcrumbItem("Add New Department Profile User", "/ui/cmds/admin/departments/profile-users/create"));
        }
    }
}