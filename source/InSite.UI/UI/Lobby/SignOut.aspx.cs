using System;
using System.Collections.Generic;
using System.Web;
using System.Web.UI;

using InSite.Common.Web;
using InSite.Common.Web.UI;
using InSite.UI.Layout.Lobby;

using Shift.Common;

namespace InSite.UI.Lobby
{
    public partial class SignOut : LobbyBasePage
    {
        private string ReturnUrl => Request.QueryString["returnurl"];

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            if (Request.IsAuthenticated)
                CurrentIdentityFactory.SignedOut();

            var cookiesToExpire = new List<string>
            {
                ServiceLocator.AppSettings.Security.Cookie.Name
            };

            var sessionCookieName = Global.GetSessionStateCookieName();

            if (!string.IsNullOrEmpty(sessionCookieName))
                cookiesToExpire.Add(sessionCookieName);

            ExpireCookies(cookiesToExpire);

            Session.Clear();

            Session.Abandon();

            Context.User = null;

            // Remember cookies cannot be expired by the server in the same response with a redirect.

            var nextUrl = GetUrl() + "-completed?cookies-cleared=1";

            if (UserPasswordCheck.IsPasswordChangeRequestLimitExceeded)
                nextUrl += "&password-expired=1";

            if (ReturnUrl != null)
                nextUrl += "&returnurl=" + HttpUtility.UrlEncode(ReturnUrl);

            const int ThreeSeconds = 3000; // milliseconds

            ClientRedirect(nextUrl, ThreeSeconds);
        }

        /// <remarks>
        /// This guarantees cookies are expired both server-side and client-side. The redirect occurs after the browser
        /// processes the expired cookies. The pattern previously implemented must be avoided because it causes the 
        /// redirect to occur before the browser deletes the expired cookies. The previous pattern was basically this:
        ///   ExpireCookies();
        ///   Response.Redirect(LoginUrl); // The browser sends stale cookies here, which are supposed to be expired
        /// </remarks>
        private void ClientRedirect(string nextUrl, int sleep)
        {
            var script = $@"
                setTimeout(function() {{
                    window.location.href = '{nextUrl}';
                }}, {sleep}); // Allow enough time for the cookie jar to empty";

            ClientScript.RegisterStartupScript(GetType(), nameof(SignOut), script, true);
        }

        public static void Redirect(object redirector, string reason)
        {
            const string redirectFlagKey = "Redirect_AlreadyPerformed";

            var httpContext = HttpContext.Current;

            if (httpContext == null)
                throw new InvalidOperationException("No active HTTP context");

            // Prevent multiple redirects in same request

            if (httpContext.Items.Contains(redirectFlagKey))
            {
                AppSentry.SentryInfo("Redirect aborted: already performed during this request.");
                return;
            }

            httpContext.Items[redirectFlagKey] = true;

            var targetUrlString = GetWebUrl().ToString(); // Assume the target is always root-relative

            if (string.IsNullOrWhiteSpace(targetUrlString))
                throw new InvalidOperationException("Target URL is null or empty.");

            if (!string.IsNullOrEmpty(reason))
            {
                var separator = targetUrlString.Contains("?") ? "&" : "?";
                targetUrlString += $"{separator}reason={Uri.EscapeDataString(reason)}";
            }

            // Resolve target URI relative to current request

            var currentRequest = httpContext.Request;

            var resolvedTargetUri = new Uri(currentRequest.Url, targetUrlString);

            // Get original requested path (RawUrl without query)

            string rawPath = currentRequest.RawUrl?.Split('?')[0];

            if (string.Equals(resolvedTargetUri.AbsolutePath, rawPath, StringComparison.OrdinalIgnoreCase))
            {
                AppSentry.SentryInfo("Redirect aborted: target path matches current path");
                return;
            }

            var context = new Dictionary<string, string>
            {
                { "Redirector", redirector.GetType().Name },
                { "Redirect Reason", reason },
                { "Sign Out URL", resolvedTargetUri.ToString() },
                { "Raw URL", currentRequest.RawUrl }
            };

            if (redirector is Page p)
            {
                context.Add("Referrer URL", p.Request.UrlReferrer?.ToString() ?? "N/A");
            }

            var data = ServiceLocator.Serializer.Serialize(context);

            HttpResponseHelper.Redirect(targetUrlString);
        }

        public static string GetUrl() => "/ui/lobby/signout";

        public static WebUrl GetWebUrl() => new WebUrl(GetUrl());

        /// <summary>
        /// Instructs the client to remove cookies by setting their expiration dates in the past.
        /// </summary>
        private void ExpireCookies(List<string> cookiesToExpire)
        {
            foreach (string cookie in cookiesToExpire)
            {
                var original = Request.Cookies[cookie];

                if (original == null)
                    continue;

                HttpCookie expiredCookie = new HttpCookie(cookie)
                {
                    Domain = ServiceLocator.AppSettings.Security.Domain,
                    Expires = DateTime.UtcNow.AddMonths(-1),
                    HttpOnly = original.HttpOnly,
                    Path = original.Path,
                    SameSite = original.SameSite,
                    Value = original.Value
                };

                Response.Cookies.Add(expiredCookie);
            }
        }
    }
}