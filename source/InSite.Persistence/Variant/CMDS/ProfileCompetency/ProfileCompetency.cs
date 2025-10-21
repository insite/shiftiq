using System;

namespace InSite.Persistence.Plugin.CMDS
{
    public class ProfileCompetency
    {
        public Decimal? CertificationHoursCore { get; set; }
        public Decimal? CertificationHoursNonCore { get; set; }
        public Guid CompetencyStandardIdentifier { get; set; }
        public Guid ProfileStandardIdentifier { get; set; }
    }
}
