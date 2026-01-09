using System;

using InSite.Common.Web.UI;
using InSite.Persistence;
using InSite.UI.Layout.Admin;

using Shift.Contract;

namespace InSite.Custom.CMDS.Admin.Standards.Validations.Forms
{
    public partial class Search : SearchPage<StandardValidationFilter>
    {
        public override string EntityName => "Validation";

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            PageHelper.AutoBindHeader(this, new BreadcrumbItem("Add New Validation", "/ui/cmds/admin/validations/create"));
        }
    }
}