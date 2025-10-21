using System;

using InSite.Persistence.Content;

namespace InSite.Portal.Surveys.Pages
{
    public static class SurveyUrl
    {
        public static string GetResumeUrl(Guid session) 
            => new LaunchCardAdapter().ResumeSurvey(session);

        public static string GetReviewUrl(Guid session)
            => new LaunchCardAdapter().ReviewSurvey(session);

        public static string GetStartUrl(int form, Guid? user = null, string language = null)
            => new LaunchCardAdapter().StartSurvey(ServiceLocator.Urls.GetApplicationUrl(CurrentSessionState.Identity.Organization.Code), form, user, language);
    }
}