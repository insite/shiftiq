using System;
using System.Threading;
using System.Web;
using System.Web.Http;
using System.Web.Http.Controllers;

using Shift.Common;
using Shift.Contract;

namespace InSite.Api
{
    public class CookieAuthorizeAttribute : AuthorizeAttribute
    {
        public override void OnAuthorization(HttpActionContext context)
        {
            var settings = Global.GetSecuritySettings();

            // 1. Get the HTTP Request
            var request = HttpContext.Current?.Request;
            if (request == null)
            {
                HandleUnauthorizedRequest(context);
                return;
            }

            // 2. Check if Cookie Exists
            var authCookie = request.Cookies[settings.Cookie.Name];
            if (authCookie == null || string.IsNullOrEmpty(authCookie.Value))
            {
                HandleUnauthorizedRequest(context);
                return;
            }

            // 3. Extract Token from Cookie
            string token = authCookie.Value;

            var secret = settings.Cookie.Secret;

            var encoder = new CookieTokenEncoder();

            var cookie = encoder.Deserialize(token, settings.Cookie.Encrypt, secret);

            var converter = new ClaimConverter(Global.GetSecuritySettings());

            var p = ToPrincipal(cookie);

            Thread.CurrentPrincipal = p;

            if (HttpContext.Current != null)
            {
                HttpContext.Current.User = p;
            }
        }

        private IShiftPrincipal ToPrincipal(CookieToken cookie)
        {
            var principal = new Principal();

            principal.User = new Actor { Identifier = cookie.UserIdentifier ?? Guid.Empty, Email = cookie.UserEmail, Name = cookie.UserEmail };

            principal.Organization = new Model
            {
                Identifier = cookie.OrganizationIdentifier ?? Guid.Empty,
                Name = cookie.OrganizationCode,
                Slug = cookie.OrganizationCode
            };

            return principal;
        }
    }
}