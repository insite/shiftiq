using System;

namespace InSite.Application.Identities.Departments.Read
{
    public class TDepartment
    {
        public Guid? BillingAddressIdentifier { get; set; }
        public Guid CreatedBy { get; set; }
        public Guid DepartmentIdentifier { get; set; }
        public Guid ModifiedBy { get; set; }
        public Guid? PhysicalAddressIdentifier { get; set; }
        public Guid? ShippingAddressIdentifier { get; set; }

        public string DepartmentCode { get; set; }
        public string DepartmentDescription { get; set; }
        public string DepartmentName { get; set; }

        public bool AccessGrantedToCmds { get; set; }
        public bool AccessGrantedToPortal { get; set; }

        public Guid? DivisionIdentifier { get; set; }
        public Guid? ParentDepartmentIdentifier { get; set; }
        public Guid OrganizationIdentifier { get; set; }

        public DateTimeOffset Created { get; set; }
        public DateTimeOffset Modified { get; set; }
    }
}
