using System.Collections.Concurrent;
using System.Security.Claims;
using System.Threading;
using System.Web;
using System.Web.Http;
using System.Web.Http.Controllers;

using Shift.Common;

namespace InSite.Api
{
    public class JwtAuthorizeAttribute : AuthorizeAttribute
    {
        private static ConcurrentDictionary<string, bool> _verifiedTokens = new ConcurrentDictionary<string, bool>();

        public override void OnAuthorization(HttpActionContext context)
        {
            var authHeader = context.Request.Headers.Authorization?.ToString();

            if (string.IsNullOrEmpty(authHeader) || !authHeader.StartsWith("Bearer"))
            {
                HandleUnauthorizedRequest(context);
                return;
            }

            var token = authHeader.Substring("Bearer".Length).Trim();

            var (jwt, _) = GetJwt(token);

            if (jwt == null)
            {
                HandleUnauthorizedRequest(context);
                return;
            }

            var converter = new ClaimConverter(Global.GetSecuritySettings());

            var principal = new ClaimsPrincipal(converter.ToClaimsIdentity(jwt, "JWT"));

            var p = converter.ToPrincipal(jwt);

            if (!ValidatePrincipal(p))
            {
                HandleUnauthorizedRequest(context);
                return;
            }

            Thread.CurrentPrincipal = p;

            if (HttpContext.Current != null)
                HttpContext.Current.User = p;
        }

        private bool ValidatePrincipal(IShiftPrincipal principal)
        {
            // TODO: Check the cache for a validation result for this principal

            var user = ServiceLocator.UserSearch.GetUser(principal.User.Identifier);

            if (user == null)
                return false;

            var org = ServiceLocator.OrganizationSearch.Get(principal.Organization.Identifier);

            if (org == null)
                return false;

            // TODO: Hydrate the principal object (i.e. fill in the blanks)

            // TODO: Cache the validation result for this principal

            return true;
        }

        public static (IJwt jwt, bool wasInCache) GetJwt(string token)
        {
            var encoder = new JwtEncoder();

            var jwt = encoder.Decode(token);

            var settings = Global.GetSecuritySettings();

            var audience = settings.Token.Audience;

            var secret = settings.Secret;

            if (_verifiedTokens.TryGetValue(token, out _))
            {
                if (!jwt.IsExpired())
                    return (jwt, true);

                _verifiedTokens.TryRemove(token, out _);

                return (null, true);
            }

            // Bearer tokens may be issued by the v1 API or by the v2 API, therefore we need to exclude Issuer from the
            // token validation check. In future, when all tokens are issued by the v2 API, then we can include Issuer
            // when we validate JWTs.

            var isValid = !jwt.IsExpired()
                && jwt.Audience == audience
                && encoder.VerifySignature(token, secret)
                ;

            if (!isValid)
                return (null, false);

            _verifiedTokens.TryAdd(token, true);

            return (jwt, false);
        }
    }
}
