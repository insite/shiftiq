using System;
using System.Globalization;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;

using Shift.Common;
using Shift.Constant;

namespace InSite.Common.Web
{
    public static class HttpRequestHelper
    {
        private static readonly Regex MobileDetectRegex
            = new Regex("Android|webOS|iPhone|iPad|iPod|BlackBerry", RegexOptions.IgnoreCase | RegexOptions.Compiled);

        public static string CurrentRootUrl
        {
            get
            {
                var request = HttpContext.Current.Request;
                return "https://" + request.Url.Host;
            }
        }

        public static string CurrentRootUrlFiles
            => CurrentRootUrl + RelativeUrl.FileAppSubfolder;

        public static bool IsMobileDevice => !string.IsNullOrEmpty(HttpContext.Current.Request.UserAgent) && MobileDetectRegex.IsMatch(HttpContext.Current.Request.UserAgent);

        public static string ApplicationPath => HttpContext.Current.Request.ApplicationPath;

        public static bool IsAjaxRequest => HttpContext.Current.Request.Headers["X-Requested-With"] == "XMLHttpRequest";

        public static bool IsIE
        {
            get
            {
                var browser = HttpContext.Current.Request.Browser.Browser;
                return string.Equals(browser, "IE", StringComparison.OrdinalIgnoreCase)
                    || string.Equals(browser, "InternetExplorer", StringComparison.OrdinalIgnoreCase);
            }
        }

        public static string GetAbsoluteUrl(string relativeUrl)
        {
            relativeUrl = relativeUrl.Trim();

            if (Uri.TryCreate(relativeUrl, UriKind.Absolute, out var abs))
                return abs.ToString();

            var request = HttpContext.Current.Request;
            var applicationPath = @"/";

            if (relativeUrl.StartsWith("~", StringComparison.CurrentCulture))
            {
                relativeUrl = relativeUrl.Substring(2);
                applicationPath = request.ApplicationPath;
            }
            else if (relativeUrl.StartsWith("/", StringComparison.CurrentCulture))
            {
                relativeUrl = relativeUrl.Substring(1);
            }

            var serverUrl = request.Url.AbsoluteUri;
            var index = serverUrl.IndexOf("://", StringComparison.CurrentCulture);

            index = serverUrl.IndexOf('/', index + "://".Length);

            if (index >= 0)
                serverUrl = serverUrl.Substring(0, index);

            if (applicationPath != null && !applicationPath.EndsWith("/", StringComparison.CurrentCulture))
                applicationPath += "/";

            return string.Format(CultureInfo.CurrentCulture, "{0}{1}{2}", serverUrl, applicationPath, relativeUrl);
        }

        public static bool IsUrlFilePathValid(string url)
        {
            try
            {
                HttpWebRequest request = (HttpWebRequest)System.Net.WebRequest.Create(url);
                using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
                    return (response.StatusCode == HttpStatusCode.OK ? true : false);
            }
            catch
            {
            }
            return false;
        }

        public static int GetParameterValueAsInt32(string name)
        {
            var stringValue = GetParameterValueAsString(name);

            if (ValueConverter.IsInt32(stringValue))
                return ValueConverter.ToInt32(stringValue);

            throw new ArgumentOutOfRangeException(nameof(name), $"Parameter Not Found: {name}");
        }

        public static int? GetParameterValueAsInt32Nullable(string name)
        {
            var stringValue = GetParameterValueAsString(name);

            if (ValueConverter.IsInt32(stringValue))
                return ValueConverter.ToInt32(stringValue);

            return null;
        }

        public static Guid GetParameterValueAsGuid(string name)
        {
            var stringValue = GetParameterValueAsString(name);

            if (!Guid.TryParse(stringValue, out var result))
                return result;

            throw new ArgumentOutOfRangeException(nameof(name), $"Parameter Not Found: {name}");
        }

        public static Guid? GetParameterValueAsGuidNullable(string name)
        {
            var stringValue = GetParameterValueAsString(name);

            return Guid.TryParse(stringValue, out var result) ? result : (Guid?)null;
        }

        public static string GetParameterValueAsString(string name)
        {
            var parameters = HttpContext.Current.Request.QueryString;

            // If there are multiple parameter values then they will appear as a comma-separated list. In this scenario
            // take the first item in the list.

            if (!string.IsNullOrEmpty(parameters[name]))
            {
                var textValue = parameters[name];
                if (textValue.Contains(","))
                {
                    var arrayValue = StringHelper.Split(textValue, ',');
                    return arrayValue[0];
                }
                return textValue;
            }

            return null;
        }

        public static string ResolveUrl(string url)
        {
            return url != null && url.StartsWith("~") ? $"{ApplicationPath}{url.Substring(1)}".Replace("//", "/") : url;
        }

        public static string GetDomainName(int level)
        {
            if (level <= 0)
                throw ApplicationError.Create("The domain level can't be less than or equal to zero.");

            var host = HttpContext.Current.Request.Url.Host;
            var parts = host.Split('.');
            if (level >= parts.Length)
                return host;

            var result = new StringBuilder();

            for (var i = 0; i < parts.Length && i < level; i++)
                result.Insert(0, parts[parts.Length - i - 1]).Insert(0, ".");

            return result.ToString();
        }

        public static string GetRawUrl(HttpRequest request)
        {
            if (request == null)
                return null;

            if (request.RawUrl != null)
            {
                var index = request.RawUrl.IndexOf('?');
                return index != -1
                    ? request.RawUrl.Substring(0, index)
                    : request.RawUrl;
            }

            if (request?.Url?.AbsolutePath != null)
            {
                var path = request.Url.AbsolutePath;
                var index = path.IndexOf('?');
                return index != -1
                    ? path.Substring(0, index)
                    : path;
            }

            return request.Url.ToString();
        }

        public static WebUrl GetCurrentWebUrl() => new WebUrl(HttpContext.Current.Request.RawUrl);

        public static bool IsEmbeddedResource(HttpRequest request)
        {
            var extension = VirtualPathUtility.GetExtension(request.Path);

            return StringHelper.ContainsAny(extension, FileExtension.EmbeddedResources);
        }

        public static bool IsStaticAsset(HttpRequest request)
        {
            var extension = VirtualPathUtility.GetExtension(request.Path);

            return StringHelper.ContainsAny(extension, FileExtension.StaticAssets)
                && StringHelper.StartsWithAny(extension, new[] { "/api" });
        }
    }
}
