using System.Text;

using Markdown = Shift.Common.Markdown;

namespace Shift.Test.Common
{
    public class MarkdownTests
    {
        private const string RenderingError = "Rendering error";

        #region Escape

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        public void Escape_NullOrEmpty_ReturnsInput(string? input)
        {
            Assert.Equal(input, Markdown.Escape(input));
        }

        [Fact]
        public void Escape_LeadingDash_IsEscaped()
        {
            Assert.Equal("\\- Rajesh", Markdown.Escape("- Rajesh"));
        }

        [Fact]
        public void Escape_DashWithinText_IsNotEscaped()
        {
            Assert.Equal("Jean-Luc", Markdown.Escape("Jean-Luc"));
        }

        [Fact]
        public void Escape_LeadingHash_IsEscaped()
        {
            Assert.Equal("\\#1 Smith", Markdown.Escape("#1 Smith"));
        }

        [Fact]
        public void Escape_LeadingOrderedListMarker_IsEscaped()
        {
            Assert.Equal("1\\. Smith", Markdown.Escape("1. Smith"));
            Assert.Equal("12\\) Smith", Markdown.Escape("12) Smith"));
        }

        [Fact]
        public void Escape_DecimalWithinText_IsNotEscaped()
        {
            Assert.Equal("Level 1.2", Markdown.Escape("Level 1.2"));
        }

        [Fact]
        public void Escape_InlineMarkup_IsEscaped()
        {
            Assert.Equal("A\\|B", Markdown.Escape("A|B"));
            Assert.Equal("\\*Bob\\*", Markdown.Escape("*Bob*"));
            Assert.Equal("John\\_Doe", Markdown.Escape("John_Doe"));
            Assert.Equal("\\<b\\>x\\</b\\>", Markdown.Escape("<b>x</b>"));
            Assert.Equal("Smith \\& Jones", Markdown.Escape("Smith & Jones"));
        }

        [Fact]
        public void Escape_PlainText_IsUnchanged()
        {
            Assert.Equal("O'Brien", Markdown.Escape("O'Brien"));
            Assert.Equal("Zoë Doe", Markdown.Escape("Zoë Doe"));
        }

        #endregion

        #region EscapeTableCell

        [Fact]
        public void EscapeTableCell_LineBreaks_AreFlattened()
        {
            Assert.Equal("Jane Roe", Markdown.EscapeTableCell("  Jane\r\nRoe  "));
        }

        [Fact]
        public void EscapeTableCell_LeadingWhitespaceBeforeDash_StillEscapesDash()
        {
            Assert.Equal("\\- Rajesh", Markdown.EscapeTableCell("   - Rajesh"));
        }

        #endregion

        #region ToHtml

        /// <summary>
        /// TEC-1144. A candidate name beginning with a dash used to be parsed as a list bullet, which ended the
        /// table after its header row and swallowed every remaining row into a single list item. With enough
        /// rows that tripped Markdig's nesting depth guard and the whole message body was replaced with the
        /// "Rendering error" banner produced by <see cref="Markdown.ToHtml"/>.
        /// </summary>
        [Theory]
        [InlineData("- Rajesh")]
        [InlineData("-")]
        [InlineData("1. Smith")]
        [InlineData("A|B Smith")]
        [InlineData("*Bob*")]
        [InlineData("<b>x</b>")]
        public void ToHtml_EscapedCandidateNames_RenderAsTableRows(string firstName)
        {
            const int rows = 30;

            var html = Markdown.ToHtml(BuildMessageBody(BuildCandidateRegistrationTable(firstName, rows)));

            Assert.DoesNotContain(RenderingError, html);
            Assert.Contains("<table class=\"table-markdown\">", html);
            Assert.Equal(rows, CountBodyRows(html));
        }

        private static string BuildCandidateRegistrationTable(string firstName, int rows)
        {
            var md = new StringBuilder();
            md.AppendLine("Name | Code | Exam | Status | Accommodation | Materials");
            md.AppendLine(":-- |:-- |:-- |:-- |:-- |:--");

            for (var i = 0; i < rows; i++)
            {
                var name = i == 0 ? firstName : $"Candidate {i}";
                md.AppendLine($"{Markdown.EscapeTableCell(name)} | 12345 | ABCD | Eligible | None, Time Limit: 3.0 h | None");
            }

            return md.ToString();
        }

        private static string BuildMessageBody(string table)
        {
            var md = new StringBuilder();
            md.AppendLine("## Agricultural Something Something ABCD");
            md.AppendLine();
            md.AppendLine("The following session has been scheduled.");
            md.AppendLine();
            md.AppendLine(table);
            md.AppendLine();
            md.AppendLine("Please confirm the details above.");

            return md.ToString();
        }

        private static int CountBodyRows(string html)
        {
            var index = html.IndexOf("<tbody>", System.StringComparison.Ordinal);
            if (index < 0)
                return 0;

            var count = 0;
            while ((index = html.IndexOf("<tr>", index, System.StringComparison.Ordinal)) >= 0)
            {
                count++;
                index += 4;
            }

            return count;
        }

        #endregion
    }
}
