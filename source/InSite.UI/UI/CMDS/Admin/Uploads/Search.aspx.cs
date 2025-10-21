using System;

using InSite.Common.Web.UI;
using InSite.Persistence;
using InSite.UI.Layout.Admin;

using Shift.Contract;

namespace InSite.Cmds.Admin.Uploads.Forms
{
    public partial class Search : SearchPage<UploadFilter>
    {
        public override string EntityName => "Upload";

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            if (!Page.IsPostBack)
                PageHelper.AutoBindHeader(
                    this,
                    new BreadcrumbItem("Add New Upload", "/ui/cmds/admin/uploads/create", null, null));
        }
    }
}