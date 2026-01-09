using System;

using Shift.Common;

namespace Shift.Contract
{
    public interface ISubmissionCriteria
    {
        QueryFilter Filter { get; set; }
        
        Guid? GroupIdentifier { get; set; }
        Guid? LastAnsweredQuestionIdentifier { get; set; }
        Guid? LastChangeUser { get; set; }
        Guid? OrganizationIdentifier { get; set; }
        Guid? PeriodIdentifier { get; set; }
        Guid? RespondentUserIdentifier { get; set; }
        Guid? SurveyFormIdentifier { get; set; }

        bool? ResponseIsLocked { get; set; }

        string LastChangeType { get; set; }
        string RespondentLanguage { get; set; }
        string ResponseSessionStatus { get; set; }

        DateTimeOffset? LastChangeTime { get; set; }
        DateTimeOffset? ResponseSessionCompleted { get; set; }
        DateTimeOffset? ResponseSessionCreated { get; set; }
        DateTimeOffset? ResponseSessionStarted { get; set; }
    }
}