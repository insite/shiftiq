using System;
using System.Web.UI;

using InSite.Web.Helpers;

namespace InSite.UI.Layout.Portal
{
    public partial class PortalPrintMaster : MasterPage
    {
        protected override void OnLoad(EventArgs e)
        {
            if (IsPostBack)
                AntiForgeryHelper.Validate();

            base.OnLoad(e);
        }

        public void HideSideContent()
        {
            SideContent.Visible = false;
        }
    }
}