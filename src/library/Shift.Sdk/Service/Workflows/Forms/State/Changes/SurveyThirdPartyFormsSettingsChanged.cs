using Shift.Common.Timeline.Changes;

namespace InSite.Domain.Surveys.Forms
{
    public class SurveyThirdPartyFormsSettingsChanged : Change
    {
        public bool Enabled { get; set; }

        public SurveyThirdPartyFormsSettingsChanged(bool enabled)
        {
            Enabled = enabled;
        }
    }
}
