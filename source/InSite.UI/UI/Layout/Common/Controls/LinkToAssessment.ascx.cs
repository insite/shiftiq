using InSite.Common.Web.UI;
using InSite.Persistence;

using Shift.Common;
using Shift.Sdk.UI;

namespace InSite.UI.Layout.Common.Controls
{
    public partial class LinkToAssessment : BlockControl, IBlockControl
    {
        private static readonly string[] _contentLabels = new[] { "Link Target" };

        public override string[] GetContentLabels() => _contentLabels;

        public override void BindContent(ContentContainer block, string hook = null)
        {
            if (hook.IsEmpty())
                return;

            var target = GetText(block, "Link Target");

            BindAssessment(hook, target);
            BindProgress(hook);
        }

        private void BindAssessment(string hook, string target)
        {
            BlockStartUrl.Disabled = true;
            BlockStartUrl.Visible = false;

            if (hook.IsEmpty())
            {
                BlockTitle.Attributes["class"] = "text-danger";
                BlockTitle.InnerText = Translate("Error: Missing Assessment Hook");
                BlockDescription.InnerHtml = Markdown.ToHtml(Translate("The integration hook for this ") + "**Link to Assessment** " + Translate("content block has not been input by the author of this web page."));
            }
            else
            {
                var course = ServiceLocator.PageSearch.BindFirst(x => x,
                    x => x.OrganizationIdentifier == Organization.Identifier && x.Hook == hook && x.PageType != "Block");

                if (course != null)
                {
                    var content = ServiceLocator.ContentSearch.GetBlock(course.PageIdentifier);
                    BlockTitle.InnerText = GetText(content, "Title");
                    BlockDescription.InnerHtml = GetHtml(content, "Summary");

                    // if (course.DurationMinutes.HasValue)
                    //    BlockTimeRequired.Text = Translate("Time Limit: ")
                    //        + TimeSpan.FromMinutes(course.DurationMinutes.Value).Humanize(precision: 2);

                    if (course.NavigateUrl != null)
                    {
                        BlockStartUrl.HRef = course.NavigateUrl;
                        BlockStartUrl.Disabled = false;
                        BlockStartUrl.Visible = true;

                        if (target != null)
                            BlockStartUrl.Target = target;
                    }
                }
                else
                {
                    BlockTitle.Attributes["class"] = "text-danger";
                    BlockTitle.InnerText = $"{Translate("Error: Assessment Not Found")}: {hook}";
                    BlockDescription.InnerText = Translate("This assessment is not yet added to your library, or there is a typo in the assessment hook.");
                }
            }
        }

        private void BindProgress(string hook)
        {
            var item = ServiceLocator.RecordSearch.GetGradeItemByHook(hook);
            if (item == null)
                return;

            var progress = ServiceLocator.RecordSearch
                .GetProgress(item.GradebookIdentifier, item.GradeItemIdentifier, User.UserIdentifier);
            if (progress == null)
                return;

            if (progress.ProgressGraded.HasValue)
                BlockTimeRequired.Text = Translate("Completed ") + TimeZones.FormatDateOnly(progress.ProgressGraded.Value, User.TimeZone);

            PassedLabel.Visible = progress.ProgressPercent >= 0.9m;
            FailedLabel.Visible = progress.ProgressPercent < 0.9m;

            BlockStartUrl.Attributes["class"] = "btn btn-sm btn-primary disabled";
        }

        private static readonly BlockContentControlTypeInfo _blockInfo =
            new BlockContentControlTypeInfo(3, typeof(LinkToAssessment), "Link to Assessment", "~/UI/Layout/Common/Controls/LinkToAssessment.ascx", _contentLabels);
        public static BlockContentControlTypeInfo GetBlockInfo() => _blockInfo;
    }
}