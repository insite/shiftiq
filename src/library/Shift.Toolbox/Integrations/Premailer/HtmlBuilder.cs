using System;
using System.Collections.Generic;
using System.Text;

using Shift.Common;
using Shift.Common.Integration.Premailer;

namespace Shift.Toolbox
{
    public class HtmlBuilder
    {
        #region Properties

        public string Title { get; set; }
        public StringBuilder Body { get; set; }
        public IEnumerable<string> Styles => _styles;
        public IEnumerable<string> Scripts => _scripts;

        #endregion

        #region Fields

        private static ApiSettings _premailerApiSettings;

        private List<string> _styles;
        private List<string> _scripts;

        #endregion

        #region Construction

        public HtmlBuilder(string title, string body)
        {
            _styles = new List<string>();
            _scripts = new List<string>();

            Title = title;
            Body = new StringBuilder();
            if (!string.IsNullOrEmpty(body))
                Body.Append(body);
        }

        public static void Initialize(ApiSettings premailerApiSettings)
        {
            _premailerApiSettings = premailerApiSettings;
        }

        #endregion

        #region Methods

        public bool AddCss(string content)
        {
            var hasContent = !string.IsNullOrEmpty(content);

            if (hasContent)
                _styles.Add(content);

            return hasContent;
        }

        public bool AddCssFile(string path)
            => AddCss(GetFileContent(path));

        public bool AddJavaScript(string content)
        {
            var hasContent = !string.IsNullOrEmpty(content);

            if (hasContent)
                _scripts.Add(content);

            return hasContent;
        }

        public string ToHtml(bool moveCssInline = false)
        {
            var sb = new StringBuilder();

            sb.Append("<!DOCTYPE html>");
            sb.Append("<html>");

            sb.Append("<head>");
            sb.Append("<meta charset='utf-8'>");

            sb.Append("<title>")
              .Append(Title)
              .Append("</title>");

            foreach (var style in _styles)
                WriteStyle(sb, style);

            foreach (var script in _scripts)
                WriteScript(sb, script);

            sb.Append("</head>");

            sb.Append("<body>")
              .Append(Body)
              .Append("</body>");

            sb.Append("</html>");

            var html = sb.ToString();

            if (moveCssInline)
                html = MoveCssInline(html);

            return html;
        }

        public static string MoveCssInline(string html)
        {
            if (_premailerApiSettings == null)
            {
                var error = "Your code must call the Initialize method on this class to configure"
                    + " the PreMailer API before calling the MoveCssInline function.";

                throw new InvalidOperationException(error);
            }

            var client = new PremailerClient(_premailerApiSettings);

            return client.MoveCssInline(html);
        }

        #endregion

        #region Helper methods

        private static string GetFileContent(string physicalPath)
        {
            if (string.IsNullOrEmpty(physicalPath))
                throw new ArgumentNullException(nameof(physicalPath));

            return System.IO.File.Exists(physicalPath) ? System.IO.File.ReadAllText(physicalPath) : null;
        }

        private static void WriteStyle(StringBuilder sb, string content)
        {
            if (!string.IsNullOrEmpty(content))
                sb.Append("<style type='text/css'>")
                  .Append(content)
                  .Append("</style>");
        }

        private static void WriteScript(StringBuilder sb, string content)
        {
            if (!string.IsNullOrEmpty(content))
                sb.Append("<script type='text/javascript'>")
                  .Append(content)
                  .Append("</script>");
        }

        #endregion
    }
}