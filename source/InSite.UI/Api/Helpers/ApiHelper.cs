using System;
using System.Linq;
using System.Net.Http;
using System.Web;
using System.Web.Http.Filters;

using InSite.Application.Contacts.Read;
using InSite.Domain.Organizations;
using InSite.Persistence;
using InSite.Persistence.Integration;

using Microsoft.Extensions.Logging.Abstractions;

using Shift.Common;
using Shift.Constant;

using UserPacket = InSite.Domain.Foundations.User;

namespace InSite.Api.Settings
{
    public static class ApiHelper
    {
        public static string OrganizationCode
        {
            get
            {
                var key = typeof(ApiHelper) + "." + nameof(OrganizationCode);
                if (HttpContext.Current.Items[key] == null)
                    HttpContext.Current.Items[key] = UrlHelper.GetOrganizationCode(HttpContext.Current.Request.Url);
                return (string)HttpContext.Current.Items[key];
            }
            set => HttpContext.Current.Items[typeof(ApiHelper) + "." + nameof(OrganizationCode)] = value;
        }

        public static string UserEmail
        {
            get => (string)HttpContext.Current.Items[typeof(ApiHelper) + "." + nameof(UserEmail)];
            set => HttpContext.Current.Items[typeof(ApiHelper) + "." + nameof(UserEmail)] = value;
        }

        public static Guid? UserIdentifier
        {
            get => (Guid?)HttpContext.Current.Items[typeof(ApiHelper) + "." + nameof(UserIdentifier)];
            set => HttpContext.Current.Items[typeof(ApiHelper) + "." + nameof(UserIdentifier)] = value;
        }

        public static OrganizationState GetOrganization()
            => OrganizationSearch.Select(OrganizationCode);

        public static string GetApiKey()
        {
            var user = CurrentSessionState.Identity?.User;
            if (user == null)
                return null;

            var context = HttpContext.Current;
            var sessionKey = typeof(ApiHelper).FullName + ".UserApiKey[" + user.Identifier + "]";
            var apiKey = (string)context.Session[sessionKey];

            if (apiKey == null)
                context.Session[sessionKey] = apiKey = StringHelper.EncodeBase64Url(EncryptionKey.Default, stream =>
                {
                    var random = new RandomNumberGenerator();

                    var data = user.Identifier.ToByteArray();
                    for (var i = 0; i < data.Length; i++)
                    {
                        stream.WriteByte(data[i]);

                        if (i == 3 || i == 6 || i == 8 || i == 11)
                            stream.WriteByte((byte)random.Next(0, 256));
                    }
                });

            return apiKey;
        }

        public static bool TryReadApiKey(string value, out Guid key)
        {
            key = Guid.Empty;

            var keyObj = StringHelper.DecodeBase64Url(value, EncryptionKey.Default, stream =>
            {
                var data = new byte[16];
                var dataIndex = 0;

                for (var i = 0; i < 20; i++)
                {
                    var b = stream.ReadByte();

                    if (b < 0)
                        return null;

                    if (i != 4 && i != 8 && i != 11 && i != 15)
                        data[dataIndex++] = (byte)b;
                }

                return stream.ReadByte() < 0
                    ? new Guid(data)
                    : (Guid?)null;
            });

            if (keyObj != null)
            {
                key = (Guid)keyObj;

                return true;
            }

            return false;
        }

        public static ApiValidationResult Validate(HttpAuthenticationContext context)
        {
            var organization = GetOrganization();
            if (organization == null)
                return new ApiValidationResult(false, $"Organization account not found: {OrganizationCode}");

            var result = ValidateUserToken(context, organization);
            if (!result.Success)
                result = ValidateDeveloperToken(context, organization);

            result.HostAddress = HttpContext.Current?.Request?.UserHostAddress;
            result.IsLocal = HttpContext.Current?.Request?.IsLocal ?? false;
            result.OrganizationCode = OrganizationCode;

            return result;
        }

