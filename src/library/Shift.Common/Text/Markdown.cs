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

            var html = Markdig.Markdown.ToHtml(markdown, pipeline);

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
    }
}
