using InSite.Common.Web.UI;

using Shift.Common;
using Shift.Sdk.UI;

namespace InSite.UI.Layout.Common.Controls
{
    public partial class HeadingAndParagraphs : BlockControl, IBlockControl
    {
        private static readonly string[] _contentLabels = new[] { "Heading", "Paragraphs" };

        public override string[] GetContentLabels() => _contentLabels;

        public override void BindContent(ContentContainer block, string hook = null)
        {
            BlockHeading.InnerText = GetText(block, "Heading", CurrentLanguage);
            BlockParagraphs.Text = GetHtml(block, "Paragraphs", CurrentLanguage);
        }

        private static readonly BlockContentControlTypeInfo _blockInfo =
            new BlockContentControlTypeInfo(1, typeof(HeadingAndParagraphs), "Heading and Paragraphs", "~/UI/Layout/Common/Controls/HeadingAndParagraphs.ascx", _contentLabels);
        public static BlockContentControlTypeInfo GetBlockInfo() => _blockInfo;
    }
}