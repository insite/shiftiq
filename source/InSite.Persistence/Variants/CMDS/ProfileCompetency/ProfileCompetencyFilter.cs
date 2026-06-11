using System;

using Shift.Common;

namespace InSite.Persistence.Plugin.CMDS
{
    [Serializable]
    public class ProfileCompetencyFilter : Filter
    {
        public Guid? ProfileStandardIdentifier { get; set; }
        public Guid? CompetencyStandardIdentifier { get; set; }
        public Guid? OrganizationIdentifier { get; set; }
    }
}
