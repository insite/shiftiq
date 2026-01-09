
using System;

using Shift.Common;



namespace InSite.Persistence
{
    [Serializable]
    public class VGroupEmployerFilter : Filter
    {
        public Guid? OrganizationIdentifier { get; set; }
        public Guid? ParentGroupIdentifier { get; set; }
        public Guid[] GroupDepartmentIdentifiers { get; set; }

        public string Address { get; set; }
        public string Country { get; set; }
        public string Province { get; set; }
        public string City { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }

        public bool? IsApproved { get; set; }

        public string EmployerName { get; set; }
        public string EmployerContactName { get; set; }
        public string EmployerSize { get; set; }
        public string Industry { get; set; }
        public string Sector { get; set; }

        public DateTimeOffset? DateRegisteredSince { get; set; }
        public DateTimeOffset? DateRegisteredBefore { get; set; }
    }
}