        private static ApiValidationResult ValidateUserToken(HttpAuthenticationContext context, OrganizationState organization)
        {
            var userValue = HttpContext.Current?.Request?.Headers["user"];
            if (string.IsNullOrEmpty(userValue))
                return new ApiValidationResult(false, @"Your HTTP request is missing the User header.");

            var userEntity = TryReadApiKey(userValue.CleanTrim(), out var userIdentifier)
                ? UserSearch.Select(userIdentifier)
                : null;

            if (userEntity == null)
                return new ApiValidationResult(false, $"Invalid user authorization token: {userValue}");

            var userPacket = new UserPacket
            {
                Identifier = userEntity.UserIdentifier,
                FirstName = userEntity.FirstName,
                LastName = userEntity.LastName,
                FullName = userEntity.FullName,
                TimeZone = TimeZoneInfo.FindSystemTimeZoneById(userEntity.TimeZone)
            };

            var isAdministrator = PersonCriteria.BindFirst(
                x => (bool?)x.IsAdministrator,
                new PersonFilter
                {
                    OrganizationIdentifier = organization.OrganizationIdentifier,
                    UserIdentifier = userIdentifier
                });

            context.Principal = new ApiUserIdentity(organization, userPacket, isAdministrator ?? false);

            UserEmail = userPacket.Email;
            UserIdentifier = userPacket.Identifier;

            return new ApiValidationResult(true, null);
        }

        private static ApiValidationResult ValidateDeveloperToken(HttpAuthenticationContext context, OrganizationState organization)
        {
            var result = ValidateAuthorizationHeader(context.Request);

            if (result.Success && result?.Developer?.User != null)
            {
                UserEmail = result.Developer.User.Email;
                UserIdentifier = result.Developer.User.Identifier;

                var isAdministrator = PersonCriteria.BindFirst(
                    x => x.IsAdministrator,
                    new PersonFilter
                    {
                        OrganizationIdentifier = organization.OrganizationIdentifier,
                        UserIdentifier = UserIdentifier.Value
                    });

                context.Principal = new ApiUserIdentity(organization, result.Developer.User, isAdministrator);
            }

            return result;
        }

        public static ApiValidationResult ValidateAuthorizationHeader(HttpRequestMessage request)
        {
            try
            {
                // Requests submitted the CMDS API must be authorized.

                var headers = request.Headers;

                // The HTTP request must include the Authorization header.

                if (!headers.Contains("Authorization"))
                    return new ApiValidationResult(false, @"Your HTTP request is missing the Authorization header.");

                var header = headers.GetValues("Authorization").First();

                // The Authorization header must be a cryptographically strong Bearer Token.

                const string authorizationPrefix = "Bearer ";

                if (!header.StartsWith(authorizationPrefix))
                    return new ApiValidationResult(false, @"The Authorization header in your HTTP request must contain a bearer token.");

                UserPacket user = null;
                QPersonSecret secret = null;

                var token = header.Substring(authorizationPrefix.Length);
                if (!string.IsNullOrWhiteSpace(token))
                {
                    secret = ServiceLocator.PersonSecretSearch.GetBySecretValue(token,
                        x => x.Person,
                        x => x.Person.User,
                        x => x.Person.Organization);

                    if (secret?.Person?.User != null)
                    {
                        var person = secret.Person;
                        var userId = person.UserIdentifier;
                        var org = person.Organization;

                        if (org == null || org.OrganizationCode == "*")
                            user = UserSearch.SelectWebContact(userId, OrganizationIdentifiers.Global);
                        else
                            user = UserSearch.SelectWebContact(userId, org.OrganizationIdentifier);
                    }
                }

                // The bearer must be pre-authorized.

                if (user == null)
                    return new ApiValidationResult(false, @"Your bearer token is not recognized.");

                if (secret.SecretExpiry <= DateTimeOffset.UtcNow)
                    return new ApiValidationResult(false, @"Your bearer token is expired.");

                var orgCode = secret.Person?.Organization?.OrganizationCode ?? "*";
                if (orgCode != "*" && !string.Equals(orgCode, OrganizationCode, StringComparison.OrdinalIgnoreCase))
                    return new ApiValidationResult(false, @"Your bearer token is not authorized to access this organization.");

                var result = new ApiValidationResult(true, null)
                {
                    Developer = new ApiDeveloper(
                        orgCode,
                        secret.SecretName,
                        secret.Person?.User?.Email ?? "",
                        secret.SecretValue,
                        secret.SecretExpiry)
                    {
                        User = user
                    }
                };

                return result;
            }
            catch (Exception ex)
            {
                return new ApiValidationResult(false, CatchError(ex, nameof(ValidateAuthorizationHeader)));
            }
        }

        private static string CatchError(Exception ex, string method)
        {
            var source = typeof(ApiHelper).FullName + "." + method;
            var error = $"{source}: An expected error occurred during validation of the InSite API Authorization header. {ex.Message}";
            AppSentry.SentryError(error, source);
            return error;
        }
    }
}