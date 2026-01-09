using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;

using InSite.Api.Settings;
using InSite.Persistence;

using Shift.Common;
using Shift.Constant;
using Shift.Sdk.UI;

namespace InSite.Api.Controllers
{
    [DisplayName("Security")]
    [RoutePrefix("api/oauth")]
    public class OAuthController : ApiOpenController
    {
        [HttpGet]
        [Route("microsoft")]
        public HttpResponseMessage Microsoft()
        {
            var queries = Request.GetQueryNameValuePairs();
            var code = queries.FirstOrDefault(x => x.Key == "Code");
            var state = queries.FirstOrDefault(x => x.Key == "Microsoft");
            return new HttpResponseMessage(HttpStatusCode.OK);
        }
    }

    [DisplayName("Security")]
    public class AuthenticationController : ApiOpenController
    {
        private readonly SecuritySettings _settings;
        private string _ipAddress;
        private List<string> _errors = new List<string>();

        public AuthenticationController()
        {
            _settings = Global.GetSecuritySettings();
        }

        [HttpPost]
        [Route("api/token")]
        public HttpResponseMessage GenerateToken([FromBody] JwtRequest request)
        {
            ServiceLocator.Logger.Debug("JWT request received. (Secret = {secret}, Lifetime = {lifetime}, Organization = {organization})", request.Secret, request.Lifetime, request.Organization);

            if (!RemoteIpAddressIsValid())
                return JsonUnauthorized("Invalid IP address");

            if (string.IsNullOrEmpty(request?.Secret))
                return JsonUnauthorized("Missing secret");

            try
            {
                var identity = GetIdentity(
                    request.Secret,
                    request.Organization, _settings.Token.Whitelist,
                    request.Lifetime, _settings.Token.Lifetime);

                if (identity == null)
                {
                    return _errors != null && _errors.Count > 0
                        ? JsonUnauthorized(string.Join(". ", _errors))
                        : JsonUnauthorized("Invalid secret");
                }

                if (identity.Claims != null && identity.Claims.IsExpired())
                {
                    return JsonUnauthorized("Expired secret");
                }

                var jwt = CreateToken(identity);

                var lifetimeInMinutes = identity.Claims.Lifetime ?? JwtRequest.DefaultLifetime;

                var body = new
                {
                    AccessToken = jwt,
                    TokenType = "Bearer",
                    ExpiresIn = lifetimeInMinutes * 60 // Time in seconds until the token expires
                };

                return JsonSuccess(body);
            }
            catch (Exception ex)
            {
                ServiceLocator.Logger.Error(ex, "An error occurred while generating a JWT.");

                AppSentry.SentryError(ex);

                return JsonError(ex.Message, HttpStatusCode.InternalServerError);
            }
        }

        [HttpPost]
        [Route("api/token/validate")]
        public async Task<HttpResponseMessage> ValidateToken()
        {
            string token = await Request.Content.ReadAsStringAsync();

            if (!RemoteIpAddressIsValid())
                return JsonUnauthorized("Invalid IP address");

            if (string.IsNullOrEmpty(token) || token.Count(c => c == '.') != 2)
                return JsonBadRequest("Invalid token format. A JWT must have 3 parts.");

            try
            {
                var encoder = new JwtEncoder();
                var jwt = encoder.Decode(token);
                return JsonSuccess(jwt);
            }
            catch (ArgumentException ex)
            {
                return JsonBadRequest($"Error processing token: {ex.Message}");
            }
        }

