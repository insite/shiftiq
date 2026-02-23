using System;
using System.Collections.Generic;

using Shift.Common;

namespace Shift.Contract
{
    public class SearchSubmissions : Query<IEnumerable<SubmissionMatch>>, ISubmissionCriteria
    {
        public Guid? GroupId { get; set; }
        public Guid? LastAnsweredQuestionId { get; set; }
        public Guid? LastChangeUser { get; set; }
        public Guid? OrganizationId { get; set; }
        public Guid? PeriodId { get; set; }
        public Guid? RespondentUserId { get; set; }
        public Guid? SurveyFormId { get; set; }

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