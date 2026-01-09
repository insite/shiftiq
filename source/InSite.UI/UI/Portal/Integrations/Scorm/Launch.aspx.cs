using System;

using InSite.Common.Web;
using InSite.Persistence;
using InSite.Persistence.Integration.Moodle;
using InSite.UI.Layout.Portal;
using InSite.Web.Integration;
using InSite.Web.Routing;

using Shift.Common;

namespace InSite.UI.Portal.Integrations.Scorm
{
    public partial class Launch : PortalBasePage
    {
        public override string ActionUrl => "ui/portal/integrations/scorm/launch";

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            try
            {
                var callbackActivity = RouteData.Values["activity"];
                if (callbackActivity == null)
                {
                    HandleError("The launch URL is missing an activity identifier.");
                    return;
                }
                var activityId = Guid.Parse(callbackActivity.ToString());

                var organization = Identity.Organization;
                var organizationId = CourseSearch.BindActivityFirst(x => x.Module.Unit.Course.OrganizationIdentifier, x => x.ActivityIdentifier == activityId);

                if (organizationId != organization.Identifier && organizationId != organization.ParentOrganizationIdentifier)
                {
                    var org = OrganizationSearch.Select(organizationId);
                    HandleError($"This activity belongs to another organization account ({org.Name}).");
                    return;
                }

                var courseActivity = CourseSearch.SelectActivity(activityId, x => x.Module.Unit);

                var courseId = courseActivity.Module.Unit.CourseIdentifier;

                ScormHeader.Text = courseActivity.Module.ModuleName + ": " + courseActivity.ActivityName;

                ScormCloseLink.NavigateUrl = RoutingConfiguration.PortalCourseUrl(courseId, activityId);

                if (ScormCloseLink.NavigateUrl.Contains("?"))
                    ScormCloseLink.NavigateUrl += "&sync=scorm-cloud";
                else
                    ScormCloseLink.NavigateUrl += "?sync=scorm-cloud";

                var url = GetScormLaunchUrl(courseActivity.ActivityPlatform, courseActivity.ActivityHook, courseId, activityId, organizationId);

                if (string.IsNullOrEmpty(url) || url == "#")
                    return;

                ScormContent.Src = url;
            }
            catch (Exception ex)
            {
                if (!HandleException(ex))
                    throw;
            }
        }

        private string GetScormLaunchUrl(string platform, string package, Guid courseId, Guid activityId, Guid organizationId)
        {
            if (StringHelper.Equals(platform, "Moodle"))
            {
                return GetMoodleLaunchUrl(package, activityId);
            }

            if (StringHelper.Equals(platform, "Scoop"))
            {
                HideNavigation(); // Scoop has its own navigation bar

                return GetScoopLaunchUrl(package, courseId, activityId);
            }

            return GetRusticiLaunchUrl(organizationId, activityId); // Default to Rustici SCORM Cloud
        }

        public static string GetScoopLaunchUrl(string package, Guid courseId, Guid activityId)
        {
            var activityUrl = RoutingConfiguration.PortalCourseUrl(courseId, activityId);

            activityUrl = HttpRequestHelper.GetAbsoluteUrl(activityUrl);

            var exitUrl = StringHelper.EncodeBase64(activityUrl);

            // SCORM content hosted in Moodle does not always launch correctly on iOS devices. (Learners are sometimes
            // prompted with the Moodle login page, so they are unable to access the content. This problem does not
            // occur on Android, Linux, OSX, or Windows devices.) To solve the immediate problem, I have implemented
            // basic support for the new open-source app Scoop.

            var baseUri = new Uri(ServiceLocator.AppSettings.Engine.Api.Scoop.BaseUrl);

            var relativePath = $"{Organization.Code}/{package}";

            var queryString = $"exitUrl={exitUrl}";

            var launchUri = new Uri(baseUri, relativePath + "?" + queryString);

            return launchUri.ToString();
        }

        private void HideNavigation()
        {
            NavigationBar.Visible = false;
        }

        private string GetMoodleLaunchUrl(string package, Guid activityId)
        {
            var options = ServiceLocator.AppSettings.Integration.Moodle;

            var callback = PathHelper.GetAbsoluteUrl(options.ProgressCallbackUrl);

            var moodle = new MoodleCloud(options.SiteUrl, options.TokenSecret, callback);

            return moodle.GetLaunchUrl(Identity.User.Identifier, Identity.User.FullName, package, activityId);
        }

        private string GetRusticiLaunchUrl(Guid organizationId, Guid activityId)
        {
            var options = ServiceLocator.AppSettings.Engine.Api.Scorm;

            var callback = PathHelper.GetAbsoluteUrl(options.CallbackPath);

            var organization = OrganizationSearch.Select(organizationId);

            var rustici = new ScormIntegrator(organization, Identity.User, activityId);

            if (!Validate(rustici))
                return null;

            return rustici.GetUrl(Request, callback);
        }

        private bool Validate(ScormIntegrator rustici)
        {
            if (rustici.Activity != null)
                return true;

            HandleError("The launch activity could not be validated.");
            return false;
        }

        private bool HandleException(Exception ex)
        {
            if (ex.Message == "Thread was being aborted.")
                return false;

            HandleError(ex.Message);
            return true;
        }

        private void HandleError(string error)
        {
            ScormHeader.Text = $"<span style='color:red;'>{error}</span>";
            ScormContent.Visible = false;
        }
    }
}