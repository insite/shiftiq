using InSite.Common.Web.UI;

using Shift.Common;
using Shift.Sdk.UI;

namespace InSite.UI.Layout.Common.Controls
{
    public partial class TwoColumns : BlockControl, IBlockControl
    {
        private static readonly string[] _contentLabels = new[] { "Column 1", "Column 2" };

        public override string[] GetContentLabels() => _contentLabels;

        public override void BindContent(ContentContainer block, string hook = null)
        {
            Column1.Text = GetHtml(block, "Column 1");
            Column2.Text = GetHtml(block, "Column 2");
        }

        private static readonly BlockContentControlTypeInfo _blockInfo =
            new BlockContentControlTypeInfo(2, typeof(TwoColumns), "Two Columns", "~/UI/Layout/Common/Controls/TwoColumns.ascx", _contentLabels);

        public static BlockContentControlTypeInfo GetBlockInfo() => _blockInfo;
    }
}