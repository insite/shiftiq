using System;

namespace InSite.Persistence.Plugin.CMDS
{
    public class CmdsDepartmentProfileCompetency
    {
        public Guid CompetencyStandardIdentifier { get; set; }
        public String Criticality { get; set; }
        public Guid DepartmentIdentifier { get; set; }
        public Guid ProfileStandardIdentifier { get; set; }
        public Int32? ValidForCount { get; set; }
        public String ValidForUnit { get; set; }
    }
}
