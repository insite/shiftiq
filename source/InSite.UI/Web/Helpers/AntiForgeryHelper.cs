using System.Web;
using System.Web.Helpers;

namespace InSite.Web.Helpers
{
    public static class AntiForgeryHelper
    {
        public static HtmlString GetHtml()
        {
            return ServiceLocator.AppSettings.Application.AntiForgeryTokenValidationEnabled
                ? AntiForgery.GetHtml()
                : new HtmlString("");
        }

        public static void EnsureCookie()
        {
            if (!ServiceLocator.AppSettings.Application.AntiForgeryTokenValidationEnabled)
                return;

            var request = HttpContext.Current.Request;

            if (request.Cookies.Count == 0)
                return;

            var antiForgertyCookie = request.Cookies["__RequestVerificationToken"];
            if (antiForgertyCookie != null)
                antiForgertyCookie.SameSite = SameSiteMode.Strict;
        }

        public static void Validate()
        {
            if (ServiceLocator.AppSettings.Application.AntiForgeryTokenValidationEnabled)
                AntiForgery.Validate();
        }
    }
}