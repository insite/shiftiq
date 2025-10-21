using System;
using System.Web.UI;

using InSite.UI.Layout.Admin;

using Shift.Sdk.UI;

namespace InSite.Cmds.Actions.Reporting.Compliance
{
    public partial class Snapshot : AdminBasePage, ICmdsUserControl
    {
        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            ScriptManager.GetCurrent(Page).AsyncPostBackTimeout = 30 * 60 * 1000;
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            if (!IsPostBack)
            {
                PageHelper.AutoBindHeader(Page);

                HistorySnapshotGrid.LoadData();
            }
        }

        protected override void OnPreRender(EventArgs e)
        {
            base.OnPreRender(e);

            ScriptManager.RegisterStartupScript(Page, typeof(Snapshot), "init", "snapshotPage.init();", true);
        }
    }
}
