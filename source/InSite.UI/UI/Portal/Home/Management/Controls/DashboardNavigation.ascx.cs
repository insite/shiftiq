using System;

using InSite.Common.Web.UI;

namespace InSite.UI.Portal.Home.Management.Controls
{
    public partial class DashboardNavigation : BaseUserControl
    {
        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            if (IsPostBack)
                return;

            if (Organization.Toolkits.Accounts.AHRIFeatures)
            {
                MyInvoices.Visible = false;
                MyCatalog.Visible = false;
            }
        }
    }
}
