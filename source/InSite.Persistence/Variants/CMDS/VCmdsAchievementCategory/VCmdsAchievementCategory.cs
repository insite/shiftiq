using System;

namespace InSite.Persistence.Plugin.CMDS
{
    public class VCmdsAchievementCategory
    {
        public string AchievementLabel { get; set; }
        public Guid AchievementIdentifier { get; set; }
        public Guid CategoryIdentifier { get; set; }
        public Guid OrganizationIdentifier { get; set; }
        public string CategoryName { get; set; }
        public int? ClassificationSequence { get; set; }

        public virtual VCmdsAchievement Achievement { get; set; }
    }
}
