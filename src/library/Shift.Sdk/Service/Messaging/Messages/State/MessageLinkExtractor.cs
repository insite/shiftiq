using System;
using System.Collections.Generic;
using System.Linq;

using Shift.Common;

namespace InSite.Domain.Messages
{
    public static class MessageLinkExtractor
    {
        private static string[] _domains;

        public static void Initialize(string[] domains)
        {
            _domains = domains;
        }

        public static List<LinkItem> ExtractLinks(MultilingualString markdown)
        {
            var result = new List<LinkItem>();

            foreach (var text in markdown.Select(x => x.Value))
            {
                var html = Markdown.ToHtml(text);
                var links = HtmlHelper.ExtractLinks(html, 256, 256);

                foreach (var link in links)
                {
                    if (!IsValidAbsoluteUrl(link.Href))
                        continue;

                    if (!IsPermittedUrl(link.Href))
                        continue;

                    if (!IsAdded(link.Href))
                        result.Add(new LinkItem(link.HtmlTag, link.Href, link.Text));
                }
            }

            return result;

            bool IsAdded(string href)
                => result.Any(x => string.Equals(x.Href, href, StringComparison.OrdinalIgnoreCase));

            bool IsValidAbsoluteUrl(string href)
                => Uri.IsWellFormedUriString(href, UriKind.Absolute);

            bool IsPermittedUrl(string href)
            {
                var uri = new Uri(href);
                var host = uri.Host;
                
                var isInternal = _domains.Any(y => host.EndsWith(y, StringComparison.OrdinalIgnoreCase));
                if (!isInternal)
                    return true;
                
                return !href.EndsWith("links/click");
            }
        }
    }
}
