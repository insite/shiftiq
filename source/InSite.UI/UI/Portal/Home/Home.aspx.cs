using System;
using System.Web.UI;

using InSite.UI.Layout.Admin;
using InSite.UI.Layout.Common.Controls.Navigation;
using InSite.UI.Layout.Portal;

namespace InSite.UI.Portal.Home
{
    public partial class Home : PortalBasePage
    {
        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            if (IsPostBack)
                return;

            PortalMaster.ShowAvatar();

            PageHelper.AutoBindHeader(this);

            if (Page.Master is PortalMaster m)
            {
                m.HideBreadcrumbsAndTitle();

                var showSidebar = SidebarSwitch.SidebarEnabled() ?? Organization.Toolkits.Portal.ShowMyDashboard;
                if (!showSidebar)
                    m.SidebarVisible(false);
                else
                    m.EnableSidebarToggle(true);
            }
        }
    }
}