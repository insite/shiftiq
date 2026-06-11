using System;

namespace InSite.Persistence.Plugin.CMDS
{
    public class UserRole
    {
        public String GroupAbbreviation { get; set; }
        public Guid GroupIdentifier { get; set; }
        public String GroupName { get; set; }
        public String UserEmail { get; set; }
        public Guid UserIdentifier { get; set; }
    }
}
