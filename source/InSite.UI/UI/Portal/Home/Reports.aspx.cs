using System;

using InSite.UI.Layout.Admin;
using InSite.UI.Layout.Portal;

using Shift.Constant;

namespace InSite.UI.Portal.Home
{
    public partial class Reports : PortalBasePage
    {
        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            PageHelper.AutoBindHeader(this);

            PortalMaster.ShowAvatar();
            PortalMaster.EnableSidebarToggle(true);

            TBD.Visible = Organization.Key != OrganizationIdentifiers.Demo;
            TempReportForDemo.Visible = Organization.Key == OrganizationIdentifiers.Demo;
        }
    }
}