using System;

namespace InSite.Persistence.Plugin.CMDS
{
    public class CompetencyCategory
    {
        public String CategoryName { get; set; }
        public String CompetencyKnowledge { get; set; }
        public String CompetencyNumber { get; set; }
        public String CompetencyAchievements { get; set; }
        public String CompetencySkills { get; set; }
        public Guid CompetencyStandardIdentifier { get; set; }
        public String CompetencySummary { get; set; }
    }
}
