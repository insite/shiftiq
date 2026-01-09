using System;

namespace InSite.Persistence.Plugin.CMDS
{
    public class UserProfile
    {
        public String CurrentStatus { get; set; }
        public Guid DepartmentIdentifier { get; set; }
        public Boolean IsComplianceRequired { get; set; }
        public Boolean IsPrimary { get; set; }
        public Guid ProfileStandardIdentifier { get; set; }
        public Guid OrganizationIdentifier { get; set; }
        public Guid UserIdentifier { get; set; }
    }
}
