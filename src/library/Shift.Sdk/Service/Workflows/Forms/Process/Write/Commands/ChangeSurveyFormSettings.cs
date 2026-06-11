using System;

using Shift.Common.Timeline.Commands;

using Shift.Constant;

namespace InSite.Application.Surveys.Write
{
    public class ChangeSurveyFormSettings : Command
    {
        public ChangeSurveyFormSettings(Guid form, UserFeedbackType userFeedback, bool requireUserIdentification, bool requireUserAuthentication, int? responseLimitPerUser, int? durationMinutes, bool enableUserConfidentiality)
        {
            AggregateIdentifier = form;
            EnableUserConfidentiality = enableUserConfidentiality;
            UserFeedback = userFeedback;
            RequireUserIdentification = requireUserIdentification;
            RequireUserAuthentication = requireUserAuthentication;
            ResponseLimitPerUser = responseLimitPerUser;
            DurationMinutes = durationMinutes;
        }

        public bool EnableUserConfidentiality { get; }
        public UserFeedbackType UserFeedback { get; }
        public bool RequireUserIdentification { get; }
        public bool RequireUserAuthentication { get; }
        public int? ResponseLimitPerUser { get; }
        public int? DurationMinutes { get; }
    }
}