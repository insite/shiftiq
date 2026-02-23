using System;

namespace InSite.Application.Surveys2.Read
{
    public class VSurveyRespondent
    {
        public Guid SurveyFormIdentifier { get; set; }
        public string SurveyFormName { get; set; }

        public Guid RespondentUserIdentifier { get; set; }
        public string FullName { get; set; }
        public string Email { get; set; }

        public Guid? ResponseSessionIdentifier { get; set; }
        public string ResponseSessionStatus { get; set; }
        public DateTimeOffset? ResponseSessionStarted { get; set; }
        public DateTimeOffset? ResponseSessionCompleted { get; set; }
        public bool? ResponseIsLocked { get; set; }
    }
}
