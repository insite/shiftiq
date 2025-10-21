using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Web;
using System.Web.UI;

using Shift.Common;
using Shift.Constant;

namespace InSite.Web.Helpers
{
    public class HtmlConverterSettings
    {
        #region Enums


        #endregion

        #region Classes

        public class Cookie
        {
            public string Name { get; }
            public string Value { get; }

            public Cookie(HttpCookie cookie)
                : this(HttpUtility.UrlDecode(cookie.Name), HttpUtility.UrlDecode(cookie.Value))
            {

            }

            public Cookie(string name, string value)
            {
                Name = name;
                Value = value;
            }
        }

        public class Variable
        {
            public string Name { get; }
            public string Value { get; }

            public Variable(string name, string value)
            {
                Name = name;
                Value = value;
            }
        }

        public class ViewportSize
        {
            public int Width { get; set; }
            public int Height { get; set; }

            public ViewportSize(int width)
            {
                Width = width;
            }

            public ViewportSize(int width, int height)
                : this(width)
            {
                Height = height;
            }
        }

        #endregion

        #region Properties

        public string ExePath { get; set; }

        public string WorkingDirectory { get; set; }

        /// <summary>
        /// Web cache directory
        /// </summary>
        public string WebCacheDirectory { get; set; }

        /// <summary>
        /// The title of the generated pdf file (The title of the first document is used if not specified)
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// PDF will be generated in grayscale
        /// </summary>
        public bool RenderGrayscale { get; set; }

        /// <summary>
        /// Generates lower quality pdf/ps. Useful to shrink the result document space
        /// </summary>
        public bool RenderLowQuality { get; set; }

        /// <summary>
        /// Enable the intelligent shrinking strategy used by WebKit that makes the pixel/dpi
        /// </summary>
        public bool EnableSmartShrinking { get; set; }

        /// <summary>
        /// Do allow web pages to run javascript
        /// </summary>
        public bool EnableJavaScript { get; set; }

        /// <summary>
        /// Use print media-type instead of screen
        /// </summary>
        public bool EnablePrintMediaType { get; set; }

        /// <summary>
        /// Make links to remote web pages
        /// </summary>
        public bool EnableExternalLinks { get; set; }

        /// <summary>
        /// Make local links
        /// </summary>
        public bool EnableInternalLinks { get; set; }

        /// <summary>
        /// Wait some milliseconds for javascript finish
        /// </summary>
        public int? JavaScriptDelay { get; set; }

        /// <summary>
        /// Change the dpi explicitly (this has no effect on X11 based systems)
        /// </summary>
        public int? Dpi { get; set; }

        /// <summary>
        /// When embedding images scale them down to this dpi (default 600)
        /// </summary>
        public int? ImageDpi { get; set; }

        /// <summary>
        /// When jpeg compressing images use this quality (default 94)
        /// </summary>
        public int? ImageQuality { get; set; }

        /// <summary>
        /// Do not use lossless compression on pdf objects
        /// </summary>
        public bool DisablePdfCompression { get; set; }

        /// <summary>
        /// Set orientation to Landscape or Portrait
        /// </summary>
        public PageOrientationType PageOrientation { get; set; }

        /// <summary>
        /// Set paper size to: A4, Letter, etc.
        /// </summary>
        public PageSizeType PageSize { get; set; }

        /// <summary>
        /// Page width
        /// </summary>
        public float? PageWidth { get; set; }

        /// <summary>
        /// Page height
        /// </summary>
        public float? PageHeight { get; set; }

        /// <summary>
        /// Set the page bottom margin
        /// </summary>
        public float? MarginBottom { get; set; }

        /// <summary>
        /// Set the page left margin (default 10mm)
        /// </summary>
        public float? MarginLeft { get; set; }

        /// <summary>
        /// Set the page right margin (default 10mm)
        /// </summary>
        public float? MarginRight { get; set; }

        /// <summary>
        /// Set the page top margin
        /// </summary>
        public float? MarginTop { get; set; }