        /// <summary>
        /// Confirm authentication and authorization access to SiteBuilder
        /// </summary>
        /// <remarks>
        /// INTERNAL USE ONLY. This endpoint is used by Site Builder websites to authenticate site administrators. It 
        /// returns HTTP 200 OK only if the login credentials are a valid match for an administrator with permission 
        /// granted on the Site Builder tools.
        /// </remarks>
        [HttpPost]
        [Route("api/authenticate")]
        public HttpResponseMessage AuthenticateUser([FromBody] LoginModel login)
        {
            if (!RemoteIpAddressIsValid())
                return JsonUnauthorized("Invalid IP address");

            try
            {
                if (!login.OrganizationIdentifier.HasValue)
                    return JsonUnauthorized("Invalid Organization");

                if (login.Email.HasNoValue() || login.Password.HasNoValue())
                    return JsonUnauthorized("Invalid Credentials");

                var result = UserSearch.ValidateUser(login.Email, login.Password, out var user);
                if (result != AuthenticationResult.Success)
                    return JsonUnauthorized("Invalid User");

                if (!IsGranted(user, PermissionNames.Admin_Polaris))
                    return JsonUnauthorized("Unauthorized");

                var person = PersonSearch.Select(login.OrganizationIdentifier.Value, user.UserIdentifier);
                if (person?.UserAccessGranted == null)
                    return JsonUnauthorized("Unauthorized");

                return StringSuccess("OK");
            }
            catch (Exception ex)
            {
                ServiceLocator.Logger.Error(ex, "An error occurred while generating a JWT.");

                AppSentry.SentryError(ex);

                return JsonError(ex.Message, HttpStatusCode.InternalServerError);
            }
        }

        private string CreateToken(Principal identity)
        {
            var converter = new ClaimConverter(ServiceLocator.AppSettings.Security);

            var claims = converter.ToClaims(identity);

            var dictionary = converter.ToDictionary(claims);

            var expiry = DateTime.UtcNow.Add(TimeSpan.FromMinutes(identity.Claims.Lifetime ?? JwtRequest.DefaultLifetime));

            var issuer = Request.RequestUri.GetLeftPart(UriPartial.Path);

            var jwt = new Jwt(dictionary, identity.Name, issuer, _settings.Token.Audience, expiry);

            var encoder = new JwtEncoder();

            var secret = ServiceLocator.AppSettings.Security.Secret;

            return encoder.Encode(jwt, secret);
        }

        private Principal GetIdentity(string secret,
            Guid? organization, string whitelist,
            int? requestedTokenLifetime, int? defaultTokenLifetime)
        {
            return GetIdentityBySecret(secret,
                organization, whitelist, _ipAddress,
                requestedTokenLifetime, defaultTokenLifetime);
        }

        private Principal GetIdentityBySecret(string secret,
            Guid? organization, string whitelist, string ipAddress,
            int? requestedTokenLifetime, int? defaultTokenLifetime)
        {
            var converter = new ClaimConverter(ServiceLocator.AppSettings.Security);

            var proxy = GetRootProxy(secret, organization, ipAddress, whitelist);
            if (proxy != null)
            {
                proxy.Claims.Lifetime = converter.CalculateLifetime(proxy.Claims.Lifetime, null, requestedTokenLifetime);
                return proxy;
            }

            var person = GetPrincipal(secret);
            if (person == null)
                return null;

            person.Claims.Lifetime = converter.CalculateLifetime(person.Claims.Lifetime, requestedTokenLifetime, defaultTokenLifetime);
            return person;
        }

        private Principal GetPrincipal(string secretValue)
        {
            var secret = ServiceLocator.PersonSecretSearch.GetBySecretValue(secretValue, x => x.Person.Organization, x => x.Person.User.Memberships);

            if (secret?.Person?.User == null)
                return null;

            var person = secret.Person;

            var user = secret.Person.User;

            var principal = new Principal
            {
                User = new Actor
                {
                    Identifier = secret.Person.UserIdentifier,
                    Email = user.Email,
                    Name = user.FullName,
                    Phone = user.PhoneMobile,
                    Language = person.Language,
                    TimeZone = user.TimeZone
                },

                Organization = new Model
                {
                    Identifier = person.OrganizationIdentifier,
                    Name = person.Organization.CompanyName,
                    Slug = person.Organization.OrganizationCode
                }
            };

            principal.Claims.Expiry = secret.SecretExpiry;
            principal.Claims.Lifetime = secret.SecretLifetimeLimit ?? int.MaxValue;

            var groups = user.Memberships
                .Select(x => x.GroupIdentifier)
                .ToArray();

            principal.Roles = ServiceLocator.GroupSearch.BindGroups(
                x => new { x.GroupIdentifier, x.GroupName },
                x => x.GroupType == "Role" && groups.Contains(x.GroupIdentifier)
                )
                .Select(x => new Role(x.GroupName, x.GroupIdentifier))
                .ToList();

            return principal;
        }

