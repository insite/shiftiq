using System;

using InSite.UI.Layout.Admin;
using InSite.UI.Layout.Portal;
using InSite.UI.Portal.Billing.Models;

namespace InSite.UI.Portal.Billing
{
    public partial class Cart : PortalBasePage
    {
        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            CartDetail.StateChanged += (s, a) => SetCheckoutEnabled(CartDetail.AllowCheckout);
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            if (IsPostBack)
                return;

            PageHeader.Visible = !CurrentSessionState.Identity.IsAuthenticated;

            ContinueShopping.HRef = CurrentSessionState.Identity.IsAuthenticated
                ? "/ui/portal/management/dashboard/catalog"
                : "/ui/portal/billing/catalog";

            var cart = CartStorage.Get();

            CartDetail.LoadData(cart);

            PortalMaster.ShowAvatar();
            PageHelper.AutoBindHeader(this, null, "Cart");
        }

        private void SetCheckoutEnabled(bool enabled)
        {
            var cls = "btn btn-primary rounded-pill px-4";
            if (!enabled)
                cls += " disabled";

            CheckoutLink.Attributes["class"] = cls;
            CheckoutLink.Attributes["aria-disabled"] = (!enabled).ToString().ToLower();
            CheckoutLink.HRef = enabled ? "/ui/portal/billing/checkout" : "#";

            if (!enabled)
                CheckoutLink.Attributes["tabindex"] = "-1";
            else
                CheckoutLink.Attributes.Remove("tabindex");
        }
    }
}
