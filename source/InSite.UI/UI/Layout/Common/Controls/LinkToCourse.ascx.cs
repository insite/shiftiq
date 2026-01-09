using System;

using InSite.Common.Web.UI;
using InSite.Persistence;
using InSite.UI.Portal.Learning.Models;

using Shift.Common;
using Shift.Sdk.UI;

namespace InSite.UI.Layout.Common.Controls
{
    public partial class LinkToCourse : BlockControl, IBlockControl
    {
        private static readonly string[] _contentLabels = new[] { "Link Target" };

        public override string[] GetContentLabels() => _contentLabels;

        public override void BindContent(ContentContainer block, string hook = null)
        {
            if (hook.IsEmpty())
                return;

            var target = GetText(block, "Link Target");
            hook = hook.Replace("$Language", CurrentLanguage);

            BindCourse(hook, target);
            BindProgress(hook);
        }

        private void BindCourse(string hook, string target)
        {
            var translator = ((Portal.PortalBasePage)Page).Translator;

            BlockStartUrl.Disabled = true;
            BlockStartUrl.Visible = false;

            if (hook.IsEmpty())
            {
                BlockTitle.Attributes["class"] = "text-danger";
                BlockTitle.InnerText = Translate("Error: Missing Course Hook");
                BlockDescription.InnerHtml = Markdown.ToHtml(Translate("The integration hook for this ") + "**Link to Course** " + Translate("content block has not been input by the author of this web page."));
            }
            else
            {
                var courseId = CourseSearch.BindCourseFirst(x => (Guid?)x.CourseIdentifier, x => x.OrganizationIdentifier == Organization.Identifier && x.CourseHook == hook);

                if (courseId.HasValue)
                {
                    var content = ServiceLocator.ContentSearch.GetBlock(courseId.Value);
                    BlockTitle.InnerText = GetText(content, "Title", CurrentLanguage);
                    BlockDescription.InnerHtml = GetHtml(content, "Summary", CurrentLanguage);

                    var pages = ServiceLocator.PageSearch.GetCourseWebPages(courseId.Value);
                    if (pages.Count > 0)
                        BlockStartUrl.HRef = pages[0].Item1;
                    else
                        BlockStartUrl.HRef = ProgressState.GetPreviewUrl(courseId.Value);

                    if (StringHelper.Equals(target, "launch-frame"))
                        BlockStartUrl.Attributes["onclick"] = "return showLaunchFrame();";

                    BlockStartUrl.Disabled = false;
                    BlockStartUrl.Visible = true;

                    if (target != null)
                        BlockStartUrl.Target = target;
                }
                else
                {
                    BlockTitle.Attributes["class"] = "text-danger";
                    BlockTitle.InnerText = $"{Translate("Error: Course Not Found")}: {hook}";
                    BlockDescription.InnerText = Translate("This course is not yet added to your library, or there is a typo in the course hook.");
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

            LaunchedLabel.Visible = progress.ProgressText == "Launched";
            CompletedLabel.Visible = progress.ProgressText == "Completed";
        }

        private static readonly BlockContentControlTypeInfo _blockInfo =
            new BlockContentControlTypeInfo(3, typeof(LinkToCourse), "Link to Course", "~/UI/Layout/Common/Controls/LinkToCourse.ascx", _contentLabels);

        public static BlockContentControlTypeInfo GetBlockInfo() => _blockInfo;
    }
}