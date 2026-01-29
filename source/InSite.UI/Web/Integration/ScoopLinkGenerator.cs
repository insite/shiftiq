using System;
using System.Collections.Generic;
using System.Configuration;
using System.Web;

using InSite.Domain.Foundations;

namespace InSite.Web.Integration
{
    /// <summary>
    /// Generates links to Scoop with support for token relay authentication
    /// </summary>
    public class ScoopLinkGenerator
    {
        private readonly string _scoopBaseUrl;
        private readonly bool _relayTokenEnabled;
        private readonly string _relayTokenSecret;
        private readonly int _relayTokenLifetimeInSeconds;

        /// <summary>
        /// Creates a new ScoopLinkGenerator using configuration from appsettings
        /// </summary>
        public ScoopLinkGenerator()
        {
            var settings = ServiceLocator.AppSettings.Engine.Api.Scoop;

            _relayTokenEnabled = settings.Relay.Enabled;

            _scoopBaseUrl = settings.BaseUrl?.TrimEnd('/')
                ?? throw new ConfigurationErrorsException("Missing Scoop:BaseUrl configuration");

            _relayTokenSecret = settings.Relay.Secret
                ?? throw new ConfigurationErrorsException("Missing Scoop:Relay:Secret configuration");

            _relayTokenLifetimeInSeconds = settings.Relay.GetLifetimeInMinutes() * 60;
        }

        /// <summary>
        /// Generates an authenticated URL to Scoop for the given user
        /// </summary>
        public string GenerateAuthenticatedUrl(ISecurityFramework user, string domain, string targetPath, string exitUrl)
        {
            if (user == null)
                throw new ArgumentNullException(nameof(user));

            if (string.IsNullOrEmpty(domain))
                throw new ArgumentNullException(nameof(domain));

            var token = CreateRelayToken(user, domain, targetPath, exitUrl);

            var encodedToken = token.Encode(_relayTokenSecret);

            var callbackUrl = $"{_scoopBaseUrl}/lobby/auth/callback?token={HttpUtility.UrlEncode(encodedToken)}";

            return callbackUrl;
        }

        /// <summary>
        /// Generates an unauthenticated URL to Scoop
        /// </summary>
        /// <remarks>
        /// If authentication is needed, then shared cookie authentication is assumed.
        /// </remarks>
        public string GenerateUnauthenticatedUrl(string targetPath)
        {
            var relativePath = targetPath.TrimStart('/');

            var callbackUrl = $"{_scoopBaseUrl}/{relativePath}";

            return callbackUrl;
        }

        /// <summary>
        /// Generates a URL to launch a specific course
        /// </summary>
        public string GenerateCourseUrl(ISecurityFramework user, string domain, string accountSlug, string courseSlug, string exitUrl)
        {
            var targetPath = $"/{accountSlug}/{courseSlug}";

            return GenerateAuthenticatedUrl(user, domain, targetPath, exitUrl);
        }

        /// <summary>
        /// Generates a URL to the course library
        /// </summary>
        public string GenerateLibraryUrl(ISecurityFramework user, string domain, string accountSlug, string exitUrl)
        {
            var path = $"admin/library";

            return _relayTokenEnabled
                ? GenerateAuthenticatedUrl(user, domain, path, exitUrl)
                : GenerateUnauthenticatedUrl(path);
        }

        /// <summary>
        /// Generates an authenticated URL to the admin dashboard
        /// </summary>
        public string GenerateDashboardUrl(ISecurityFramework user, string domain, string accountSlug, string exitUrl)
        {
            var path = $"admin/dashboard";

            return _relayTokenEnabled
                ? GenerateAuthenticatedUrl(user, domain, path, exitUrl)
                : GenerateUnauthenticatedUrl(path);
        }

        private RelayToken CreateRelayToken(ISecurityFramework user, string domain, string targetPath, string exitUrl)
        {
            var now = DateTimeOffset.UtcNow;

            return new RelayToken
            {
                TokenId = Guid.NewGuid().ToString("N"),
                IssuedAt = now.ToUnixTimeSeconds(),
                ExpiresAt = now.AddSeconds(_relayTokenLifetimeInSeconds).ToUnixTimeSeconds(),
                Issuer = domain,  // This determines account isolation in Scoop
                UserId = user.User.Identifier,
                Email = user.User.Email,
                Name = user.Name,
                OrganizationCode = user.Organization.Code,
                OrganizationId = user.OrganizationId,
                Roles = GetUserRoles(user),
                TargetUrl = targetPath,
                ExitUrl = exitUrl
            };
        }

        private static string[] GetUserRoles(ISecurityFramework user)
        {
            var roles = new List<string>();

            if (user.Person.IsLearner)
                roles.Add("learner");

            if (user.Person.IsOperator)
                roles.Add("operator");

            if (user.Person.IsAdministrator)
                roles.Add("administrator");

            // Default to learner if no roles specified
            if (roles.Count == 0)
                roles.Add("learner");

            return roles.ToArray();
        }
    }
}
