using System;

namespace InSite.Persistence
{
    public class CompanyDepartment
    {
        public Guid CompanyKey { get; set; }
        public Guid DepartmentIdentifier { get; set; }
        public Guid OrganizationIdentifier { get; set; }

        public string CompanyName { get; set; }
        public string DepartmentName { get; set; }
    }
}
