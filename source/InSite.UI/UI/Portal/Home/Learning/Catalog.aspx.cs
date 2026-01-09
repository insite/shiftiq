using System;

using InSite.UI.Layout.Admin;
using InSite.UI.Layout.Portal;

namespace InSite.UI.Portal.Home.Learning
{
    public partial class Catalog : PortalBasePage
    {
        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            if (IsPostBack)
                return;

            PortalMaster.ShowAvatar(dashboardUrl: "/ui/portal/learning/dashboard/home");
            PortalMaster.RenderHelpContent(null);
            PortalMaster.HideBreadcrumbsAndTitle();

            PageHelper.AutoBindHeader(this);
        }
    }
}