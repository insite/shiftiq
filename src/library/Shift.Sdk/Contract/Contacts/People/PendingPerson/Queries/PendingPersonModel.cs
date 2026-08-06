using System;

namespace Shift.Contract
{
    public class PendingPersonModel
    {
        public Guid PendingPersonIdentifier { get; set; }
        public Guid OrganizationIdentifier { get; set; }
        public Guid? GroupIdentifier { get; set; }
        public Guid SubmittedBy { get; set; }

        public string PersonCode { get; set; }
        public string UserEmail { get; set; }
        public string UserFirstName { get; set; }
        public string UserLastName { get; set; }
        public string UserMiddleName { get; set; }
        public string JobDivision { get; set; }
        public string JobTitle { get; set; }
        public string WorkAddressStreet1 { get; set; }
        public string WorkAddressStreet2 { get; set; }
        public string WorkAddressCity { get; set; }
        public string WorkAddressProvince { get; set; }
        public string WorkAddressPostalCode { get; set; }
        public string EmployeeStatus { get; set; }

        public DateTimeOffset SubmittedAt { get; set; }
    }
}
