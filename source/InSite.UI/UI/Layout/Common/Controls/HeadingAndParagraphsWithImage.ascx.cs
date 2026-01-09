using InSite.Common.Web.UI;

using Shift.Common;
using Shift.Sdk.UI;

namespace InSite.UI.Layout.Common.Controls
{
    public partial class HeadingAndParagraphsWithImage : BlockControl, IBlockControl
    {
        private static readonly string[] _contentLabels = new[] { "Heading", "Paragraphs", "Image URL" };

        public override string[] GetContentLabels() => _contentLabels;

        public override void BindContent(ContentContainer block, string hook = null)
        {
            BlockHeading.InnerText = GetText(block, "Heading", CurrentLanguage);
            BlockParagraphs.Text = GetHtml(block, "Paragraphs", CurrentLanguage);
            BlockImage.Alt = GetText(block, "Image URL:Alt");
            BlockImage.Src = GetText(block, "Image URL:Url");
            BlockImageCaption.InnerText = GetText(block, "Image URL:Alt");
        }

        private static readonly BlockContentControlTypeInfo _blockInfo =
            new BlockContentControlTypeInfo(1, typeof(HeadingAndParagraphsWithImage), "Heading and Paragraphs with Image", "~/UI/Layout/Common/Controls/HeadingAndParagraphsWithImage.ascx", _contentLabels);
        public static BlockContentControlTypeInfo GetBlockInfo() => _blockInfo;
    }
}