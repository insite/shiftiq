using System;

using InSite.UI.Layout.Admin;

namespace InSite.UI.Admin.Foundations
{
    public partial class Colors : AdminBasePage
    {
        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            if (!IsPostBack)
                PageHelper.AutoBindHeader(this);
        }
    }
}