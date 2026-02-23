using System;

namespace Shift.Contract
{
    public class CreateAttempt
    {
        public Guid AssessorUserId { get; set; }
        public Guid AttemptId { get; set; }
        public Guid FormId { get; set; }
        public Guid? GradingAssessorUserId { get; set; }
        public Guid LearnerUserId { get; set; }
        public Guid OrganizationId { get; set; }
        public Guid? RegistrationId { get; set; }

        public bool AttemptIsPassing { get; set; }
        public bool SectionsAsTabsEnabled { get; set; }
        public bool SingleQuestionPerTabEnabled { get; set; }
        public bool TabNavigationEnabled { get; set; }

        public string AttemptGrade { get; set; }
        public string AttemptLanguage { get; set; }
        public string AttemptStatus { get; set; }
        public string AttemptTag { get; set; }
        public string TabTimeLimit { get; set; }
        public string UserAgent { get; set; }

        public int? ActiveQuestionIndex { get; set; }
        public int? ActiveSectionIndex { get; set; }
        public int AttemptNumber { get; set; }
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