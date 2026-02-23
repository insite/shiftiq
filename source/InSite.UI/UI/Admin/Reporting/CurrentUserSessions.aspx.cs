using System;

using InSite.UI.Layout.Admin;

namespace InSite.UI.Admin.Reporting
{
    public partial class CurrentUserSessions : AdminBasePage
    {
        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            PageHelper.AutoBindHeader(this);

            DashboardUsers.LoadData();
        }
    }
}