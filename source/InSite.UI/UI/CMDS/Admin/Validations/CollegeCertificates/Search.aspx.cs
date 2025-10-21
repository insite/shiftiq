using System;

using InSite.Common.Web.UI;
using InSite.Persistence.Plugin.CMDS;
using InSite.UI.Layout.Admin;

using Shift.Contract;

namespace InSite.Cmds.Actions.Profile.Employee.Certificate
{
    public partial class Search : SearchPage<EmployeeCertificateFilter>
    {
        public override string EntityName => "College Certificate";

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            if (Page.IsPostBack)
                return;

            PageHelper.AutoBindHeader(
                this, 
                new BreadcrumbItem("Add New College Certificate", "/ui/cmds/admin/validations/college-certificates/create"));
        }
    }
}