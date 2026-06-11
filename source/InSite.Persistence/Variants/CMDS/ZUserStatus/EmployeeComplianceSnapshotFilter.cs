using System;

using Shift.Common;

namespace InSite.Persistence.Plugin.CMDS
{
    [Serializable]
    public class EmployeeComplianceSnapshotFilter : Filter
    {
        public Guid? OrganizationIdentifier { get; set; }
        public Guid? DepartmentIdentifier { get; set; }
        public Guid? UserIdentifier { get; set; }
        public Guid? PrimaryProfileIdentifier { get; set; }
    }
}
