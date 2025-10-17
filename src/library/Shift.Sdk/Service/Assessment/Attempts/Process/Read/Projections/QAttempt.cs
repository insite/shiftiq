using System;
using System.Collections.Generic;
using System.ComponentModel;

using InSite.Application.Banks.Read;
using InSite.Application.Contacts.Read;
using InSite.Application.Registrations.Read;

using Shift.Common;
using Shift.Constant;

namespace InSite.Application.Attempts.Read
{
    [Serializable]
    public class QAttempt
    {
        public Guid AssessorUserIdentifier { get; set; }
        public Guid AttemptIdentifier { get; set; }
        public Guid FormIdentifier { get; set; }
        public Guid LearnerUserIdentifier { get; set; }
        public Guid OrganizationIdentifier { get; set; }
        public Guid? RegistrationIdentifier { get; set; }
        public Guid? GradingAssessorUserIdentifier { get; set; }

        public string AttemptGrade { get; set; }
        public string AttemptStatus { get; set; }
        public string AttemptTag { get; set; }
        public string AttemptLanguage { get; set; }
        public string UserAgent { get; set; }

        public bool AttemptIsPassing { get; set; }

        public decimal? AttemptDuration { get; set; }
        public int AttemptNumber { get; set; }
        public int? AttemptTimeLimit { get; set; }
        public int? AttemptPingInterval { get; set; }

        public decimal? AttemptPoints { get; set; }
        public decimal? AttemptScore { get; set; }
        public decimal? FormPoints { get; set; }

        [DefaultValue(true)]
        public bool TabNavigationEnabled { get; set; }
        public bool SectionsAsTabsEnabled { get; set; }
        public bool SingleQuestionPerTabEnabled { get; set; }
        public int? FormSectionsCount { get; set; }
        public int? ActiveQuestionIndex { get; set; }
        public int? ActiveSectionIndex { get; set; }
        public string TabTimeLimit { get; set; }

        public DateTimeOffset? AttemptGraded { get; set; }
        public DateTimeOffset? AttemptImported { get; set; }
        public DateTimeOffset? AttemptPinged { get; set; }
        public DateTimeOffset? AttemptStarted { get; set; }
        public DateTimeOffset? AttemptSubmitted { get; set; }

        public virtual VPerson AssessorPerson { get; set; }
        public virtual VUser AssessorUser { get; set; }
        public virtual VPerson LearnerPerson { get; set; }
        public virtual VUser LearnerUser { get; set; }
        public virtual QBankForm Form { get; set; }
        public virtual QRegistration Registration { get; set; }
        public virtual VUser GradingAssessor { get; set; }

        public virtual ICollection<QAttemptOption> Options { get; set; }
        public virtual ICollection<QAttemptMatch> Matches { get; set; }
        public virtual ICollection<QAttemptPin> Pins { get; set; }
        public virtual ICollection<QAttemptQuestion> Questions { get; set; }
        public virtual ICollection<QRegistration> Registrations { get; set; }
        public virtual ICollection<QAttemptSolution> Solutions { get; set; }
        public virtual ICollection<QAttemptSection> Sections { get; set; }

        public bool IsTimeLimitEnabled => AttemptTimeLimit.HasValue && AttemptTimeLimit.Value > 0
            || SectionsAsTabsEnabled && !TabNavigationEnabled
               && TabTimeLimit.ToEnum(SpecificationTabTimeLimit.Disabled) == SpecificationTabTimeLimit.AllTabs;

        public QAttempt()
        {
            Matches = new HashSet<QAttemptMatch>();
            Pins = new HashSet<QAttemptPin>();
            Options = new HashSet<QAttemptOption>();
            Questions = new HashSet<QAttemptQuestion>();
            Registrations = new HashSet<QRegistration>();
            Solutions = new HashSet<QAttemptSolution>();
            Sections = new HashSet<QAttemptSection>();
        }
    }
}