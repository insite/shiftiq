using System;

using InSite.Common.Web.UI;
using InSite.Persistence.Plugin.CMDS;
using InSite.UI.Layout.Admin;

using Shift.Contract;

namespace InSite.Custom.CMDS.Admin.Standards.Profiles
{
    public partial class Search : SearchPage<ProfileFilter>
    {
        public override string EntityName => "Profile";

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            if (!Page.IsPostBack)
                PageHelper.AutoBindHeader(
                    this,
                    new BreadcrumbItem("Add New Profile", "/ui/cmds/admin/standards/profiles/create", null, null));
        }
    }
}