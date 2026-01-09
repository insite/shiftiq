using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;

using Shift.Common;
using Shift.Constant;

namespace InSite.Web.Routing
{
    public static class SiteSettings
    {
        #region Properties

        public static string Subdomain => GetSubdomain(ServiceLocator.AppSettings.Environment);

        #endregion

        #region Methods (domain)

        public static string GetDomain(string host)
            => host.Replace($"{Subdomain}.", string.Empty);

        public static string GetSubdomain(EnvironmentModel environment)
        {
            switch (environment.Name)
            {
                case EnvironmentName.Production:
                    return "www";
                case EnvironmentName.Sandbox:
                    return "sandbox";
                case EnvironmentName.Development:
                    return "dev";
                default:
                    return "local";
            }
        }

        #endregion

        #region Methods (build URL)

        private static readonly Regex DomainPattern = new Regex("^([a-z0-9][a-z0-9\\-]*)(.[a-z0-9][a-z0-9\\-]*){1,2}$", RegexOptions.Compiled | RegexOptions.IgnoreCase);
        private static readonly Regex PathPattern = new Regex("^[a-z0-9][a-z0-9\\-]*$", RegexOptions.Compiled | RegexOptions.IgnoreCase);

        public static string GetUrl(IEnumerable<string> path) => GetUrl(ServiceLocator.AppSettings.Environment, path);

        public static string GetUrl(EnvironmentModel environment, IEnumerable<string> path)
        {
            var url = new StringBuilder();

            url.Append("http://").Append(GetSubdomain(environment)).Append(".");

            var index = 0;

            foreach (var p in path)
            {
                if (string.IsNullOrEmpty(p))
                    return null;

                if (index == 0)
                {
                    if (!DomainPattern.IsMatch(p))
                        return null;

                    url.Append(p);
                }
                else if (PathPattern.IsMatch(p))
                {
                    url.Append("/").Append(p);
                }
                else
                {
                    return null;
                }

                index++;
            }

            url.Append("?session=" + GenerateAuthenticationToken());

            return url.ToString();
        }

        public static string GetUrl(Guid page, bool portal, bool session)
        {
            return "http://" + GetSubdomain(ServiceLocator.AppSettings.Environment)
                + (portal ? "-" : ".")
                + ServiceLocator.PageSearch.GetPagePath(page, true)
                + (session ? "?session=" + GenerateAuthenticationToken() : string.Empty);
        }

        private static string GenerateAuthenticationToken()
        {
            var ipAddress = IPAddress.Parse(HttpContext.Current.Request.UserHostAddress);

            return StringHelper.EncodeBase64Url(EncryptionKey.Default, EncryptionHelper.GenerateSalt(4), stream =>
            {
                using (var writer = new BinaryWriter(stream))
                {
                    writer.Write(DateTime.UtcNow.AddSeconds(15));
                    writer.Write(ipAddress);
                    writer.Write(CookieTokenModule.Current.ID);
                }
            });
        }

        #endregion
    }
}