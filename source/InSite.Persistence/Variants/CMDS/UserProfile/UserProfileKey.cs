using System;

namespace InSite.Persistence.Plugin.CMDS
{
    public class UserProfileKey
    {
        public Guid UserIdentifier { get; set; }
        public Guid ProfileStandardIdentifier { get; set; }
        public Guid DepartmentIdentifier { get; set; }
    }
}
