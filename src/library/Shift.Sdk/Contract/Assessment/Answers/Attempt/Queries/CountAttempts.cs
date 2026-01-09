using System;

using Shift.Common;

namespace Shift.Contract
{
    public class CountAttempts : Query<int>, IAttemptCriteria
    {
        public Guid? AssessorUserIdentifier { get; set; }
        public Guid? FormIdentifier { get; set; }
        public Guid? GradingAssessorUserIdentifier { get; set; }
        public Guid? LearnerUserIdentifier { get; set; }
        public Guid? OrganizationIdentifier { get; set; }
        public Guid? RegistrationIdentifier { get; set; }

        public bool? AttemptIsPassing { get; set; }
        public bool? SectionsAsTabsEnabled { get; set; }
        public bool? SingleQuestionPerTabEnabled { get; set; }
        public bool? TabNavigationEnabled { get; set; }

        public string AttemptGrade { get; set; }
        public string AttemptLanguage { get; set; }
        public string AttemptStatus { get; set; }
        public string AttemptTag { get; set; }
        public string TabTimeLimit { get; set; }
        public string UserAgent { get; set; }

        public int? ActiveQuestionIndex { get; set; }
        public int? ActiveSectionIndex { get; set; }
        public int? AttemptNumber { get; set; }
        public int? AttemptPingInterval { get; set; }
        public int? AttemptTimeLimit { get; set; }
        public int? FormSectionsCount { get; set; }

        public decimal? AttemptDuration { get; set; }
        public decimal? AttemptPoints { get; set; }
        public decimal? AttemptScore { get; set; }
        public decimal? FormPoints { get; set; }

        public DateTimeOffset? AttemptGraded { get; set; }
        public DateTimeOffset? AttemptImported { get; set; }
        public DateTimeOffset? AttemptPinged { get; set; }
        public DateTimeOffset? AttemptStarted { get; set; }
        public DateTimeOffset? AttemptSubmitted { get; set; }
    }
}