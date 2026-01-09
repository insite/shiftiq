using System;

using Shift.Common;

namespace InSite.Application.Surveys.Read
{
    [Serializable]
    public class QResponseSessionFilter : Filter
    {
        public Guid? AgencyGroupIdentifier { get; set; }
        public Guid? GroupIdentifier { get; set; }
        public Guid[] PeriodIdentifiers { get; set; }
        public Guid OrganizationIdentifier { get; set; }
        public Guid? RespondentUserIdentifier { get; set; }
        public Guid[] RespondentUserIdentifiers { get; set; }
        public Guid? SurveyFormIdentifier { get; set; }
        public Guid? SurveyQuestionIdentifier { get; set; }
        
        public string ResponseAnswerText { get; set; }
        public string RespondentName { get; set; }
        
        public DateTimeOffset? StartedSince { get; set; }
        public DateTimeOffset? StartedBefore { get; set; }
        public DateTimeOffset? CompletedSince { get; set; }
        public DateTimeOffset? CompletedBefore { get; set; }

        public bool IsPlatformAdministrator { get; set; }
        public bool? IsLocked { get; set; }
    }
}