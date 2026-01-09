using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

using Shift.Constant;

namespace Shift.Common
{
    public static class HtmlHelper
    {
        #region Classes

        public class LinkItem
        {
            public string HtmlTag { get; set; }
            public string Href { get; set; }
            public string Text { get; set; }
        }

        public class ResolveOptions
        {
            #region Properties

            public ResolvePath Anchor { get; set; }

            public ResolvePath Link { get; set; }

            public ResolvePath Form { get; set; }

            public ResolvePath Image { get; set; }

            public ResolvePath Script { get; set; }

            public ResolvePath Input { get; set; }

            #endregion

            #region Construction

            public ResolveOptions()
            {
                var all = ResolvePath.Internal | ResolvePath.Target;

                Anchor = all;
                Link = all;
                Form = all;
                Image = all;
                Script = all;
                Input = all;
            }

            #endregion
        }

        private class ResolveTagInfo
        {
            #region Properties

            public string AttributeName { get; }

            public Func<ResolveOptions, ResolvePath> GetResolvePath { get; }

            #endregion

            #region Construction

            public ResolveTagInfo(string attr, Func<ResolveOptions, ResolvePath> getPath)
            {
                AttributeName = attr;
                GetResolvePath = getPath;
            }

            #endregion
        }

        #endregion

        #region Fields

        private static readonly Regex _linkPattern = new Regex(@"<a.*?href=([""'])?(?<href>http.*?)[""'].*?>(?<text>.*?)</a>");

        private static readonly IDictionary<string, ResolveTagInfo> _tags = new Dictionary<string, ResolveTagInfo>(StringComparer.OrdinalIgnoreCase)
        {
            { "a", new ResolveTagInfo("href=", o => o.Anchor) },
            { "link", new ResolveTagInfo("href=", o => o.Link) },
            { "form", new ResolveTagInfo("action=", o => o.Form) },
            { "img", new ResolveTagInfo("src=", o => o.Image) },
            { "script", new ResolveTagInfo("src=", o => o.Script) },
            { "input", new ResolveTagInfo("src=", o => o.Input) }
        };

        #endregion

        #region Methods

        public static string EncodePotentiallyDangerousInclusions(string input) => 
            ReplacePotentiallyDangerousInclusions(input, ch =>
            {
                if (ch == '<')
                    return "&lt;";

                if (ch == '&')
                    return "&amp;";

                throw new ApplicationError("Invalid input char: " + ch);
            });

        public static string RemovePotentiallyDangerousInclusions(string input) => 
            ReplacePotentiallyDangerousInclusions(input, string.Empty);

        public static string ReplacePotentiallyDangerousInclusions(string input, string replaceWith = " ") => 
            ReplacePotentiallyDangerousInclusions(input, ch => replaceWith);

        public static string ReplacePotentiallyDangerousInclusions(string input, Func<char, string> replace)
        {
            if (string.IsNullOrEmpty(input) || input.Length == 1)
                return input;

            var output = new StringBuilder();

            var prevChar = input[0];
            for (var i = 1; i < input.Length; i++)
            {
                var ch = input[i];

                if (prevChar == '<' && (ch >= 'a' && ch <= 'z' || ch >= 'A' && ch <= 'Z' || ch == '!' || ch == '/' || ch == '?'))
                    output.Append(replace(prevChar));
                else if (prevChar == '&' && ch == '#')
                    output.Append(replace(prevChar));
                else
                    output.Append(prevChar);

                prevChar = ch;
            }

            output.Append(prevChar);

            return output.ToString();
        }

        public static string ResolveRelativePaths(string url, StringBuilder html, ResolveOptions options = null) =>
            ResolveRelativePaths(new Uri(url), html.ToString(), options);

        public static string ResolveRelativePaths(Uri uri, StringBuilder html, ResolveOptions options = null) =>
            ResolveRelativePaths(uri, html.ToString(), options);

        public static string ResolveRelativePaths(string url, string html, ResolveOptions options = null) =>
            ResolveRelativePaths(new Uri(url), html, options);

