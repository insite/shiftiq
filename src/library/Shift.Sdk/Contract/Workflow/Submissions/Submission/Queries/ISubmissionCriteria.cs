using System;

using Shift.Common;

namespace Shift.Contract
{
    public interface ISubmissionCriteria : IQueryByOrganization
    {
        QueryFilter Filter { get; set; }

        Guid? GroupId { get; set; }
        Guid? LastAnsweredQuestionId { get; set; }
        Guid? LastChangeUser { get; set; }
        Guid? PeriodId { get; set; }
        Guid? RespondentUserId { get; set; }
        Guid? SurveyFormId { get; set; }

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