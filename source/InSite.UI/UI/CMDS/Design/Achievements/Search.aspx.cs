using System;

using InSite.Common.Web.UI;
using InSite.Persistence.Plugin.CMDS;
using InSite.UI.Layout.Admin;

namespace InSite.Cmds.Admin.Achievements.Forms
{
    public partial class Search2 : SearchPage<VCmdsAchievementFilter>
    {
        public override string EntityName => "Field Achievement";

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            if (!IsPostBack)
                PageHelper.AutoBindHeader(this);
        }
    }
}