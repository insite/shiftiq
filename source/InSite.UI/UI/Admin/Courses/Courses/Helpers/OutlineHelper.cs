using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

using InSite.UI;

using Shift.Common;

using HttpUtility = System.Web.HttpUtility;

namespace InSite.Admin.Courses.Courses
{
    internal static class OutlineHelper
    {
        public static readonly string[] AllowedVideoFormats = new[] { ".mp4", ".m4v", ".webm", ".webp", ".ogv" };

        public static string GenerateContent(string type, string target, string url)
        {
            url = url?.CleanTrim();

            if (url.IsNotEmpty())
            {
                if (StringHelper.Equals(type, "Document"))
                    return GenerateEmbedDocument(url);
                else if (StringHelper.Equals(type, "Link"))
                    return GenerateEmbedLink(url);
                else if (StringHelper.Equals(type, "Video"))
                    return StringHelper.Equals(target, "_embed")
                        ? GenerateEmbedVideo(url, 570, 350)
                        : $"<a target={HttpUtility.JavaScriptStringEncode(target, true)} href={HttpUtility.JavaScriptStringEncode(url, true)}>Watch Video</a>";
            }

            return null;
        }

        public static string GenerateEmbedDocument(string url)
        {
            url = url?.CleanTrim();
            if (url.IsNotEmpty())
            {
                var ext = GetExtension(url);

                if (FileExtension.IsImage(ext))
                    return $"<img alt='' src={HttpUtility.JavaScriptStringEncode(url, true)} style='max-width:100%;' />";
                else if (ext == ".pdf")
                    return $"<embed src={HttpUtility.JavaScriptStringEncode(url, true)} width='100%' height='800px' />";
            }

            return null;
        }

        public static string GenerateEmbedLink(string url)
        {
            url = url?.CleanTrim();
            if (url.IsNotEmpty())
            {
                var ext = GetExtension(url);

                if (FileExtension.IsImage(ext))
                    return $"<img alt='' src={HttpUtility.JavaScriptStringEncode(GetHttpsUrl(url), true)} style='max-width:100%;' />";
                else if (string.Equals(ext, ".pdf", StringComparison.OrdinalIgnoreCase))
                    return $"<embed src={HttpUtility.JavaScriptStringEncode(GetHttpsUrl(url), true)} width='100%' height='800px' />";
                else
                    return $"<iframe src={HttpUtility.JavaScriptStringEncode(GetHttpsUrl(url), true)} style='width:100%; height:800px; border:none;' sandbox='allow-same-origin'></iframe>";
            }

            return null;
        }

        public static string GenerateEmbedVideo(string url, int width, int height)
        {
            url = url?.CleanTrim();
            if (url.IsNotEmpty())
            {
                var ext = GetExtension(url);

                if (ext.IsNotEmpty() && AllowedVideoFormats.Any(x => x.Equals(ext, StringComparison.OrdinalIgnoreCase)))
                {
                    var u = new UrlParser().Parse(url);
                    if (u.Type == UrlParser.UrlType.Relative)
                        url = PathHelper.GetAbsoluteUrl(url);

                    return "<video width='570' height='350' title='Video Player' controls>" +
                        $"<source src={url} type='{MimeMapping.GetContentType("file" + ext)}'>" +
                        "Your browser does not support the video tag." +
                        "</video>";
                }
                else
                {
                    string playerUrl = null;

                    foreach (var regex in UrlHelper.YouTubeLinkPatterns)
                    {
                        var match = regex.Match(url);
                        if (!match.Success)
                            continue;

                        var videoId = match.Groups["ID"]?.Value;

                        var webUrl = new WebUrl(url);
                        var startTime = StringHelper.FirstValue(
                            webUrl.QueryString["start"],
                            webUrl.QueryString["t"]);

                        if (videoId.IsNotEmpty())
                        {
                            playerUrl = $"https://www.youtube.com/embed/{videoId}";

                            if (startTime.IsNotEmpty())
                                playerUrl += $"?start={startTime}";

                            break;
                        }
                    }

                    if (playerUrl.IsEmpty())
                    {
                        foreach (var regex in UrlHelper.VimeoLinkPatterns)
                        {
                            var match = regex.Match(url);
                            if (!match.Success)
                                continue;

                            var videoId = match.Groups["ID"].Value;
                            var videoH = match.Groups["h"].Value;

                            var query = string.Empty;
                            {
                                var qStartIndex = url.IndexOf('?');

                                if (qStartIndex == -1)
                                    qStartIndex = url.IndexOf('#');

                                if (qStartIndex != -1)
                                    query = url.Substring(qStartIndex);
                            }

                            if (videoId.IsNotEmpty())
                            {
                                playerUrl = $"https://player.vimeo.com/video/{videoId}?h={videoH}&amp;badge=0&amp;autopause=0&amp;player_id=0&amp;app_id=58479";
                                break;
                            }
                        }
                    }

                    if (playerUrl.IsNotEmpty())
                        return
                              $"<iframe title='Video Player' width={width} height={height}"
                            + $" frameborder='0' allow='autoplay; fullscreen; picture-in-picture' allowfullscreen='' "
                            + $" src={playerUrl} "
                            + $"></iframe>"
                            + $"<script src='https://player.vimeo.com/api/player.js'></script>";
                }
            }

            return null;
        }

