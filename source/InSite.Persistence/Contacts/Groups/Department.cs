using System;
using System.Collections.Generic;

namespace InSite.Persistence
{
    public class Department
    {
        public Guid DepartmentIdentifier { get; set; }

        public string DepartmentCode { get; set; }
        public string DepartmentDescription { get; set; }
        public string DepartmentName { get; set; }

        public Guid? DivisionIdentifier { get; set; }
        public Guid? ParentDepartmentIdentifier { get; set; }
        public Guid OrganizationIdentifier { get; set; }

        public DateTimeOffset GroupCreated { get; set; }
        public DateTimeOffset LastChangeTime { get; set; }

        public string LastChangeUser { get; set; }

        public string DepartmentLabel { get; set; }

        public virtual Division Division { get; set; }
        public virtual VOrganization Organization { get; set; }

        public virtual ICollection<DepartmentProfileCompetency> ProfileCompetencies { get; set; } = new HashSet<DepartmentProfileCompetency>();
        public virtual ICollection<DepartmentProfileUser> ProfileUsers { get; set; } = new HashSet<DepartmentProfileUser>();
        public virtual ICollection<TDepartmentStandard> Standards { get; set; } = new HashSet<TDepartmentStandard>();
    }
}