        private Principal GetRootProxy(string secret, Guid? organizationId, string ipAddress, string whitelist)
        {
            var root = Global.GetRootSentinel();

            if (IsRootSecret(secret, root.Secret))
            {
                if (!IsWhitelisted(ipAddress, whitelist))
                {
                    _errors.Add($"API requests that use the root token secret must be sent from a whitelisted IP address. IP {ipAddress} is not whitelisted.");
                    return null;
                }

                var identity = new Principal
                {
                    User = new Actor
                    {
                        Identifier = root.Identifier,
                        Email = root.Email,
                        Name = root.Name,
                        Language = "en",
                        TimeZone = "UTC"
                    },

                    IPAddress = ipAddress
                };

                if (organizationId.HasValue)
                {
                    var organization = OrganizationSearch.Select(organizationId.Value);

                    identity.Organization = new Model
                    {
                        Identifier = organization.Identifier,
                        Name = organization.Name,
                        Slug = organization.Code
                    };
                }
                else
                {
                    var code = CookieTokenModule.Current.OrganizationCode;

                    var organization = OrganizationSearch.Select(code);

                    if (organization != null)
                    {
                        identity.Organization = new Model
                        {
                            Identifier = organization.OrganizationIdentifier,
                            Name = organization.Name,
                            Slug = organization.Code
                        };
                    }
                }

                return identity;
            }

            return null;
        }

        private static bool IsGranted(User user, string permissionName)
        {
            var roles = MembershipSearch.Select(
                x => x.UserIdentifier == user.UserIdentifier,
                x => x.User);

            foreach (var role in roles)
            {
                var group = ServiceLocator.GroupSearch.GetGroup(role.GroupIdentifier);
                if (group == null)
                    continue;

                var permissions = TGroupPermissionSearch.Select(x => x.GroupIdentifier == group.GroupIdentifier);
                var claims = new Domain.Foundations.ClaimList();

                foreach (var permission in permissions)
                {
                    var action = TActionSearch.Get(permission.ObjectIdentifier);

                    if (string.Compare(action?.ActionUrl, permissionName, StringComparison.OrdinalIgnoreCase) == 0)
                        return true;
                }
            }

            return false;
        }

        private bool IsRootSecret(string developerSecret, string rootSecret)
        {
            return developerSecret == rootSecret;
        }

        private bool IsWhitelisted(string ipAddress, string whitelist)
        {
            if (string.IsNullOrEmpty(ipAddress))
                return false;

            if (string.IsNullOrEmpty(whitelist))
                return false;

            return StringHelper.EqualsAny(ipAddress, StringHelper.Split(whitelist));
        }

        private HttpResponseMessage JsonBadRequest(string message)
        {
            return Request.CreateResponse(HttpStatusCode.BadRequest, new { success = false, message });
        }

        private bool RemoteIpAddressIsValid()
        {
            _ipAddress = GetClientIpAddress(Request);
            return !string.IsNullOrEmpty(_ipAddress);
        }

        private string GetClientIpAddress(HttpRequestMessage request)
        {
            // Assuming you have access to HttpContext.Current
            if (HttpContext.Current != null)
            {
                return HttpContext.Current.Request.UserHostAddress;
            }

            // If for some reason you don't have HttpContext.Current, you might try to get the information from request properties
            if (request.Properties.ContainsKey("MS_HttpContext"))
            {
                var context = request.Properties["MS_HttpContext"] as HttpContextWrapper;
                if (context != null)
                {
                    return context.Request.UserHostAddress;
                }
            }

            // For ASP.NET Core, you'd typically use IHttpContextAccessor to access HttpContext and then get the IP address
            // This example does not apply directly as HttpRequestMessage is not commonly used in ASP.NET Core for this purpose

            // If none of the above methods work, return null or an indication that the IP address couldn't be determined
            return null;
        }
    }
}