using System;

using InSite.Admin.Courses.Courses;
using InSite.Application.Courses.Read;

using Shift.Common;
using Shift.Constant;

namespace InSite.Admin.Courses.Outlines.Controls
{
    public partial class ActivityEditDocument : BaseActivityEdit
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
            ActivitySetup.BindModelToControls(activity, ActivitySaveButton.ValidationGroup);
            SetDocumentPanel(activity);
        }

        private void SetDocumentPanel(QActivity activity)
        {
            ActivityIsMultilingual.SelectedValue = activity.ActivityIsMultilingual.ToString().ToLower();
            DocumentUrl.Text = activity.ActivityUrl;
            DocumentUrlLink.Visible = !string.IsNullOrEmpty(activity.ActivityUrl);
            DocumentUrlLink.NavigateUrl = activity.ActivityUrl;

            DocumentTarget.Value = activity.ActivityUrlTarget;

            var html = OutlineHelper.GenerateContent(
                activity.ActivityType,
                activity.ActivityUrlTarget,
                activity.ActivityUrl);

            if (html.IsNotEmpty())
                DocumentViewer.Text = html;
            else
                DocumentViewer.Text = "No Preview Available";

            var otherLanguageLinks = activity.ActivityIsMultilingual
                ? OutlineHelper.GetOtherLanguageLinks(CourseIdentifier, activity.ActivityUrl)
                : null;

            OtherLanguageUrlRepeater.Visible = otherLanguageLinks != null && otherLanguageLinks.Count > 0;
            OtherLanguageUrlRepeater.DataSource = otherLanguageLinks;
            OtherLanguageUrlRepeater.DataBind();
        }

        protected override void BindControlsToModel(QActivity activity)
        {
            activity.ActivityIsMultilingual = ActivityIsMultilingual.SelectedValue == "true";
            activity.ActivityUrl = DocumentUrl.Text;
            activity.ActivityUrlTarget = DocumentTarget.Value;
        }
    }
}