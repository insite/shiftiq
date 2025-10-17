using System;
using System.Collections.Generic;

using InSite.Application.Events.Read;
using InSite.Application.Standards.Read;

namespace InSite.Application.Records.Read
{
    public class QJournalSetup
    {
        public Guid JournalSetupIdentifier { get; set; }
        public Guid OrganizationIdentifier { get; set; }
        public Guid? FrameworkStandardIdentifier { get; set; }
        public string JournalSetupName { get; set; }
        public DateTimeOffset JournalSetupCreated { get; set; }
        public DateTimeOffset? JournalSetupLocked { get; set; }
        public DateTimeOffset? LastChangeTime { get; set; }
        public string LastChangeType { get; set; }
        public string LastChangeUser { get; set; }
        public Guid? EventIdentifier { get; set; }
        public Guid? AchievementIdentifier { get; set; }
        public bool IsValidationRequired { get; set; }
        public bool? AllowLogbookDownload { get; set; }
        public Guid? ValidatorMessageIdentifier { get; set; }
        public Guid? LearnerMessageIdentifier { get; set; }
        public Guid? LearnerAddedMessageIdentifier { get; set; }

        public virtual QAchievement Achievement { get; set; }
        public virtual QEvent Event { get; set; }
        public virtual VFramework Framework { get; set; }

        public virtual ICollection<QCompetencyRequirement> CompetencyRequirements { get; set; } = new HashSet<QCompetencyRequirement>();
        public virtual ICollection<QJournalSetupField> Fields { get; set; } = new HashSet<QJournalSetupField>();
        public virtual ICollection<QJournal> Journals { get; set; } = new HashSet<QJournal>();
        public virtual ICollection<QJournalSetupUser> Users { get; set; } = new HashSet<QJournalSetupUser>();
        public virtual ICollection<QJournalSetupGroup> Groups { get; set; } = new HashSet<QJournalSetupGroup>();
        public virtual ICollection<QAreaRequirement> AreaRequirements { get; set; } = new HashSet<QAreaRequirement>();
    }
}
