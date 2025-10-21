using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using System.Web.Http.Filters;

using InSite.Persistence;
using InSite.Persistence.Integration;
using InSite.Web;

using Shift.Common;
using Shift.Sdk.UI;

using UserPacket = InSite.Domain.Foundations.User;

namespace InSite.Api.Settings
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
    public class ApiValidationFilter : Attribute, IAuthenticationFilter
    {
        public bool AllowMultiple
            => false;

        public bool LogLocalApiRequests
            => ServiceLocator.AppSettings.Shift.Api?.Telemetry?.Logging?.Level == "Verbose";

        public async Task AuthenticateAsync(HttpAuthenticationContext context, CancellationToken cancellation)
        {
            var authType = GetApiAuthenticationRequirement(context);
            if (authType == ApiAuthenticationType.None)
                return;

            if (authType == ApiAuthenticationType.Request)
                AuthenticateRequest(context);

            else if (authType == ApiAuthenticationType.Jwt)
                AuthenticateJwt(context);

            else if (authType == ApiAuthenticationType.Cookie)
                AuthenticateCookie(context);

            else
                await AuthenticateLegacy(context);
        }

        private static void AuthenticateRequest(HttpAuthenticationContext context)
        {
            if (!ValidateRequest(context))
                context.ErrorResult = new ServerErrorActionResult("Unauthenticated requests are not permitted.", context, HttpStatusCode.Forbidden);
        }

        private void AuthenticateJwt(HttpAuthenticationContext context)
        {
            var bearer = context.Request.Headers?.Authorization?.Parameter;
            if (bearer.IsEmpty())
            {
                context.ErrorResult = new ServerErrorActionResult("This method requires a bearer authorization token", context, HttpStatusCode.Forbidden);
                return;
            }

            var validation = ValidateJwt(bearer);
            if (!validation.IsValid)
                context.ErrorResult = new ServerErrorActionResult(validation.Error, context, HttpStatusCode.Forbidden);
        }

        private async Task AuthenticateLegacy(HttpAuthenticationContext context)
        {
            var validation = ApiHelper.Validate(context);
            if (!validation.Success)
                context.ErrorResult = new ServerErrorActionResult(validation.Error, context, HttpStatusCode.Forbidden);

            // If the request is not local, then it must be logged. If it is local, then log the request only if this
            // is explicitly enabled with the LogLocalApiRequests app setting.

            if (validation.IsLocal && !LogLocalApiRequests)
                return;

            var request = context.Request;
            var responseLogId = UniqueIdentifier.Create();

            request.Headers.Add("X-InSiteApiResponseLogId", responseLogId.ToString());

            await LogApiRequestAsync(request, validation, responseLogId);
        }

        private void AuthenticateCookie(HttpAuthenticationContext context)
        {
            if (!HttpContext.Current.Request.IsAuthenticated)
                context.ErrorResult = new ServerErrorActionResult("Unauthenticated requests are not permitted.", context, HttpStatusCode.Forbidden);
        }

        private static bool ValidateRequest(HttpAuthenticationContext context)
        {
            var token = CookieTokenModule.Current;
            if (token == null || !token.IsActive())
                return false;

            var isAuthenticated = context.ActionContext?.ControllerContext?.RequestContext?.Principal?.Identity?.IsAuthenticated ?? false;
            if (!isAuthenticated)
                return false;

            var organization = ApiHelper.GetOrganization();
            if (organization == null || token.OrganizationCode != organization.Code)
                return false;

            var userEntity = UserSearch.BindFirst(
                x => new
                {
                    UserIdentifier = x.UserIdentifier,
                    Email = x.Email,
                    FirstName = x.FirstName,
                    LastName = x.LastName,
                    FullName = x.FullName,
                    TimeZone = x.TimeZone,
                    IsAdministrator = (bool?)x.Persons
                        .Where(p => p.OrganizationIdentifier == organization.Identifier)
                        .Select(p => p.IsAdministrator)
                        .FirstOrDefault()
                },
                new UserFilter { EmailExact = token.UserEmail });

            if (userEntity?.IsAdministrator == null)
                return false;

            var userPacket = new UserPacket
            {
                Identifier = userEntity.UserIdentifier,
                Email = userEntity.Email,
                FirstName = userEntity.FirstName,
                LastName = userEntity.LastName,
                FullName = userEntity.FullName,
                TimeZone = TimeZoneInfo.FindSystemTimeZoneById(userEntity.TimeZone),
            };
            var identity = new ApiUserIdentity(organization, userPacket, userEntity.IsAdministrator.Value);

            HttpContext.Current.User = identity;
            context.Principal = identity;

            return true;
        }

        private static (bool IsValid, string Error) ValidateJwt(string jwt)
        {
            try
            {
                var encoder = new JwtEncoder();

                var token = encoder.Decode(jwt);

                var security = ServiceLocator.AppSettings.Security;

                var secret = security.Secret;

                if (!encoder.VerifySignature(jwt, secret))
                    return (false, "Signature verification failed.");

                // Bearer tokens may be issued by the v1 API or by the v2 API, therefore we need to exclude Issuer from
                // the token validation check. In future, when all tokens are issued by the v2 API, then we can include
                // Issuer when we validate JWTs.

                if (!token.HasExpectedClaimValue(Shift.Common.ClaimName.Audience, security.Token.Audience))
                    return (false, "Audience validation failed.");

                if (token.IsExpired())
                    return (false, "Token is expired.");

                var converter = new Shift.Common.ClaimConverter(security);

                HttpContext.Current.User = converter.ToPrincipal(token);

                return (true, null);
            }
            catch (Exception ex)
            {
                return (false, ex.Message);
            }
        }

        private ApiAuthenticationType GetApiAuthenticationRequirement(HttpAuthenticationContext context)
        {
            if (context.ActionContext?.ControllerContext?.Controller is ApiOpenController)
                return ApiAuthenticationType.None;

            var anonymous = context.ActionContext?.ActionDescriptor?.GetCustomAttributes<AllowAnonymousAttribute>();
            if (anonymous != null && anonymous.Count > 0)
                return ApiAuthenticationType.None;

            anonymous = context.ActionContext?.ControllerContext?.ControllerDescriptor?.GetCustomAttributes<AllowAnonymousAttribute>();
            if (anonymous != null && anonymous.Count > 0)
                return ApiAuthenticationType.None;

            var requirements = context.ActionContext?.ActionDescriptor?.GetCustomAttributes<ApiAuthenticationRequirementAttribute>();
            if (requirements != null && requirements.Count > 0)
                return requirements.First().Type;

            requirements = context.ActionContext?.ControllerContext?.ControllerDescriptor?.GetCustomAttributes<ApiAuthenticationRequirementAttribute>();
            if (requirements != null && requirements.Count > 0)
                return requirements.First().Type;

            // Most of of the API controllers assume the old original authentication type, therefore we need to return
            // this as the default (for now).

            return ApiAuthenticationType.Header;
        }

        private async Task LogApiRequestAsync(HttpRequestMessage request, ApiValidationResult validation, Guid? responseLogId)
        {
            try
            {
                if (validation.OrganizationCode == null)
                    return;

                var organization = OrganizationSearch.Select(validation.OrganizationCode);

                if (organization == null)
                    return;

                var logger = new ApiRequestLogger(AppSentry.SentryError);

                await logger.InsertAsync(validation.Developer, organization.Identifier, request, "In", (validation.Success ? "Success" : "Failure"), validation.HostAddress, responseLogId);
            }
            catch (Exception ex)
            {
                AppSentry.SentryError(ex);
            }
        }

        public Task ChallengeAsync(HttpAuthenticationChallengeContext context, CancellationToken cancellation)
        {
            return Task.CompletedTask;
        }
    }

    public static class HttpContextExtensions
    {
        public static Principal GetIdentity(this HttpContext context)
        {
            return context.User as Principal;
        }
    }
}