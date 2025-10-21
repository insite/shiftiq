using System;

using InSite.Common.Web.UI;
using InSite.Persistence;
using InSite.UI.Layout.Admin;

using Shift.Contract;

namespace InSite.Custom.CMDS.Admin.Standards.ValidationChanges.Forms
{
    public partial class Search : SearchPage<StandardValidationChangeFilter>
    {
        public override string EntityName => "Validation Change";

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            if (!IsPostBack)
                PageHelper.AutoBindHeader(this, new BreadcrumbItem("Add New Change", "/ui/cmds/admin/validations/changes/create"));
        }
    }
}