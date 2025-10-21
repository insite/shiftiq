using System;

using Humanizer;

using InSite.Common.Web.UI;

using Shift.Common;
using Shift.Sdk.UI;

namespace InSite.UI.Layout.Common.Controls
{
    public partial class LinkToSurvey : BlockControl, IBlockControl
    {
        private static readonly string[] _contentLabels = new[] { "Link Target" };

        public override string[] GetContentLabels() => _contentLabels;

        public override void BindContent(ContentContainer block, string hook = null)
        {
            var target = GetText(block, "Link Target");

            BindSurvey(hook, target);
            BindProgress(hook);
        }

        private void BindSurvey(string hook, string target)
        {
            BlockStartUrl.Disabled = true;
            BlockStartUrl.Visible = false;

            if (hook.IsEmpty())
            {
                BlockTitle.Attributes["class"] = "text-danger";
                BlockTitle.InnerText = Translate("Error: Missing Survey Hook");
                BlockDescription.InnerHtml = Markdown.ToHtml(Translate("The integration hook for this ") + "**Link to Survey** " + Translate("content block has not been input by the author of this web page."));
            }
            else
            {
                var survey = ServiceLocator.SurveySearch.GetSurveyFormByHook(Organization.Identifier, hook);

                if (survey != null)
                {
                    var content = ServiceLocator.ContentSearch.GetBlock(survey.SurveyFormIdentifier);
                    BlockTitle.InnerText = GetText(content, "Title", CurrentLanguage);
                    BlockDescription.InnerHtml = GetHtml(content, "Page Header", CurrentLanguage);

                    if (survey.SurveyFormDurationMinutes.HasValue)
                        BlockTimeRequired.Text = Translate("Time Required") + ": "
                            + TimeSpan.FromMinutes(survey.SurveyFormDurationMinutes.Value).Humanize(precision: 2);

                    BlockStartUrl.HRef = $"/survey/{survey.AssetNumber}/{User.UserIdentifier}";
                    BlockStartUrl.Disabled = false;
                    BlockStartUrl.Visible = true;

                    if (target != null)
                        BlockStartUrl.Target = target;
                }
                else
                {
                    BlockTitle.Attributes["class"] = "text-danger";
                    BlockTitle.InnerText = $"{Translate("Survey Not Found")}: {hook}";
                    BlockDescription.InnerText = Translate("This survey is not yet added to your library, or there is a typo in the survey code.");
                }
            }
        }

        private void BindProgress(string hook)
        {
            if (hook.IsEmpty())
                return;

            var item = ServiceLocator.RecordSearch.GetGradeItemByHook(hook);
            if (item == null)
                return;

            var progress = ServiceLocator.RecordSearch
                .GetProgress(item.GradebookIdentifier, item.GradeItemIdentifier, User.UserIdentifier);

            if (progress == null)
                return;

            StartedLabel.Visible = 0 <= progress.ProgressPercent && progress.ProgressPercent < 1;
            CompletedLabel.Visible = progress.ProgressPercent == 1;
        }

        private static readonly BlockContentControlTypeInfo _blockInfo =
            new BlockContentControlTypeInfo(3, typeof(LinkToSurvey), "Link to Survey", "~/UI/Layout/Common/Controls/LinkToSurvey.ascx", _contentLabels);
        public static BlockContentControlTypeInfo GetBlockInfo() => _blockInfo;
    }
}