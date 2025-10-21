using System;

using InSite.Common.Web.UI;
using InSite.Persistence.Plugin.CMDS;
using InSite.UI.Layout.Admin;

using Shift.Contract;

namespace InSite.Cmds.Admin.Achievements.Forms
{
    public partial class Search : SearchPage<VCmdsAchievementFilter>
    {
        public override string EntityName => "Achievement";

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            PageHelper.AutoBindHeader(this, new BreadcrumbItem("Add New Achievement", "/ui/cmds/admin/achievements/create"));
        }
    }
}