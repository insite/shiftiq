using System;

namespace InSite.Persistence
{
    public class TDepartmentStandard
    {
        public Guid DepartmentIdentifier { get; set; }
        public Guid StandardIdentifier { get; set; }
        public DateTimeOffset? Assigned { get; set; }
        public virtual Department Department { get; set; }
        public virtual Standard Standard { get; set; }
    }
}
