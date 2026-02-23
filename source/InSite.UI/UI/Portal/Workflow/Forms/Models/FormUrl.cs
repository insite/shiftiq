using System;

using InSite.Persistence.Content;

namespace InSite.UI.Portal.Workflow.Forms.Models
{
    public static class FormUrl
    {
        public static string GetResumeUrl(Guid session)
            => new LaunchCardAdapter(ServiceLocator.Partition).ResumeSurvey(session);

        public static string GetReviewUrl(Guid session)
            => new LaunchCardAdapter(ServiceLocator.Partition).ReviewSurvey(session);

        public static string GetStartUrl(int form, Guid? user = null, string language = null)
            => new LaunchCardAdapter(ServiceLocator.Partition).StartSurvey(ServiceLocator.Urls.GetApplicationUrl(CurrentSessionState.Identity.Organization.Code), form, user, language);
    }
}