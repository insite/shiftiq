using System;

namespace InSite.Persistence
{
    public class Employer
    {
        public Guid EmployerGroupIdentifier { get; set; }
        public string EmployerGroupName { get; set; }
        public string EmployerGroupCategory { get; set; }

        public string EmployerDistrictName { get; set; }
        
        public Guid EmployerOrganizationIdentifier { get; set; }
        public string EmployerOrganizationName { get; set; }

        public int EmployeeCount { get; set; }
    }
}