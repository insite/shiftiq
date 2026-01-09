using System;

using InSite.UI.Layout.Admin;

namespace InSite.UI.Admin.Assets.Documents
{
    public partial class Convert : AdminBasePage
    {
        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            PageHelper.AutoBindHeader(this);
        }
    }
}