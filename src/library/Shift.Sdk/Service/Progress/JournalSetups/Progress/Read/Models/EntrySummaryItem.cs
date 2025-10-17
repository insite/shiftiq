using System;

namespace InSite.Application.Records.Read
{
    public class EntrySummaryItem
    {
        public Guid UserIdentifier { get; set; }
        public Guid JournalSetupIdentifier { get; set; }
        public Guid ExperienceIdentifier { get; set; }
        public Guid? CompetencyStandardIdentifier { get; set; }

        public string User { get; set; }
        public string Logbook { get; set; }
        public string GAC { get; set; }
        public string Competency { get; set; }
        public DateTimeOffset Created { get; set; }
        public DateTime? Started { get; set; }
        public DateTime? Stopped { get; set; }
        public string Employer { get; set; }
        public string Supervisor { get; set; }
        public string TrainingLocation { get; set; }
        public string Class { get; set; }
        public int EntryNumber { get; set; }
        public bool Validated { get; set; }
        public decimal? EntryHours { get; set; }
        public decimal? CompetencyHours { get; set; }
        public string CompetencySatisfactionLevel { get; set; }
        public int? CompetencySkillRating { get; set; }
    }
}