        /// <summary>
        /// Set viewport size if you have custom scrollbars or css attribute overflow to emulate window size
        /// </summary>
        public ViewportSize Viewport { get; set; }

        /// <summary>
        /// Use this zoom factor (default 1)
        /// </summary>
        public float Zoom { get; set; }

        /// <summary>
        /// Set an additional cookie
        /// </summary>
        public IEnumerable<Cookie> Cookies { get; set; }

        /// <summary>
        /// Replace [name] with value in header and footer
        /// </summary>
        public IEnumerable<Variable> Variables { get; set; }

        /// <summary>
        /// Adds a html header
        /// </summary>
        public string HeaderUrl { get; set; }

        /// <summary>
        /// Left aligned header text
        /// </summary>
        public string HeaderTextLeft { get; set; }

        /// <summary>
        /// Right aligned header text
        /// </summary>
        public string HeaderTextRight { get; set; }

        /// <summary>
        /// Centered header text
        /// </summary>
        public string HeaderTextCenter { get; set; }

        /// <summary>
        /// Set header font name (default Arial)
        /// </summary>
        public string HeaderFontName { get; set; }

        /// <summary>
        /// Set header font size (default 12)
        /// </summary>
        public int? HeaderFontSize { get; set; }

        /// <summary>
        /// Spacing between header and content in mm
        /// </summary>
        public float? HeaderSpacing { get; set; }

        /// <summary>
        /// Display line below the header
        /// </summary>
        public bool ShowHeaderLine { get; set; }

        /// <summary>
        /// Adds a html footer
        /// </summary>
        public string FooterUrl { get; set; }

        /// <summary>
        /// Left aligned footer text
        /// </summary>
        public string FooterTextLeft { get; set; }

        /// <summary>
        /// Right aligned footer text
        /// </summary>
        public string FooterTextRight { get; set; }

        /// <summary>
        /// Centered footer text
        /// </summary>
        public string FooterTextCenter { get; set; }

        /// <summary>
        /// Set footer font name (default Arial)
        /// </summary>
        public string FooterFontName { get; set; }

        /// <summary>
        /// Set footer font size (default 12)
        /// </summary>
        public int? FooterFontSize { get; set; }

        /// <summary>
        /// Spacing between footer and content in mm
        /// </summary>
        public float? FooterSpacing { get; set; }

        /// <summary>
        /// Display line below the footer
        /// </summary>
        public bool ShowFooterLine { get; set; }

        #endregion

        #region Construction

        public HtmlConverterSettings(string exePath)
        {
            ExePath = exePath;
            WorkingDirectory = Path.GetDirectoryName(exePath);
            WebCacheDirectory = WorkingDirectory + "\\WebCache";

            EnableSmartShrinking = true;
            EnableJavaScript = true;
            EnableExternalLinks = true;
            EnableInternalLinks = true;
            PageOrientation = PageOrientationType.Portrait;
            PageSize = PageSizeType.A4;
            MarginLeft = 6.35f;
            MarginRight = 6.35f;
            MarginTop = 12.7f;
            MarginBottom = 12.7f;
            Zoom = 1;
        }

        #endregion

        #region Overriden methods

