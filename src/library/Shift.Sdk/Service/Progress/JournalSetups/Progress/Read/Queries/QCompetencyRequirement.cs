using System;

using InSite.Application.Standards.Read;

namespace InSite.Application.Records.Read
{
    public class QCompetencyRequirement
    {
        public Guid JournalSetupIdentifier { get; set; }
        public Guid CompetencyStandardIdentifier { get; set; }
        public decimal? CompetencyHours { get; set; }
        public int? JournalItems { get; set; }
        public int? SkillRating { get; set; }
        public bool IncludeHoursToArea { get; set; }

        public virtual QJournalSetup JournalSetup { get; set; }
        public virtual VCompetency Competency { get; set; }
    }
}
