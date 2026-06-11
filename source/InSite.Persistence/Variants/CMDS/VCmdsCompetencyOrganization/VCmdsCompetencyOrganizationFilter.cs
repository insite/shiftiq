using System;

using Shift.Common;

namespace InSite.Persistence.Plugin.CMDS
{
    [Serializable]
    public class VCmdsCompetencyOrganizationFilter : Filter
    {
        public Guid? OrganizationIdentifier { get; set; }
    }
}
