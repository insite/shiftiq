using System;

using InSite.UI.Layout.Admin;

namespace InSite.UI.Admin
{
    public partial class Home : AdminBasePage
    {
        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            PageHelper.AutoBindHeader(this);
        }
    }
}