        private static string ResolveRelativePaths(Uri uri, string html, ResolveOptions options = null)
        {
            if (uri == null)
                throw new ArgumentNullException(nameof(uri));

            if (!uri.IsAbsoluteUri)
                throw new ArgumentException($@"URI is not absolute: {uri}", nameof(uri));

            if (options == null)
                options = new ResolveOptions();

            var result = new StringBuilder();
            var buffer = new StringBuilder();
            var state = 0;
            var searchValue2 = string.Empty;
            var searchIndex2 = -1;
            var searchValue4 = char.MinValue;
            ResolveTagInfo currectTag = null;

            foreach (var ch in html)
            {
                if (state == 0)
                {
                    if (ch == '<')
                        state = 1;

                    result.Append(ch);
                }
                else if (state == 1)
                {
                    if (ch == ' ' || ch == '>' || ch == '<')
                    {
                        var tagName = buffer.ToString();

                        if (_tags.ContainsKey(tagName))
                        {
                            currectTag = _tags[tagName];

                            state = 2;
                            searchIndex2 = 0;
                            searchValue2 = currectTag.AttributeName;
                        }
                        else
                        {
                            state = 0;
                            searchIndex2 = -1;
                            searchValue2 = string.Empty;
                        }

                        buffer.Clear();
                    }
                    else
                    {
                        buffer.Append(ch);
                    }

                    result.Append(ch);
                }
                else if (state == 2)
                {
                    if (ch == '>')
                    {
                        state = 0;
                        searchIndex2 = -1;
                        searchValue2 = string.Empty;
                    }
                    else if (ch == searchValue2[searchIndex2])
                    {
                        searchIndex2++;

                        if (searchValue2.Length == searchIndex2)
                        {
                            state = 3;
                            searchIndex2 = 0;
                        }
                    }
                    else if (searchIndex2 > 0)
                        searchIndex2 = 0;

                    result.Append(ch);
                }
                else if (state == 3)
                {
                    if (ch == '\'' || ch == '\"')
                    {
                        state = 4;
                        searchValue4 = ch;
                    }
                    else
                        state = 2;

                    result.Append(ch);
                }
                else if (state == 4)
                {
                    if (ch == searchValue4)
                    {
                        var path = buffer.ToString();
                        if (!path.StartsWith("data:"))
                        {
                            var resolvePath = currectTag.GetResolvePath(options);
                            var resolveInternal = resolvePath.HasFlag(ResolvePath.Internal);
                            var resolveTarget = resolvePath.HasFlag(ResolvePath.Target);
                            var resolveAll = resolveTarget && resolveInternal;
                            var resolveNone = !resolveTarget && !resolveInternal;

                            if (!resolveNone && (resolveAll || (!path.StartsWith("#") || resolveTarget) && (Uri.IsWellFormedUriString(path, UriKind.Absolute) || resolveInternal)))
                                if (Uri.IsWellFormedUriString(path, UriKind.Relative))
                                    path = new Uri(uri, path).AbsoluteUri;
                        }

                        result.Append(path);
                        result.Append(ch);
                        buffer.Clear();

                        state = 2;
                    }
                    else
                        buffer.Append(ch);
                }
                else
                    throw new NotImplementedException("Unexpected state: " + state);
            }

            return result.ToString();
        }

        public static string GetInnerText(string tag)
        {
            var innerText = Regex.Replace(tag, @"<(.|\n)*?>", "").Trim();

            if (string.IsNullOrEmpty(innerText))
                return string.Empty;

            innerText = innerText.Replace('\t', ' ');

            var parts = innerText.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
            var result = new StringBuilder();

            foreach (var t in parts)
            {
                var part = t.Trim();

                if (string.IsNullOrEmpty(part))
                    continue;

                if (result.Length > 0)
                    result.Append(" ");

                result.Append(part);
            }

            return result.ToString();
        }

        public static string GetPageCharset(string html)
        {
            if (!string.IsNullOrEmpty(html))
            {
                var regex = new Regex(@"<meta\b[^>]*/?>");
                var matches = regex.Matches(html);

                foreach (Match match in matches)
                {
                    var charsetIndex = match.Value.IndexOf("charset", StringComparison.OrdinalIgnoreCase);

                    if (charsetIndex > 0)
                        return GetCharsetValue(match.Value);
                }
            }

            return "utf-8";
        }

        public static LinkItem[] ExtractLinks(string html, int? hrefMaxLength = null, int? textMaxLength = null)
        {
            var links = new List<LinkItem>();

            if (!string.IsNullOrEmpty(html))
            {
                var matches = _linkPattern.Matches(html);

                foreach (Match match in matches)
                {
                    var href = match.Groups["href"].Value.Trim();
                    var text = match.Groups["text"].Value.Trim();

                    if (hrefMaxLength.HasValue && href.Length > 256)
                        continue;

                    if (textMaxLength.HasValue && text.Length > 256)
                        continue;

                    links.Add(new LinkItem
                    {
                        HtmlTag = match.Value,
                        Href = href,
                        Text = text
                    });
                }
            }

            return links.ToArray();
        }

        #endregion

        #region Helper methods

        private static string GetCharsetValue(string metaTag)
        {
            var regex = new Regex(@"charset *= *['""]?([^/'"" >]*)");
            var match = regex.Match(metaTag);

            return match.Success ? match.Groups[1].Value : "utf-8";
        }

        #endregion
    }
}
