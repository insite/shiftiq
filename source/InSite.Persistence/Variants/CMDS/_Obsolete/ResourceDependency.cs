using System;

namespace InSite.Persistence.Plugin.CMDS
{
    [Obsolete("Replace with VCmdsAchievementDependency")]
    public class ResourceDependency
    {
        public string Type { get; set; }
        public string Name { get; set; }
    }
}