        public override string ToString()
        {
            var sb = new StringBuilder();

            // GLOBAL OPTIONS

            if (!string.IsNullOrEmpty(Title))
                sb.AppendFormat(" --title \"{0}\"", Title);

            if (!string.IsNullOrEmpty(WebCacheDirectory))
                sb.AppendFormat(" --cache-dir \"{0}\"", WebCacheDirectory);

            if (RenderGrayscale)
                sb.Append(" --grayscale");

            if (RenderLowQuality)
                sb.Append(" --lowquality");

            if (Dpi.HasValue)
                sb.AppendFormat(" --dpi {0}", Dpi.Value);

            if (ImageDpi.HasValue)
                sb.AppendFormat(" --image-dpi {0}", ImageDpi.Value);

            if (ImageQuality.HasValue)
                sb.AppendFormat(" --image-quality {0}", ImageQuality.Value);

            if (DisablePdfCompression)
                sb.Append(" --no-pdf-compression");

            if (PageWidth.HasValue)
                sb.AppendFormat(" --page-width {0:n2}mm", PageWidth.Value);

            if (PageHeight.HasValue)
                sb.AppendFormat(" --page-height {0:n2}mm", PageHeight.Value);

            if (MarginBottom.HasValue)
                sb.AppendFormat(" --margin-bottom {0:n2}mm", MarginBottom.Value);

            if (MarginLeft.HasValue)
                sb.AppendFormat(" --margin-left {0:n2}mm", MarginLeft.Value);

            if (MarginRight.HasValue)
                sb.AppendFormat(" --margin-right {0:n2}mm", MarginRight.Value);

            if (MarginTop.HasValue)
                sb.AppendFormat(" --margin-top {0:n2}mm", MarginTop.Value);

            if (PageOrientation != PageOrientationType.Portrait)
                sb.AppendFormat(" --orientation {0}", PageOrientation.ToString());

            if (PageSize != PageSizeType.A4)
                sb.AppendFormat(" --page-size {0}", PageSize.ToString());

            // PAGE OPTIONS

            sb.Append(EnableSmartShrinking ? " --enable-smart-shrinking" : " --disable-smart-shrinking");

            sb.Append(EnableJavaScript ? " --enable-javascript" : " --disable-javascript");

            sb.Append(EnablePrintMediaType ? " --print-media-type" : " --no-print-media-type");

            sb.Append(EnableExternalLinks ? " --enable-external-links" : " --disable-external-links");

            sb.Append(EnableInternalLinks ? " --enable-internal-links" : " --disable-internal-links");

            if (JavaScriptDelay.HasValue)
                sb.AppendFormat(" --javascript-delay {0}", JavaScriptDelay.Value);

            if (Viewport != null && Viewport.Width > 0)
            {
                sb.AppendFormat(" --viewport-size {0}", Viewport.Width);

                if (Viewport.Height > 0)
                    sb.AppendFormat("x{0}", Viewport.Height);
            }

            if (Zoom != 1 && Zoom > 0)
                sb.AppendFormat(" --zoom {0}", Zoom);

            if (Cookies != null)
            {
                foreach (var cookie in Cookies)
                    sb.AppendFormat(" --cookie \"{0}\" \"{1}\"", HttpUtility.UrlEncode(cookie.Name), HttpUtility.UrlEncode(cookie.Value));
            }

            if (Variables != null)
            {
                foreach (var variable in Variables)
                    sb.AppendFormat(" --replace \"{0}\" \"{1}\"", variable.Name, variable.Value);
            }

            if (!string.IsNullOrEmpty(HeaderUrl))
                sb.AppendFormat(" --header-html {0}", HeaderUrl);

            if (!string.IsNullOrEmpty(HeaderTextLeft))
                sb.AppendFormat(" --header-left \"{0}\"", HeaderTextLeft);

            if (!string.IsNullOrEmpty(HeaderTextRight))
                sb.AppendFormat(" --header-right \"{0}\"", HeaderTextRight);

            if (!string.IsNullOrEmpty(HeaderTextCenter))
                sb.AppendFormat(" --header-center \"{0}\"", HeaderTextCenter);

            if (!string.IsNullOrEmpty(HeaderFontName))
                sb.AppendFormat(" --header-font-name \"{0}\"", HeaderFontName);

            if (HeaderFontSize.HasValue)
                sb.AppendFormat(" --header-font-size {0}", HeaderFontSize.Value);

            if (HeaderSpacing.HasValue)
                sb.AppendFormat(" --header-spacing {0}", HeaderSpacing.Value);

            if (ShowHeaderLine)
                sb.Append(" --header-line");

            if (!string.IsNullOrEmpty(FooterUrl))
                sb.AppendFormat(" --footer-html {0}", FooterUrl);

            if (!string.IsNullOrEmpty(FooterTextLeft))
                sb.AppendFormat(" --footer-left \"{0}\"", FooterTextLeft);

            if (!string.IsNullOrEmpty(FooterTextRight))
                sb.AppendFormat(" --footer-right \"{0}\"", FooterTextRight);

            if (!string.IsNullOrEmpty(FooterTextCenter))
                sb.AppendFormat(" --footer-center \"{0}\"", FooterTextCenter);

            if (!string.IsNullOrEmpty(FooterFontName))
                sb.AppendFormat(" --footer-font-name \"{0}\"", FooterFontName);

            if (FooterFontSize.HasValue)
                sb.AppendFormat(" --footer-font-size {0}", FooterFontSize.Value);

            if (FooterSpacing.HasValue)
                sb.AppendFormat(" --footer-spacing {0}", FooterSpacing.Value);

            if (ShowFooterLine)
                sb.Append(" --footer-line");

            return sb.ToString();
        }

