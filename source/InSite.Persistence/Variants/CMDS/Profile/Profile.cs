using System;

namespace InSite.Persistence.Plugin.CMDS
{
    public class Profile
    {
        public Decimal? CertificationHoursPercentCore { get; set; }
        public Decimal? CertificationHoursPercentNonCore { get; set; }
        public Boolean CertificationIsAvailable { get; set; }
        public Boolean IsLocked { get; set; }
        public String ProfileDescription { get; set; }
        public String ProfileNumber { get; set; }
        public Guid? ParentProfileStandardIdentifier { get; set; }
        public Guid ProfileStandardIdentifier { get; set; }
        public String ProfileTitle { get; set; }
        public Guid OrganizationIdentifier { get; set; }
        public String Visibility { get; set; }
    }
}
