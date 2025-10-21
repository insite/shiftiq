using System;

using InSite.UI.Layout.Admin;
using InSite.UI.Layout.Portal;

namespace InSite.UI.Portal.Home
{
    public partial class Chats : PortalBasePage
    {
        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            PageHelper.AutoBindHeader(this);

            PortalMaster.ShowAvatar();
            PortalMaster.EnableSidebarToggle(true);
        }
    }
}