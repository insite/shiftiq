using System;
using System.Web;

using InSite.Common.Web;
using InSite.UI.Layout.Lobby.Controls;

using Shift.Common;

namespace InSite.UI.Lobby.SignInPages
{
    public partial class SignInSocial : SignInBasePage
    {

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            if (!IsPostBack)
            {
                if (!ProcessMSLoginResponse())
                    SignOut.Redirect(this, "Microsoft authentication failed");

                if (!ProcessGALoginResponse())
                    SignOut.Redirect(this, "Google authentication failed");
            }
        }

        private bool ProcessMSLoginResponse()
        {
            var code = HttpContext.Current.Request.QueryString["code"];
            var state = HttpContext.Current.Request.QueryString["state"];
            if (string.IsNullOrWhiteSpace(code)) return false;
            if (string.IsNullOrWhiteSpace(state)) return false;
            if (!state.Contains("MS:")) return false;
            var stateid = state.Replace("MS:", "");
            var organizationId = Organization.OrganizationIdentifier;
            if (!Guid.TryParse(stateid, out var cacheId)) return false;

            var authResult = Global.AzureAD.Authenticate(code, cacheId, organizationId, Page.Request.Url.Host);
            if (authResult == null) return false;
            if (authResult.TenantMismatch)
            {

                HttpResponseHelper.Redirect($"{authResult.CacheEntry.URL}/ui/lobby/signin-social?code={code}&state={state}", true);
                return false;
            }
            if (!authResult.Authorized) return false;
            if (authResult.AuthenticationMethod != OAuthAuthenticationMethods.Microsoft) return false;
            LoginUser(authResult.UserPrincipalName, "", true, Shift.Constant.AuthenticationSource.Microsoft);
            return true;
        }
        private bool ProcessGALoginResponse()
        {
            var code = HttpContext.Current.Request.QueryString["code"];
            var state = HttpContext.Current.Request.QueryString["state"];
            if (string.IsNullOrWhiteSpace(code)) return false;
            if (string.IsNullOrWhiteSpace(state)) return false;
            if (!state.Contains("GA:")) return false;
            var stateid = state.Replace("GA:", "");
            var organizationId = Organization.OrganizationIdentifier;
            if (!Guid.TryParse(stateid, out var cacheId)) return false;
            var authResult = Global.GoogleLogin.Authenticate(code, cacheId, organizationId, Page.Request.Url.Host);
            if (authResult == null) return false;
            if (authResult.TenantMismatch)
            {
                HttpResponseHelper.Redirect($"{authResult.CacheEntry.URL}/ui/lobby/signin-social?code={code}&state={state}", true);
                return false;
            }
            if (!authResult.Authorized) return false;
            if (authResult.AuthenticationMethod != OAuthAuthenticationMethods.Google) return false;
            LoginUser(authResult.UserPrincipalName, "", true, Shift.Constant.AuthenticationSource.Google);
            return true;
        }
    }
}