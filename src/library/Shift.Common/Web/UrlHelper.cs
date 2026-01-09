using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace Shift.Common
{
    public static class UrlHelper
    {
        public static readonly Regex[] YouTubeLinkPatterns = new[]
        {
            new Regex("^http(?:s)?://(?:www\\.)?youtube\\.com/watch\\?.*(?:v|&v)=(?<ID>[^&#]+)", RegexOptions.IgnoreCase | RegexOptions.Compiled),
            new Regex("^http(?:s)?://youtu\\.be/(?<ID>[^?#/]+)", RegexOptions.IgnoreCase | RegexOptions.Compiled),
            new Regex("^http(?:s)?://(?:www\\.)?youtube\\.com/embed/(?<ID>[^?#/]+)", RegexOptions.IgnoreCase | RegexOptions.Compiled)
        };

        public static readonly Regex[] VimeoLinkPatterns = new[]
        {
            new Regex("^http(?:s)?://vimeo\\.com/(?<ID>[a-z0-9]+)/(?<h>[a-z0-9]+)", RegexOptions.IgnoreCase | RegexOptions.Compiled),
            new Regex("^http(?:s)?://vimeo\\.com/(?<ID>[a-z0-9]+)", RegexOptions.IgnoreCase | RegexOptions.Compiled)
        };

        private static readonly HashSet<char> ValidMarkdownUrlChars;

        static UrlHelper()
        {
            ValidMarkdownUrlChars = new HashSet<char>
            {
                ':', '/', '.', '=', '?', '&', '#', '+', '%', '_', '~', '-', '!', '$', ',', '\'', '*', ';'
            };

            for (var ch = 'a'; ch <= 'z'; ch++)
                ValidMarkdownUrlChars.Add(ch);

            for (var ch = 'A'; ch <= 'Z'; ch++)
                ValidMarkdownUrlChars.Add(ch);

            for (var ch = '0'; ch <= '9'; ch++)
                ValidMarkdownUrlChars.Add(ch);
        }

        public static string EncodeMarkdownUrl(string input)
        {
            if (string.IsNullOrEmpty(input))
                return input;

            var output = new StringBuilder();

            for (var i = 0; i < input.Length; i++)
            {
                var ch = input[i];
                if (ValidMarkdownUrlChars.Contains(ch))
                    output.Append(ch);
                else
                    output.Append(Uri.HexEscape(ch));
            }

            return output.ToString();

        }

        public static string GetAbsoluteUrl(string domain, EnvironmentName environment, string relativeUrl, string organizationCode)
        {
            return GetAbsoluteUrl(domain, new EnvironmentModel(environment), relativeUrl, organizationCode);
        }

        public static string GetAbsoluteUrl(string domain, EnvironmentModel environment, string relativeUrl, string organizationCode)
        {
            if (string.IsNullOrEmpty(relativeUrl))
                return null;

            var url = new StringBuilder();
            url.Append("https://");
            url.Append(environment.GetSubdomainPrefix());
            url.Append(organizationCode);
            url.Append(".");
            url.Append(domain);

            if (relativeUrl.StartsWith("~", StringComparison.CurrentCulture))
                relativeUrl = relativeUrl.Substring(2);
            else if (relativeUrl.StartsWith("/", StringComparison.CurrentCulture))
                relativeUrl = relativeUrl.Substring(1);

            url.Append("/");
            url.Append(relativeUrl);

            return url.ToString();
        }

        public static string GetAbsoluteUrl(string serverUrl, string applicationPath, string relativeUrl)
        {
            if (string.IsNullOrEmpty(relativeUrl))
                return null;

            relativeUrl = relativeUrl.Trim();

            if (relativeUrl.StartsWith("~", StringComparison.CurrentCulture))
                relativeUrl = relativeUrl.Substring(2);
            else if (relativeUrl.StartsWith("/", StringComparison.CurrentCulture))
                relativeUrl = relativeUrl.Substring(1);

            int index = serverUrl.IndexOf("://", StringComparison.CurrentCulture);

            index = serverUrl.IndexOf('/', index + "://".Length);

            if (index >= 0)
                serverUrl = serverUrl.Substring(0, index);

            if (applicationPath != null && !applicationPath.EndsWith("/", StringComparison.CurrentCulture))
                applicationPath += "/";

            return $"{serverUrl}{applicationPath}{relativeUrl}";
        }

        public static string GetDomainName(string url)
        {
            if (!StringHelper.StartsWith(url, "http"))
                url = "https://" + url;

            var uri = new Uri(url);
            var host = uri.Host.ToLower();

            if (StringHelper.StartsWith(host, "www."))
                host = host.Substring(4);

            return host;
        }

        public static string GetFileName(string url)
        {
            if (string.IsNullOrEmpty(url))
                return url;

            var index = url.LastIndexOf('/');
            if (index == -1)
                return url;

            return url.Substring(index + 1);
        }

        public static Guid GetUserId(string relativePath)
        {
            if (string.IsNullOrEmpty(relativePath))
                return Guid.Empty;

            string crop = relativePath.Substring(0, relativePath.LastIndexOf('/'));
            crop = crop.Substring(0, crop.LastIndexOf('/'));
            crop = crop.Substring(6);
            if (!string.IsNullOrEmpty(crop))
                return new Guid(crop);

            return Guid.Empty;
        }

        public static string GetOrganizationCode(Uri url)
        {
            var host = url.Host;
            var subdomain = host.Substring(0, host.IndexOf('.'));

            if (subdomain.StartsWith("dev-"))
                return subdomain.Substring("dev-".Length);

            if (subdomain.StartsWith("local-"))
                return subdomain.Substring("local-".Length);

            if (subdomain.StartsWith("sandbox-"))
                return subdomain.Substring("sandbox-".Length);

            try
            {
                if (subdomain == "dev" || subdomain == "local" || subdomain == "sandbox" || subdomain == "www")
                {
                    int from = host.IndexOf(".") + ".".Length;
                    int to = host.LastIndexOf(".");
                    return host.Substring(from, to - from);
                }
            }
            catch { }

            return subdomain;
        }

        public static string GetOrganizationUrl(Uri requestUri, EnvironmentModel environment, string organizationCode, string relativeUrl)
        {
            var request = requestUri;

            var domain = requestUri.Host.Substring(requestUri.Host.IndexOf('.'));
            var subdomain = environment.GetSubdomainPrefix() + organizationCode;
            return requestUri.Scheme + "://" + subdomain + domain + (relativeUrl ?? string.Empty);
        }

        public static string GetWebSiteUrl(string absoluteUri)
        {
            string serverUrl = absoluteUri;
            int index = serverUrl.IndexOf("://", StringComparison.CurrentCulture);

            index = serverUrl.IndexOf('/', index + "://".Length);

            return index >= 0 ? serverUrl.Substring(0, index) : serverUrl;
        }
    }
}
