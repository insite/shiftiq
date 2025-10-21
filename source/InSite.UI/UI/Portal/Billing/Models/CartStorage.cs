using System.Web;

namespace InSite.UI.Portal.Billing.Models
{
    public static class CartStorage
    {
        private const string Key = "BILLING_CART_STATE";
        public static CartState Get()
        {
            var ctx = HttpContext.Current;
            var cart = ctx?.Session?[Key] as CartState;
            if (cart == null)
            {
                cart = new CartState();
                if (ctx != null) ctx.Session[Key] = cart;
            }
            return cart;
        }
    }
}