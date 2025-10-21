using System;

using InSite.Common.Web;
using InSite.UI.Layout.Admin;
using InSite.UI.Layout.Portal;
using InSite.UI.Portal.Billing.Models;

namespace InSite.UI.Portal.Home.Management
{
    public partial class Catalog : PortalBasePage
    {
        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            CatalogDetail.ClickCart += (s, a) => HttpResponseHelper.Redirect(InSite.UI.Portal.Billing.Catalog.CartPage);
            CatalogDetail.ClickCheckout += (s, a) => HttpResponseHelper.Redirect(InSite.UI.Portal.Billing.Catalog.CheckoutPage);
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            if (IsPostBack)
                return;

            PortalMaster.ShowAvatar(dashboardUrl: "/ui/portal/management/dashboard/home");
            PortalMaster.RenderHelpContent(null);

            PageHelper.AutoBindHeader(this);

            CatalogDetail.LoadData();
        }
    }
}