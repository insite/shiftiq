using System;

using InSite.Common.Web;
using InSite.UI.Layout.Lobby.Controls;

using Shift.Common;

namespace InSite.UI.Lobby.SignInPages
{
    public partial class SignInSocial : SignInBasePage
    {
        private const string MicrosoftStatePrefix = "MS:";
        private const string GoogleStatePrefix = "GA:";

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            if (IsPostBack)
                return;

            var code = Request.QueryString["code"];
            var state = Request.QueryString["state"];

            if (code.IsNotEmpty() && state.IsNotEmpty())
            {
                if (state.StartsWith(MicrosoftStatePrefix))
                {
                    if (!ProcessMSLoginResponse(code, state))
                        SignOut.Redirect(this, "Microsoft authentication failed");
                }
                else if (state.StartsWith(GoogleStatePrefix))
                {
                    if (!ProcessGALoginResponse())
                        SignOut.Redirect(this, "Google authentication failed");
                }
            }

            SignOut.Redirect(this, "Authentication failed");
        }

        private bool ValidateStateEntity(string state, int prefixLen, OAuthMethod method)
        {
            if (!Guid.TryParse(state.Substring(prefixLen), out var stateId))
                return false;

            var stateEntity = OAuthCacheService.Get(stateId);
            if (stateEntity == null || stateEntity.Method != method)
                return false;

            if (stateEntity.OrganizationId != Organization.OrganizationIdentifier)
            {
                var url = new WebUrl(Request.RawUrl);
                url.Path = $"{stateEntity.Url}/ui/lobby/signin-social";

                HttpResponseHelper.Redirect(url);
            }

            OAuthCacheService.Remove(stateId);

            return true;
        }

        private bool ProcessMSLoginResponse(string code, string state)
        {
            if (!ValidateStateEntity(state, MicrosoftStatePrefix.Length, OAuthMethod.Microsoft))
                return false;

            var authResult = Global.MicrosoftEntra.GetOAuthResult(code);
            if (authResult?.Authorized != true)
                return false;

            LoginUser(authResult.UserName, string.Empty, true, Shift.Constant.AuthenticationSource.Microsoft);

            return true;
        }

        private bool ProcessGALoginResponse()
        {
            var code = System.Web.HttpContext.Current.Request.QueryString["code"];
            var state = System.Web.HttpContext.Current.Request.QueryString["state"];
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
                HttpResponseHelper.Redirect($"{authResult.CacheEntry.Url}/ui/lobby/signin-social?code={code}&state={state}", true);
                return false;
            }
            if (!authResult.Authorized) return false;
            if (authResult.AuthenticationMethod != OAuthMethod.Google) return false;
            LoginUser(authResult.UserPrincipalName, "", true, Shift.Constant.AuthenticationSource.Google);
            return true;
        }
    }
}
