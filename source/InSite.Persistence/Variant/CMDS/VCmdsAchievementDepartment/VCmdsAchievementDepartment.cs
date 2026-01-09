using System;

namespace InSite.Persistence.Plugin.CMDS
{
    public class VCmdsAchievementDepartment
    {
        public Guid AchievementIdentifier { get; set; }
        public Guid DepartmentIdentifier { get; set; }

        public virtual VCmdsAchievement Achievement { get; set; }
    }
}
