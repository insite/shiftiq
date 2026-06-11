using System;

namespace InSite.Persistence.Plugin.CMDS
{
    public class VCmdsAchievementCompetency
    {
        public Guid AchievementIdentifier { get; set; }
        public Guid CompetencyStandardIdentifier { get; set; }
        public string Number { get; set; }
        public string Summary { get; set; }
        public bool IsDeleted { get; set; }

        public virtual VCmdsAchievement Achievement { get; set; }
    }
}
