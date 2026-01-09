using System;

using Shift.Common;

namespace Shift.Contract
{
    public class CountSubmissions : Query<int>, ISubmissionCriteria
    {
        public Guid? GroupIdentifier { get; set; }
        public Guid? LastAnsweredQuestionIdentifier { get; set; }
        public Guid? LastChangeUser { get; set; }
        public Guid? OrganizationIdentifier { get; set; }
        public Guid? PeriodIdentifier { get; set; }
        public Guid? RespondentUserIdentifier { get; set; }
        public Guid? SurveyFormIdentifier { get; set; }

        public bool? ResponseIsLocked { get; set; }

        public string LastChangeType { get; set; }
        public string RespondentLanguage { get; set; }
        public string ResponseSessionStatus { get; set; }

        public DateTimeOffset? LastChangeTime { get; set; }
        public DateTimeOffset? ResponseSessionCompleted { get; set; }
        public DateTimeOffset? ResponseSessionCreated { get; set; }
        public DateTimeOffset? ResponseSessionStarted { get; set; }
    }
}