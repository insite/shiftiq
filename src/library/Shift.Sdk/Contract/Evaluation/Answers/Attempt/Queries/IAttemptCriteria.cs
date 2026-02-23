using System;

using Shift.Common;

namespace Shift.Contract
{
    public interface IAttemptCriteria : IQueryByOrganization
    {
        QueryFilter Filter { get; set; }

        Guid? AssessorUserId { get; set; }
        Guid? FormId { get; set; }
        Guid? GradingAssessorUserId { get; set; }
        Guid? LearnerUserId { get; set; }
        Guid? RegistrationId { get; set; }

        bool? AttemptIsPassing { get; set; }
        bool? SectionsAsTabsEnabled { get; set; }
        bool? SingleQuestionPerTabEnabled { get; set; }
        bool? TabNavigationEnabled { get; set; }

        string AttemptGrade { get; set; }
        string AttemptLanguage { get; set; }
        string AttemptStatus { get; set; }
        string AttemptTag { get; set; }
        string TabTimeLimit { get; set; }
        string UserAgent { get; set; }

        int? ActiveQuestionIndex { get; set; }
        int? ActiveSectionIndex { get; set; }
        int? AttemptNumber { get; set; }
        int? AttemptPingInterval { get; set; }
        int? AttemptTimeLimit { get; set; }
        int? FormSectionsCount { get; set; }

        decimal? AttemptDuration { get; set; }
        decimal? AttemptPoints { get; set; }
        decimal? AttemptScore { get; set; }
        decimal? FormPoints { get; set; }

        DateTimeOffset? AttemptGraded { get; set; }
        DateTimeOffset? AttemptImported { get; set; }
        DateTimeOffset? AttemptPinged { get; set; }
        DateTimeOffset? AttemptStarted { get; set; }
        DateTimeOffset? AttemptSubmitted { get; set; }
    }
}