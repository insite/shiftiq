using System;

namespace InSite.Persistence
{
    public class Employee
    {
        public Guid EmployeeUserIdentifier { get; set; }
        public Guid EmployeeOrganizationIdentifier { get; set; }
        public string EmployeeFullName { get; set; }
        public string EmployeeFirstName { get; set; }
        public string EmployeeLastName { get; set; }
        public string EmployeeEmail { get; set; }
        public string EmployeeJobTitle { get; set; }
        public string EmployeeProcessStep { get; set; }
        public string EmployeeGender { get; set; }
        public string EmployeeContactLabel { get; set; }
        public string EmployeeContactType { get; set; }

        public Guid EmployerOrganizationIdentifier { get; set; }
        public Guid EmployerGroupIdentifier { get; set; }
        public string EmployerGroupName { get; set; }
        public string EmployerGroupNumber { get; set; }
        public string EmployerContactLabel { get; set; }

        public Guid? EmployerDistrictIdentifier { get; set; }
        public string EmployerDistrictName { get; set; }
        public string EmployerDistrictRegion { get; set; }

        public string EmployerShippingAddressStreet1 { get; set; }
        public string EmployerShippingAddressStreet2 { get; set; }
        public string EmployerShippingAddressCity { get; set; }
        public string EmployerShippingAddressProvince { get; set; }
        public string EmployerShippingAddressCountry { get; set; }
        public string EmployerShippingAddressPostalCode { get; set; }
        public string EmployerPhone { get; set; }
        public string EmployerPhoneFax { get; set; }

        public string EmployeeHonorific { get; set; }
        public string EmployeePhone { get; set; }
        public string EmployeePhoneHome { get; set; }
        public string EmployeePhoneMobile { get; set; }
        public DateTime? EmployeeMemberStartDate { get; set; }
        public DateTime? EmployeeMemberEndDate { get; set; }
        public string EmployeeShippingAddressStreet1 { get; set; }
        public string EmployeeShippingAddressCity { get; set; }
        public string EmployeeShippingAddressProvince { get; set; }
        public string EmployeeShippingAddressPostalCode { get; set; }
        public string EmployeeShippingAddressCountry { get; set; }
        public string EmployeeShippingPreference { get; set; }
        public string EmployeeAccountNumber { get; set; }
        public string EmployeeHomeAddressStreet1 { get; set; }
        public string EmployeeHomeAddressCity { get; set; }
        public string EmployeeHomeAddressProvince { get; set; }
        public string EmployeeHomeAddressPostalCode { get; set; }
        public string EmployeeHomeAddressCountry { get; set; }
    }
}