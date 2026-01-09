using System;

using Shift.Constant;

namespace InSite.Domain.Records
{
    [Serializable]
    public class ExperienceCompetency
    {
        public Guid Competency { get; set; }
        public decimal? Hours { get; set; }
        public int? SkillRating { get; set; }
        public ExperienceCompetencySatisfactionLevel SatisfactionLevel { get; set; } = ExperienceCompetencySatisfactionLevel.None;

        public bool ShouldSerializeSatisfactionLevel() => SatisfactionLevel != ExperienceCompetencySatisfactionLevel.None;
    }
}
