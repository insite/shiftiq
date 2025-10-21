using System;

using Shift.Common;

namespace InSite.Persistence.Plugin.CMDS
{
    public class PersonProfileFilter : Filter
    {
        public Guid? ProfileStandardIdentifier{ get; set; }
        public Boolean OnlyComplianceRequired { get; set; }
        public Boolean OnlyPrimaryProfile { get; set; }
    }
}
