using System;

using InSite.Common.Web.UI;
using InSite.Persistence.Plugin.CMDS;
using InSite.UI.Layout.Admin;

namespace InSite.Custom.CMDS.Admin.Reports.Forms
{
    public partial class ExecutiveSummaryOnAchievementStatus : SearchPage<ExecutiveSummaryOnAchievementStatusFilter>
    {
        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            if (!Page.IsPostBack)
                PageHelper.AutoBindHeader(this);
        }
    }
}