using System;
using System.Linq;
using System.Web;
using System.Web.UI;

using InSite.Common.Web.UI;

using Shift.Common;
using Shift.Common.Events;

namespace InSite.UI.Layout.Common.Controls.Navigation
{
    public partial class SidebarSwitch : BaseUserControl
    {
        private const string CookieName = "Shift.UI.Sidebar";
        private const int CookieLifetimeInDays = 90;

        protected static class SidebarValue
        {
            public const string Enabled = "Enabled";
            public const string Disabled = "Disabled";
            public const string Default = "Default";
        }

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            UpdatePanel.Request += UpdatePanel_Request;
        }

        protected override void OnPreRender(EventArgs e)
        {
            ScriptManager.RegisterStartupScript(Page, typeof(SidebarSwitch), "set_value", $"sidebarSwitch.setState({GetJsValue()});", true);

            base.OnPreRender(e);
        }

        private void UpdatePanel_Request(object sender, StringValueArgs e)
        {
            if (e.Value == SidebarValue.Enabled)
                SaveModeToCookie(SidebarValue.Enabled);
            else if (e.Value == SidebarValue.Disabled)
                SaveModeToCookie(SidebarValue.Disabled);
            else if (e.Value == SidebarValue.Default)
                SaveModeToCookie(SidebarValue.Default);
        }

        private void SaveModeToCookie(string value)
        {
            try
            {
                var cookie = new HttpCookie(CookieName);

                if (value.IsNotEmpty())
                {
                    cookie.Value = HttpUtility.UrlEncode(value);
                    cookie.Expires = DateTime.Now.AddDays(CookieLifetimeInDays);
                    cookie.HttpOnly = true; // Security: prevent client-side script access
                    cookie.SameSite = SameSiteMode.Lax; // CSRF protection
                    cookie.Domain = ServiceLocator.AppSettings.Security.Domain;
                }
                else
                {
                    // Clear the cookie by setting expiry to past date
                    cookie.Expires = DateTime.Now.AddDays(-1);
                }

                Response.Cookies.Add(cookie);
            }
            catch (Exception)
            {
                // Ignore unexpected errors for now
            }
        }

        public static bool? SidebarEnabled()
        {
            try
            {
                var cookie = HttpContext.Current.Response.Cookies.AllKeys.Contains(CookieName)
                    ? HttpContext.Current.Response.Cookies[CookieName]
                    : HttpContext.Current.Request.Cookies[CookieName];

                if (cookie != null && cookie.Value.IsNotEmpty())
                {
                    var value = HttpUtility.UrlDecode(cookie.Value);
                    if (value == SidebarValue.Enabled)
                        return true;

                    if (value == SidebarValue.Disabled)
                        return false;

                }
            }
            catch (Exception)
            {
                // Ignore unexpected errors for now
            }

            return null;
        }

        protected string GetJsValue()
        {
            var value = SidebarEnabled();
            return value.HasValue ? value.Value.ToString().ToLower() : "null";
        }
    }
}