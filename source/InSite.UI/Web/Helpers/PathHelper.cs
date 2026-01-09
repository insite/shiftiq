using System;
using System.Linq;
using System.Web;

using Shift.Common;

namespace InSite.UI
{
    public static class PathHelper
    {
        private static readonly (string Relative, string Physical)[] _rootPaths;

        static PathHelper()
        {
            _rootPaths = new (string, string)[]
            {
                ("/", HttpContext.Current.Server.MapPath("/")),
                ("/library/", HttpContext.Current.Server.MapPath("/library/"))
            };
        }

        public static string ToAbsoluteUrl(string relativeUrl)
        {
            if (string.IsNullOrEmpty(relativeUrl))
                return relativeUrl;

            if (HttpContext.Current == null)
                return relativeUrl;

            if (relativeUrl.StartsWith("/"))
                relativeUrl = relativeUrl.Insert(0, "~");
            if (!relativeUrl.StartsWith("~/"))
                relativeUrl = relativeUrl.Insert(0, "~/");

            var url = HttpContext.Current.Request.Url;
            var port = url.Port != 80 && url.Port != 443 ? ":" + url.Port : string.Empty;
            var host = $"{url.Scheme}://{url.Host}{port}";

            if (host.EndsWith("/"))
                host.Substring(0, host.Length - 1);

            return host + VirtualPathUtility.ToAbsolute(relativeUrl);
        }

        public static string AddStartingSlash(string path, char slash) =>
            path.Length > 0 && char.ToUpperInvariant(path[0]) == char.ToUpperInvariant(slash) ? path : slash + path;

        public static string RemoveStartingSlash(string path, char slash) =>
            path.Length > 0 && char.ToUpperInvariant(path[0]) == char.ToUpperInvariant(slash) ? path.Substring(1) : path;

        public static string AddEndingSlash(string path, char slash) =>
            path.Length > 0 && char.ToUpperInvariant(path[path.Length - 1]) == char.ToUpperInvariant(slash) ? path : path + slash;

        public static string RemoveEndingSlash(string path, char slash)
        {
            var lastIndex = path.Length - 1;
            return path.Length > 0 && char.ToUpperInvariant(path[lastIndex]) == char.ToUpperInvariant(slash)
                ? path.Substring(0, lastIndex) : path;
        }

        public static string PhysicalToRelativePath(string physicalPath)
        {
            if (physicalPath == null)
                throw new ArgumentNullException(nameof(physicalPath));

            var rootPath = _rootPaths
                .Where(x => physicalPath.StartsWith(x.Physical, StringComparison.OrdinalIgnoreCase))
                .FirstOrDefault();

            if (rootPath == default)
                throw ApplicationError.Create("Invalid root path");

            return rootPath.Relative + physicalPath.Substring(rootPath.Physical.Length).Replace("\\", "/");
        }

        public static string GetAbsoluteUrl(string relativeUrl)
        {
            var request = HttpContext.Current.Request;

            var applicationPath = request.ApplicationPath;

            var serverUrl = request.Url.AbsoluteUri;

            return UrlHelper.GetAbsoluteUrl(serverUrl, applicationPath, relativeUrl);
        }

        public static string GetOrganizationUrl(EnvironmentModel environment, string organizationCode, string relativeUrl)
        {
            var requestUri = HttpContext.Current.Request.Url;

            return UrlHelper.GetOrganizationUrl(requestUri, environment, organizationCode, relativeUrl);
        }
    }
}
