using System;

namespace InSite.Persistence.Plugin.CMDS
{
    public class Employment
    {
        public Guid DepartmentIdentifier { get; set; }
        public Guid ProfileStandardIdentifier { get; set; }
        public Guid OrganizationIdentifier { get; set; }
        public Guid UserIdentifier { get; set; }
    }
}