        #endregion
    }

    public class HtmlConverterException : Exception
    {
        public HtmlConverterException()
        {

        }

        public HtmlConverterException(string message) : base(message)
        {

        }

        public HtmlConverterException(string message, Exception inner) : base(message, inner)
        {

        }
    }

    public static class HtmlConverter
    {
        public static void QueueHtmlToPdf(string fileName, string html, HtmlConverterSettings settings)
        {

        }

        public static byte[] HtmlToPdf(string html, HtmlConverterSettings settings)
        {
            if (string.IsNullOrEmpty(html))
                throw new ArgumentNullException(nameof(html));

            return Execute(settings, "- -", process =>
            {
                using (var writer = new StreamWriter(process.StandardInput.BaseStream, Encoding.UTF8))
                {
                    writer.AutoFlush = true;
                    writer.Write(html);
                }
            });
        }

        public static byte[] UrlToPdf(HtmlConverterSettings settings, params string[] url)
        {
            if (url.IsEmpty())
                throw new ArgumentNullException(nameof(url));

            return Execute(settings, "cover \"" + string.Join("\" cover \"", url) + "\" -");
        }

        private static byte[] Execute(HtmlConverterSettings settings, string args, Action<Process> setupProcess = null)
        {
            if (settings == null)
                throw new ArgumentNullException(nameof(settings));

            byte[] result;
            string error;

            var psi = new ProcessStartInfo
            {
                FileName = settings.ExePath,
                WorkingDirectory = settings.WorkingDirectory,
                UseShellExecute = false,
                CreateNoWindow = true,
                RedirectStandardInput = true,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                Arguments = "-q " + settings + " " + args,
            };

            try
            {
                using (var process = Process.Start(psi))
                {
                    if (process == null)
                        throw new HtmlConverterException("HTML to PDF conversion failed:\r\n    - Process failed to start");

                    setupProcess?.Invoke(process);

                    using (var ms = new MemoryStream())
                    {
                        using (var stdout = process.StandardOutput)
                            stdout.BaseStream.CopyTo(ms);

                        result = ms.ToArray();
                    }

                    error = process.StandardError.ReadToEnd();

                    process.WaitForExit(10000);
                }
            }
            catch (Exception ex)
            {
                throw new HtmlConverterException($"HTML to PDF conversion failed:\r\n    - Settings: {settings}", ex);
            }

            if (result.IsEmpty() && error.IsNotEmpty())
                throw new HtmlConverterException($"HTML to PDF conversion failed: {error}\r\n    - Settings: {settings}\r\n");

            return result;
        }

        #region Helper methods

        public static IEnumerable<HtmlConverterSettings.Cookie> GetCookies(Page page)
        {
            var cookies = new Dictionary<string, HtmlConverterSettings.Cookie>();

            for (var i = 0; i < page.Request.Cookies.Count; i++)
            {
                var cookie = new HtmlConverterSettings.Cookie(page.Request.Cookies[i]);
                if (!cookies.ContainsKey(cookie.Name))
                    cookies.Add(cookie.Name, cookie);
                else
                    cookies[cookie.Name] = cookie;
            }

            return cookies.Values;
        }

        #endregion
    }
}