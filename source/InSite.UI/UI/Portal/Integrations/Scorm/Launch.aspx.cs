using System;

using InSite.Common.Web;
using InSite.Persistence;
using InSite.UI.Layout.Portal;
using InSite.Web.Integration;
using InSite.Web.Routing;

using Shift.Common;
using Shift.Common.Integration.Scoop;

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

                if (organizationId != organization.Identifier && organizationId != ServiceLocator.Partition.Identifier)
                {
                    var org = OrganizationSearch.Select(organizationId);
                    HandleError($"This activity belongs to another organization account ({org.Name}).");
                    return;
                }

                var courseActivity = CourseSearch.SelectActivity(activityId, x => x.Module.Unit);

                var courseId = courseActivity.Module.Unit.CourseIdentifier;

                // The launch URL opens the provider's content directly, so it applies the same paid
                // program gate as the course page.
                if (PaidProgramAchievementSearch.IsCourseAccessDenied(courseId, User.Identifier))
                {
                    HandleError(GetDisplayText("Your account is not granted permission to access this course."));
                    return;
                }

                ScormHeader.Text = courseActivity.Module.ModuleName + ": " + courseActivity.ActivityName;

                ScormCloseLink.NavigateUrl = RoutingConfiguration.PortalCourseUrl(courseId, activityId);

                var url = GetScormLaunchUrl(courseActivity.ActivityPlatform, courseActivity.ActivityHook,
                    courseActivity.ActivityIsMultilingual, CurrentLanguage, courseId, activityId, organizationId);

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

        private string GetScormLaunchUrl(string platform, string package, bool isMultilingual, string language, Guid courseId, Guid activityId, Guid organizationId)
        {
            if (!StringHelper.Equals(platform, "Scoop"))
            {
                HandleError("This activity is not configured for a supported SCORM platform.");
                return null;
            }

            HideNavigation(); // Scoop has its own navigation bar

            return GetScoopLaunchUrl(Request.Url.Host, package, isMultilingual, language, courseId, activityId, organizationId);
        }

        // The language is passed in rather than read here: this method is static, CurrentLanguage
        // is an instance property, and both callers already have one.
        public static string GetScoopLaunchUrl(string host, string package, bool isMultilingual, string language, Guid courseId, Guid activityId, Guid organizationId)
        {
            var scoop = ServiceLocator.AppSettings.Engine.Api.Scoop;

            var activityUrl = RoutingConfiguration.PortalCourseUrl(courseId, activityId);

            activityUrl = HttpRequestHelper.GetAbsoluteUrl(activityUrl);

            var encodedExitUrl = StringHelper.EncodeBase64(activityUrl);

            var progressUrl = StringHelper.Replace(PathHelper.GetAbsoluteUrl(scoop.CallbackPath), "{activity}", activityId.ToString());

            progressUrl = StringHelper.Replace(progressUrl, "{learner}", User.Identifier.ToString());

            // Scoop posts progress back with no credential of its own, so the
            // callback URL carries its own proof that Shift iQ minted it. An
            // unsigned callback is refused by ScoopController.
            progressUrl = ScoopCallbackSignature.Sign(progressUrl, activityId, User.Identifier, scoop.Relay?.Secret);

            var encodedProgressUrl = StringHelper.EncodeBase64(progressUrl);

            if (!scoop.Relay.Enabled)
                throw new Exception("Token relay authentication must be enabled for integration between Shift iQ and Scoop.");

            var linkGenerator = new ScoopLinkGenerator();

            // The account slug must identify the organization that hosts the SCORM package in
            // Scoop (e.g. the partition organization for global modules), which is not
            // necessarily the organization the current user is signed in to.
            var owner = OrganizationSearch.Select(organizationId);
            var accountSlug = owner != null ? owner.Code : Organization.Code;

            // Shift iQ owns the language rule: the administrator stores one base SCORM Package ID
            // and Scoop holds one package per language beneath it. Resolved here, where the
            // owning account is already known, because that is the account the packages live in.
            var courseSlug = ScoopPackageResolver.ResolveLaunchHandle(accountSlug, package, isMultilingual, language);

            var scoopUrl = linkGenerator.GenerateCourseUrl(Identity, host, accountSlug, courseSlug, encodedProgressUrl, encodedExitUrl);

            return scoopUrl;
        }

        private void HideNavigation()
        {
            NavigationBar.Visible = false;
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
