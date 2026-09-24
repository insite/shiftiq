using System;
using System.Text;
using System.Text.RegularExpressions;

using Markdig;

namespace Shift.Common
{
    public static class Markdown
    {
        private const string EquationTag = "[eq]";

        public static string ToHtml(string markdown, bool removeSingleParagraph = false)
        {
            if (string.IsNullOrEmpty(markdown))
                return string.Empty;

            markdown = ProcessEquations(markdown);

            var pipeline = GetDefaultPipeline();

            string html;

            try
            {
                html = Markdig.Markdown.ToHtml(markdown, pipeline);
            }
            catch (ArgumentException ex) when (ex.Message.StartsWith("Markdown elements in the input are too deeply nested - depth limit exceeded."))
            {
                return @"
<div class=""alert alert-danger d-flex align-items-start"" role=""alert"">
    <i class=""fas fa-stop-circle me-2 mt-1""></i>
    <div>
        <strong>Rendering error</strong><br>
        The content is too complex to render.
        This usually happens with large tables containing many inline formatting elements. 
        Please simplify the content and try again.
    </div>
</div>";
            }

            html = html.Replace("<table>", "<table class=\"table-markdown\">");

            if (removeSingleParagraph)
                html = RemoveSingleParagraph(html);

            return html;
        }

        private static string RemoveSingleParagraph(string html)
        {
            var result = html.Trim();
            if (!result.StartsWith("<p>") || !result.EndsWith("</p>"))
                return html;

            result = result.Substring(3, result.Length - 7);

            return !result.Contains("<p>") ? result : html;
        }

        private static readonly Regex MarkdownImageAndLinkPattern = new Regex("!?\\[[^\\]]*]\\([^\\)]*\\)", RegexOptions.IgnoreCase | RegexOptions.Compiled);

        public static string ToText(string markdown)
        {
            if (string.IsNullOrEmpty(markdown))
                return string.Empty;

            markdown = MarkdownImageAndLinkPattern.Replace(markdown, string.Empty);

            markdown = markdown.Replace("\r", string.Empty).Replace("&nbsp;", " ");
            while (true)
            {
                var length = markdown.Length;
                markdown = markdown.Replace("\n\n", "\n");

                if (markdown.Length == length)
                    break;
            }

            return markdown.Trim().Replace("\n", System.Environment.NewLine);
        }

        public static MarkdownPipeline GetDefaultPipeline()
        {
            return new MarkdownPipelineBuilder()
                .UseEmphasisExtras()
                .UsePipeTables()
                .UseSoftlineBreakAsHardlineBreak()
                .UseTaskLists()
                .Build();
        }

        private static string ProcessEquations(string preMarkdown)
        {
            var markdown = new StringBuilder();

            var i = 0;
            while (i < preMarkdown.Length)
            {
                var c = preMarkdown[i];
                if (c != '[')
                {
                    markdown.Append(c);
                    i++;
                }
                else
                {
                    string s;
                    (s, i) = ParseEquation(i, preMarkdown);

                    markdown.Append(s);
                }
            }

            return markdown.ToString();
        }

        private static (string, int) ParseEquation(int index, string preMarkdown)
        {
            if (preMarkdown.Length <= index + 2 * EquationTag.Length
                || !IsEquationTag(index, preMarkdown)
                )
            {
                return ("[", index + 1);
            }

            var start = index + EquationTag.Length;
            var i = start;
            while (i < preMarkdown.Length && preMarkdown[i] != '[')
                i++;

            if (preMarkdown.Length <= i + EquationTag.Length - 1
                || !IsEquationTag(index, preMarkdown)
                )
            {
                return ("[", index + 1);
            }

            var latex = preMarkdown.Substring(start, i - start);
            var html = $"<span class=math-eq>{latex}</span>";

            return (html, i + EquationTag.Length);
        }

        private static bool IsEquationTag(int index, string preMarkdown)
        {
            for (int i = 1; i < EquationTag.Length; i++)
            {
                if (preMarkdown[index + i] != EquationTag[i])
                    return false;
            }

            return true;
        }

        #region Escape methods

        private static readonly bool[] InlineMarkupTable = CreateCharTable("\\`*_+~^=[]<>|&");

        private enum LineState { Indent, Digits, Text }

        public static string Escape(string text)
        {
            if (text.IsEmpty())
                return text;

            StringBuilder sb = null;
            var copied = 0;
            var state = LineState.Indent;

            for (var i = 0; i < text.Length; i++)
            {
                var c = text[i];
                var escape = IsInlineMarkup(c);

                switch (state)
                {
                    case LineState.Indent when c == ' ' || c == '\t':
                        break;

                    case LineState.Indent:
                        if (c == '-' || c == '#')
                            escape = true;
                        state = IsAsciiDigit(c) ? LineState.Digits : LineState.Text;
                        break;

                    case LineState.Digits when IsAsciiDigit(c):
                        break;

                    case LineState.Digits:
                        if (c == '.' || c == ')')
                            escape = true;
                        state = LineState.Text;
                        break;
                }

                if (c == '\n' || c == '\r')
                    state = LineState.Indent;

                if (!escape)
                    continue;

                if (sb == null)
                    sb = new StringBuilder(text.Length + 16);

                sb.Append(text, copied, i - copied).Append('\\');
                copied = i;
            }

            if (sb == null)
                return text;

            return sb.Append(text, copied, text.Length - copied).ToString();
        }

        public static string EscapeTableCell(string text)
        {
            if (text.IsEmpty())
                return text;

            var value = text
                .Replace("\r\n", " ")
                .Replace('\r', ' ')
                .Replace('\n', ' ')
                .Trim();

            return Escape(value);
        }

        private static bool IsInlineMarkup(char c) => c < InlineMarkupTable.Length && InlineMarkupTable[c];

        private static bool IsAsciiDigit(char c) => c >= '0' && c <= '9';

        private static bool[] CreateCharTable(string chars)
        {
            var table = new bool[128];
            foreach (var ch in chars)
                table[ch] = true;
            return table;
        }

        #endregion
    }
}
