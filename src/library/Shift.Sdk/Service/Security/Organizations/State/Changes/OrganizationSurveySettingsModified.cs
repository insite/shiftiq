using Shift.Common.Timeline.Changes;

namespace InSite.Domain.Organizations
{
    public class OrganizationSurveySettingsModified : Change
    {
        public SurveySettings Surveys { get; set; }

        public OrganizationSurveySettingsModified(SurveySettings surveys)
        {
            Surveys = surveys;
        }
    }
}
