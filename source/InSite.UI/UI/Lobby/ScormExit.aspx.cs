using System;

using InSite.Common.Web;
using InSite.Persistence;

namespace InSite.UI.Lobby
{
    public partial class ScormExit : Layout.Lobby.LobbyBasePage
    {
        public string RedirectUrl { get; set; }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            var activityId = Guid.Parse(RouteData.Values["activity"].ToString());

            var activity = CourseSearch.SelectActivity(activityId, x => x.Module.Unit);

            var scormRegistrationId = Guid.Parse(RouteData.Values["registration"].ToString());

            var scormRegistration = ScormRegistrationSearch.Select(scormRegistrationId);

            if (activity == null || scormRegistration == null)
                RedirectToHome();

            var isAccessDenied = activity == null
                || TGroupPermissionSearch.IsAccessDenied(activity.ActivityIdentifier, CurrentSessionState.Identity)
                || TGroupPermissionSearch.IsAccessDenied(activity.Module.Unit.CourseIdentifier, CurrentSessionState.Identity);

            if (isAccessDenied)
                RedirectToHome();

            RedirectUrl = $"/ui/portal/learning/course/{activity.Module.Unit.CourseIdentifier}?activity={activity.ActivityIdentifier}&sync=scorm-cloud";
        }

        private void RedirectToHome()
        {
            HttpResponseHelper.Redirect("/");
        }
    }
}