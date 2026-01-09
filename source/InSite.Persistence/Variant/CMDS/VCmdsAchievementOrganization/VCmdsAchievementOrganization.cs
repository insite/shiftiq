using System;

namespace InSite.Persistence.Plugin.CMDS
{
    public class VCmdsAchievementOrganization
    {
        public Guid AchievementIdentifier { get; set; }
        public Guid OrganizationIdentifier { get; set; }

        public virtual VCmdsAchievement Achievement { get; set; }
    }
}
