using System;

using InSite.UI.Layout.Admin;

namespace InSite.Admin.Events.Reports.Forms
{
    public partial class Dashboard : AdminBasePage
    {
        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            PageHelper.AutoBindHeader(this);
        }
    }
}