        public static string GetLinkLanguage(string url)
        {
            var fileName = UrlHelper.GetFileName(url);
            if (string.IsNullOrEmpty(fileName))
                return null;

            var extIndex = fileName.LastIndexOf('.');
            if (extIndex < 0)
                return null;

            var langIndex = fileName.LastIndexOf('-', extIndex);

            return langIndex > 0 && extIndex - langIndex == 3
                ? fileName.Substring(langIndex + 1, 2).ToLower()
                : null;
        }

        public static string GetLinkByLanguage(string url, string lang)
        {
            if (string.IsNullOrEmpty(url))
                return null;

            var fileNameIndex = url.LastIndexOf('/');
            if (fileNameIndex == -1)
                return null;

            var extIndex = url.LastIndexOf('.');
            if (extIndex < fileNameIndex)
                return null;

            var langIndex = url.LastIndexOf('-', extIndex);

            if (langIndex < fileNameIndex || extIndex - langIndex != 3)
                return null;

            var result = new StringBuilder();
            result.Append(url.Substring(0, langIndex + 1));
            result.Append(lang);
            result.Append(url.Substring(extIndex));

            return result.ToString();
        }

        public static List<string> GetFileUrls(Guid courseId)
        {
            var files = Directory.GetFiles(GetFolderPhysicalPath(courseId));
            var list = new List<string>();

            foreach (var file in files)
            {
                var fileName = Path.GetFileName(file);
                list.Add($"/in-content/courses/{courseId}/{fileName}");
            }

            list.Sort();

            return list;
        }

        public static List<string> GetOtherLanguageLinks(Guid courseId, string activityUrl)
        {
            var templateUrl = GetLinkByLanguage(activityUrl, "aa");
            if (templateUrl == null)
                return null;

            var result = new List<string>();
            var urls = GetFileUrls(courseId);

            foreach (var url in urls)
            {
                if (string.Equals(activityUrl, url))
                    continue;

                var currentLanguageUrl = GetLinkByLanguage(url, "aa");

                if (string.Equals(templateUrl, currentLanguageUrl, StringComparison.OrdinalIgnoreCase))
                    result.Add(url);
            }

            result.Sort();

            return result;
        }

        public static string GetFolderPhysicalPath(Guid courseId)
        {
            var organizationCode = CurrentSessionState.Identity.Organization.Code;
            return ServiceLocator.FilePaths.GetPhysicalPathToShareFolder("Files", "Tenants", organizationCode, "Courses", courseId.ToString());
        }

        #region Methods (helpers)

        private static string GetExtension(string url)
        {
            var queryStart = url.IndexOf('?');
            var extUrl = queryStart > 0 ? url.Substring(0, queryStart) : url;
            return FileExtension.GetExtension(extUrl);
        }

        private static string GetHttpsUrl(string url)
        {
            if (url.StartsWith("https://", StringComparison.OrdinalIgnoreCase))
                return url;

            var request = System.Web.HttpContext.Current.Request;

            return $"{request.Url.Scheme}://{request.Url.Host}/api/contents/proxy?url={HttpUtility.UrlEncode(url)}";
        }

        #endregion
    }
}