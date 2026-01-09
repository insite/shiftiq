using InSite.Common.Web.UI;

using Shift.Common;
using Shift.Sdk.UI;

namespace InSite.UI.Layout.Common.Controls
{
    public partial class CourseSummary : BlockControl, IBlockControl
    {
        private static readonly string[] _contentLabels = new[] { "Title", "Description", "Time Required", "Start URL" };

        public override string[] GetContentLabels() => _contentLabels;

        public override void BindContent(ContentContainer block, string hook = null)
        {
            BlockTitle.InnerText = GetText(block, "Title");
            BlockDescription.InnerHtml = GetHtml(block, "Description");
            BlockTimeRequired.Text = GetText(block, "Time Required");
            BlockStartUrl.NavigateUrl = GetText(block, "Start URL");

            if (BlockStartUrl.NavigateUrl != null && BlockStartUrl.NavigateUrl.StartsWith("/ui/portal/integrations/scorm/consumer"))
                BlockStartUrl.NavigateTarget = "_blank";
        }

        private static readonly BlockContentControlTypeInfo _blockInfo =
            new BlockContentControlTypeInfo(4, typeof(CourseSummary), "Course Summary", "~/UI/Layout/Common/Controls/CourseSummary.ascx", _contentLabels);
        public static BlockContentControlTypeInfo GetBlockInfo() => _blockInfo;
    }
}