using System;

using InSite.UI.Layout.Admin;
using InSite.UI.Layout.Portal;

namespace InSite.UI.Portal.Home.Management
{
    public partial class Reports : PortalBasePage
    {
        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            if (IsPostBack)
                return;

            PortalMaster.ShowAvatar(dashboardUrl: "/ui/portal/management/dashboard/home");
            PortalMaster.RenderHelpContent(null);

            PageHelper.AutoBindHeader(this);
        }
    }
}