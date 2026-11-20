using System.Linq;

using Shift.Common;
using Shift.Constant;

using Xunit;

namespace Shift.Test.Common
{
    public class ContentContainerFactoryTests
    {
        [Fact]
        public void FromMarkdown_NullSource_ReturnsEmptyLoadedContainer()
        {
            var container = ContentContainer.Create(null);

            Assert.NotNull(container);
            Assert.True(container.IsLoaded);
            Assert.Empty(container.GetLabels());
        }

        [Fact]
        public void FromMarkdown_AlwaysSetsIsLoadedTrue()
        {
            var dict = new ContentExamQuestion();
            dict.Title["en"] = "Question title";

            var container = ContentContainer.Create(dict);

            Assert.True(container.IsLoaded);
        }

        [Fact]
        public void FromMarkdown_MarkdownValue_SetsTextAndLeavesHtmlNullAndGeneratesMarkdownSnip()
        {
            var dict = new ContentTitle();
            dict.Title["en"] = "## Header\n**Bold text** with [link](http://example.com)";

            var container = ContentContainer.Create(dict);
            container.CreateSnips();

            var item = container[ContentLabel.Title];
            Assert.Equal("## Header\n**Bold text** with [link](http://example.com)", item.Text["en"]);
            Assert.True(string.IsNullOrEmpty(item.Html["en"]));
            Assert.Equal(" Header Bold text with link", item.Snip["en"]);
        }

        [Fact]
        public void FromMarkdown_DropsInvalidOrExtendedLanguageCodes()
        {
            var dict = new MultilingualDictionary();
            dict["Title"] = new MultilingualString();
            dict["Title"]["en"] = "English";
            dict["Title"]["fr"] = "French";
            dict["Title"]["en:8f3d1e2a-4b5c-6d7e-8f9a-0b1c2d3e4f5a"] = "Extended key";
            dict["Title"]["eng"] = "Three letter code";
            dict["Title"]["xyz"] = "Invalid code";

            var container = ContentContainer.Create(dict);

            var item = container["Title"];
            Assert.Equal("English", item.Text["en"]);
            Assert.Equal("French", item.Text["fr"]);
            Assert.False(item.Text.Exists("en:8f3d1e2a-4b5c-6d7e-8f9a-0b1c2d3e4f5a"));
            Assert.False(item.Text.Exists("eng"));
            Assert.False(item.Text.Exists("xyz"));
        }

        [Fact]
        public void FromMarkdown_DropsWhitespaceOrEmptyValues()
        {
            var dict = new ContentExamQuestion();
            dict.Title["en"] = "   ";
            dict.Title["fr"] = "";
            dict.Rationale["en"] = null;

            var container = ContentContainer.Create(dict);

            Assert.Empty(container.GetLabels());
        }

        [Fact]
        public void FromMarkdown_ContentExamQuestionWithMultilingualLabels_MapsAllLabelsAndLanguages()
        {
            var question = new ContentExamQuestion();
            question.Title["en"] = "What is 2+2?";
            question.Title["fr"] = "Combien font 2+2?";
            question.Rationale["en"] = "Basic arithmetic";
            question.Rationale["fr"] = "Arithmétique de base";
            question.Exemplar["en"] = "4";
            question.Exemplar["fr"] = "4";

            var container = ContentContainer.Create(question);

            Assert.True(container.IsLoaded);
            var labels = container.GetLabels().OrderBy(x => x).ToArray();
            Assert.Equal(new[] { ContentLabel.Exemplar, ContentLabel.Rationale, ContentLabel.Title }, labels);

            Assert.Equal("What is 2+2?", container.GetText(ContentLabel.Title, "en"));
            Assert.Equal("Combien font 2+2?", container.GetText(ContentLabel.Title, "fr"));
            Assert.Equal("Basic arithmetic", container.GetText(ContentLabel.Rationale, "en"));
            Assert.Equal("Arithmétique de base", container.GetText(ContentLabel.Rationale, "fr"));
            Assert.Equal("4", container.GetText(ContentLabel.Exemplar, "en"));
            Assert.Equal("4", container.GetText(ContentLabel.Exemplar, "fr"));
        }
    }
}
