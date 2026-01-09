using System;

namespace InSite.Persistence
{
    public class VGroupEmployer
    {
        public Guid GroupIdentifier { get; set; }
        public Guid OrganizationIdentifier { get; set; }
        public Guid UserIdentifier { get; set; }

        public Guid? EmployeeHomeAddressIdentifier { get; set; }
        public Guid? EmployeeWorkAddressIdentifier { get; set; }
        public Guid? EmployeeBillingAddressIdentifier { get; set; }
        public Guid? EmployeeShippingAddressIdentifier { get; set; }

        public string AddressCity { get; set; }
        public string AddressCountry { get; set; }
        public string AddressLine { get; set; }
        public string AddressPostalCode { get; set; }
        public string AddressProvince { get; set; }
        public string CompanySector { get; set; }
        public string ContactFullName { get; set; }
        public string ContactJobTitle { get; set; }
        public string Email { get; set; }
        public string FirstName { get; set; }
        public string GroupCode { get; set; }
        public string GroupIndustry { get; set; }
        public string GroupIndustryComment { get; set; }
        public string GroupName { get; set; }
        public string GroupPhone { get; set; }
        public string GroupSize { get; set; }
        public string LastName { get; set; }
        public string MiddleName { get; set; }
        public string Phone1 { get; set; }
        public string Phone2 { get; set; }
        public string Url { get; set; }

        public DateTimeOffset? Approved { get; set; }

        public DateTimeOffset GroupCreated { get; set; }
        public DateTimeOffset AssignedEmployerContactDate { get; set; }
        public DateTimeOffset EmployerContactCreated { get; set; }

        public DateTimeOffset LastChangeTime { get; set; }
        public string LastChangeType { get; set; }
        public string LastChangeUser { get; set; }
    }
}