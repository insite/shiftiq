using System;
using System.Web.WebPages;

using InSite.Application.Courses.Read;
using InSite.Common.Web.UI;
using InSite.Web.Integration;

using Shift.Constant;

namespace InSite.Admin.Courses.Outlines.Controls
{
    public partial class ActivityEditLink : BaseActivityEdit
    {
        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            BindControlsToHandlers(ActivitySetup, Language, ContentRepeater, ActivitySaveButton, ActivityCancelButton);

            LinkType.AutoPostBack = true;
            LinkType.ValueChanged += (sender, args) => LinkTypeSelected();

            LinkUrl.AutoPostBack = true;
            LinkUrl.TextChanged += (sender, args) => SetNavigationUrl(LinkType.Value, LinkUrl.Text);
        }

        protected override void OnAlert(AlertType type, string message)
        {
            ScreenStatus.AddMessage(type, message);
        }

        protected override void BindModelToControls(QActivity activity)
        {
            LinkType.Value = activity.ActivityUrlType;
            ActivityPlatform.Value = activity.ActivityPlatform;
            ActivityHook.Text = activity.ActivityHook;
            BindScormStatus(activity.ActivityPlatform, activity.ActivityHook, activity.ActivityIdentifier);
            LinkIsMultilingual.Checked = activity.ActivityIsMultilingual;
            LinkIsPreview.Checked = activity.ActivityMode == "Preview";
            LinkIsDispatch.Checked = activity.ActivityIsDispatch;
            SetNavigationUrl(activity);
            LinkTarget.Value = activity.ActivityUrlTarget ?? "_blank";
            LinkTypeSelected();
        }

        private void BindScormStatus(string scormPlatform, string scormCourseId, Guid activityId)
        {
            if (scormPlatform != "SCORM Cloud")
                return;

            if (scormCourseId.IsEmpty())
                return;

            try
            {
                var integrator = new ScormIntegrator(Organization, User, activityId);

                var course = integrator.RetrieveCourse(scormCourseId);

                if (course != null)
                {
                    var info = course.CourseLearningStandard;

                    if (course.Updated.HasValue)
                        info += $" last updated {course.Updated:MMM d, yyyy}";

                    else if (course.Created.HasValue)
                        info += $" created {course.Created:MMM d, yyyy}";

                    ScormPackageStatus.Text = $"<span class='badge bg-success fs-sm'>{info}</span>";
                }
                else
                {
                    ScormPackageStatus.Text = $"<span class='badge bg-danger fs-sm'>Not Found on SCORM Cloud</span>";
                }
            }
            catch (Exception ex)
            {
                AppSentry.CaptureException(ex);
            }
        }

        protected override void BindControlsToModel(QActivity activity)
        {
            activity.ActivityUrlType = LinkType.Value;
            activity.ActivityPlatform = ActivityPlatform.Value;
            activity.ActivityHook = ActivityHook.Text;
            activity.ActivityIsMultilingual = LinkIsMultilingual.Checked;
            activity.ActivityMode = LinkIsPreview.Checked ? "Preview" : "Normal";
            activity.ActivityIsDispatch = LinkIsDispatch.Checked;
            activity.ActivityUrl = LinkUrl.Text;
            activity.ActivityUrlTarget = LinkTarget.Value;

            if (activity.ActivityUrlType == "SCORM")
            {
                var url = $"/ui/portal/integrations/scorm/launch/{activity.ActivityIdentifier}";
                if (activity.ActivityUrl == null || !activity.ActivityUrl.StartsWith(url))
                    activity.ActivityUrl = url;
            }

            if (activity.ActivityUrlType == "External" &&
                !activity.ActivityUrl.StartsWith("http://") && !activity.ActivityUrl.StartsWith("https://") && !activity.ActivityUrl.StartsWith("mailto:"))
            {
                activity.ActivityUrl = $"https://{activity.ActivityUrl}";
            }
        }

        private void SetNavigationUrl(string linkType, string url)
        {
            if (linkType == "External")
            {
                if (!url.StartsWith("http://") && !url.StartsWith("https://") && !url.StartsWith("mailto:"))
                    LinkUrlTest.NavigateUrl = $"https://{url}";
                else
                    LinkUrlTest.NavigateUrl = url;
            }
            else if (linkType == "Internal")
            {
                LinkUrlTest.NavigateUrl = $"{Request.Url.Scheme}://{Request.Url.Host}{url}";
            }
        }

        private void SetNavigationUrl(QActivity activity)
        {
            var hasUrl = !string.IsNullOrEmpty(activity.ActivityUrl);

            LinkUrl.Text = activity.ActivityUrl;
            LinkUrlTest.Visible = hasUrl;
            if (hasUrl)
            {
                SetNavigationUrl(activity.ActivityUrlType, activity.ActivityUrl);
            }
        }

        private void LinkTypeSelected()
        {
            var isScorm = LinkType.Value == "SCORM";

            ActivityPlatform.Visible = isScorm;

            var isCloud = isScorm && ActivityPlatform.Value == "SCORM Cloud";

            var isScoop = isScorm && ActivityPlatform.Value == "Scoop";

            ActivityHookField.Visible = isScorm;

            LinkUrlField.Visible = !isScorm;

            LinkIsMultilingual.Visible = isCloud;

            LinkIsPreview.Visible = isCloud;

            LinkIsDispatch.Visible = isCloud;

            LinkTargetField.Visible = isCloud;

            var scoopBaseUrl = ServiceLocator.AppSettings.Engine?.Api?.Scoop?.BaseUrl;

            if (isScoop && scoopBaseUrl != null)
            {
                var baseUri = new Uri(scoopBaseUrl);

                var libraryUrl = new Uri(baseUri, Organization.Code);

                ScoopLibraryUrl.Text = $"{Organization.Name} SCO Library";

                ScoopLibraryUrl.NavigateUrl = libraryUrl.AbsoluteUri.ToString();

                ScoopLibraryUrl.Visible = true;

                ScoopLibraryUrl.Target = "_blank";
            }

            if (isCloud)
            {
                if (LinkTarget.Value == "_embed")
                    LinkTarget.Value = "_self";

                foreach (var option in LinkTarget.FlattenOptions())
                    option.Visible = option.Value != "_embed";
            }
            else
            {
                foreach (var option in LinkTarget.FlattenOptions())
                    option.Visible = true;
            }

            SetNavigationUrl(LinkType.Value, LinkUrl.Text);
        }
    }
}