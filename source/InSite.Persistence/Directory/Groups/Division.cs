using System;
using System.Collections.Generic;

namespace InSite.Persistence
{
    public class Division
    {
        public Guid DivisionIdentifier { get; set; }

        public string DivisionCode { get; set; }
        public string DivisionDescription { get; set; }
        public string DivisionName { get; set; }

        public Guid OrganizationIdentifier { get; set; }
        public VOrganization Organization { get; set; }

        public DateTimeOffset GroupCreated { get; set; }
        public DateTimeOffset LastChangeTime { get; set; }

        public string LastChangeUser { get; set; }

        public virtual ICollection<Department> Departments { get; set; }

        public Division()
        {
            Departments = new HashSet<Department>();
        }
    }
}
