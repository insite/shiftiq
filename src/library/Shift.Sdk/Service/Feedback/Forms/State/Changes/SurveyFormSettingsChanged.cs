using Shift.Common.Timeline.Changes;

using Newtonsoft.Json;

using Shift.Constant;

namespace InSite.Domain.Surveys.Forms
{
    public class SurveyFormSettingsChanged : Change
    {
        public SurveyFormSettingsChanged(UserFeedbackType userFeedback, bool requireUserIdentification, bool requireUserAuthentication, int? responseLimitPerUser, int? durationMinutes, bool enableUserConfidentiality)
        {
            EnableUserConfidentiality = enableUserConfidentiality;
            UserFeedback = userFeedback;
            RequireUserIdentification = requireUserIdentification;
            RequireUserAuthentication = requireUserAuthentication;
            ResponseLimitPerUser = responseLimitPerUser;
            DurationMinutes = durationMinutes;
        }

        public bool EnableUserConfidentiality { get; }

        [JsonProperty(DefaultValueHandling = DefaultValueHandling.Include)]
        public UserFeedbackType UserFeedback { get; }

        public bool RequireUserIdentification { get; }
        public bool RequireUserAuthentication { get; }
        public int? ResponseLimitPerUser { get; }
        public int? DurationMinutes { get; set; }
    }
}