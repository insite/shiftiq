using System;

using InSite.Common.Web;
using InSite.UI.Layout.Admin;
using InSite.UI.Layout.Portal;

namespace InSite.UI.Portal.Billing
{
    public partial class Catalog : PortalBasePage
    {
        public const string CheckoutPage = "/ui/portal/billing/checkout";
        public const string CartPage = "/ui/portal/billing/cart";

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            CatalogDetail.ClickCart += (s, a) => HttpResponseHelper.Redirect(CartPage);
            CatalogDetail.ClickCheckout += (s, a) => HttpResponseHelper.Redirect(CheckoutPage);
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            if (IsPostBack)
                return;

            PageHelper.AutoBindHeader(this, null, "Catalog");
            
            CatalogDetail.LoadData();

            PortalMaster.ShowAvatar();
            PortalMaster.HideBreadcrumbsOnly();

            if (Identity.IsAuthenticated)
                OverrideHomeLink("/ui/portal/management/dashboard/home");
            else
                OverrideHomeLink("/ui/portal/billing/catalog");
        }
    }
}