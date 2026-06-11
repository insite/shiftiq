using System;

namespace InSite.Domain.Records
{
    [Serializable]
    public class CompetencyRequirement
    {
        public Guid Competency { get; set; }
        public decimal? Hours { get; set; }
        public int? JournalItems { get; set; }
        public int? SkillRating { get; set; }
        public bool IncludeHoursToArea { get; set; }
    }
}
