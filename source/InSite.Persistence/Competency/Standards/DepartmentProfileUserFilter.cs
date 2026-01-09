using System;

using Shift.Common;

namespace InSite.Persistence
{
    [Serializable]
    public class DepartmentProfileUserFilter : Filter
    {
        public Guid? OrganizationIdentifier { get; set; }
        public Guid? DepartmentIdentifier { get; set; }
        public Guid? ProfileStandardIdentifier { get; set; }
        public Guid? UserIdentifier { get; set; }
    }
}
