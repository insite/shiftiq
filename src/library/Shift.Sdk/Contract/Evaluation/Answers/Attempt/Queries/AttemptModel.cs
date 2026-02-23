using System;

namespace Shift.Contract
{
    public partial class AttemptModel
    {
        // Identity

        public Guid AttemptId { get; set; }
        public int AttemptNumber { get; set; }
        public string FormCode { get; set; }
        public Guid FormId { get; set; }
        public string FormName { get; set; }
        public Guid OrganizationId { get; set; }
        public Guid? RegistrationId { get; set; }

        // Participants

        public Guid AssessorUserId { get; set; }
        public Guid? GradingAssessorUserId { get; set; }
        public Guid LearnerUserId { get; set; }

        // Status / Result

        public string AttemptGrade { get; set; }
        public bool AttemptIsPassing { get; set; }
        public string AttemptStatus { get; set; }
        public string AttemptTag { get; set; }

        // Scoring

        public decimal? AttemptDuration { get; set; }
        public decimal? AttemptPoints { get; set; }
        public decimal? AttemptScore { get; set; }
        public decimal? FormPoints { get; set; }

        // Navigation / Display

        public int? ActiveQuestionIndex { get; set; }
        public int? ActiveSectionIndex { get; set; }
        public int? FormSectionsCount { get; set; }
        public bool SectionsAsTabsEnabled { get; set; }
        public bool SingleQuestionPerTabEnabled { get; set; }
        public bool TabNavigationEnabled { get; set; }
        public string TabTimeLimit { get; set; }

        // Configuration

        public string AttemptLanguage { get; set; }
        public int? AttemptPingInterval { get; set; }
        public int? AttemptTimeLimit { get; set; }
        public string UserAgent { get; set; }

        // Timestamps

        public DateTimeOffset? AttemptGraded { get; set; }
        public DateTimeOffset? AttemptImported { get; set; }
        public DateTimeOffset? AttemptPinged { get; set; }
        public DateTimeOffset? AttemptStarted { get; set; }
        public DateTimeOffset? AttemptSubmitted { get; set; }
    }
}
