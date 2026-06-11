using System;

using InSite.Application.Standards.Read;

namespace InSite.Application.Records.Read
{
    public class QExperienceCompetency
    {
        public Guid ExperienceIdentifier { get; set; }
        public Guid CompetencyStandardIdentifier { get; set; }
        public decimal? CompetencyHours { get; set; }
        public int? SkillRating { get; set; }
        public string SatisfactionLevel { get; set; }

        public virtual QExperience Experience { get; set; }
        public virtual VCompetency Competency { get; set; }
    }
}
