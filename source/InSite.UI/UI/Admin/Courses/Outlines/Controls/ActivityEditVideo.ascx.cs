using System;

using InSite.Admin.Courses.Courses;
using InSite.Application.Courses.Read;

using Shift.Common;
using Shift.Constant;

namespace InSite.Admin.Courses.Outlines.Controls
{
    public partial class ActivityEditVideo : BaseActivityEdit
    {
        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);
            BindControlsToHandlers(ActivitySetup, Language, ContentRepeater, ActivitySaveButton, ActivityCancelButton);
        }

        protected override void OnAlert(AlertType type, string message)
        {
            ScreenStatus.AddMessage(type, message);
        }

        protected override void BindModelToControls(QActivity activity)
        {
            SetVideoPanel(activity);
        }

        private void SetVideoPanel(QActivity activity)
        {
            ActivityIsMultilingual.SelectedValue = activity.ActivityIsMultilingual.ToString().ToLower();
            VideoUrl.Text = activity.ActivityUrl;
            VideoUrlLink.Visible = !string.IsNullOrEmpty(activity.ActivityUrl);
            VideoUrlLink.NavigateUrl = activity.ActivityUrl;

            VideoTarget.Value = activity.ActivityUrlTarget;

            var html = OutlineHelper.GenerateContent(
                activity.ActivityType,
                activity.ActivityUrlTarget,
                activity.ActivityUrl);

            if (html.IsNotEmpty())
                VideoViewer.Text = html;
            else
                VideoViewer.Text = "No Preview Available";

            var otherLanguageLinks = activity.ActivityIsMultilingual
                ? OutlineHelper.GetOtherLanguageLinks(CourseIdentifier, activity.ActivityUrl)
                : null;

            OtherLanguageUrlRepeater.Visible = otherLanguageLinks != null && otherLanguageLinks.Count > 0;
            OtherLanguageUrlRepeater.DataSource = otherLanguageLinks;
            OtherLanguageUrlRepeater.DataBind();
        }

        protected override void BindControlsToModel(QActivity activity)
        {
            activity.ActivityUrl = VideoUrl.Text;
            activity.ActivityUrlTarget = VideoTarget.Value;
            activity.ActivityIsMultilingual = ActivityIsMultilingual.SelectedValue == "true";
        }
    }
}