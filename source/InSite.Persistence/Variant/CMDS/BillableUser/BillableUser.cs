using System;

namespace InSite.Persistence.Plugin.CMDS
{
    public class BillableUser
    {
        public String BillingClassification { get; set; }
        public String City { get; set; }
        public String CompanyName { get; set; }
        public String Email { get; set; }
        public String FirstName { get; set; }
        public String LastName { get; set; }
        public String FullName { get; set; }
        public String Phone { get; set; }
        public String PostalCode { get; set; }
        public Int32? ProfileCount { get; set; }
        public String Province { get; set; }
        public String Street { get; set; }
        public Guid OrganizationIdentifier { get; set; }
        public Int32? TimeSensitiveResourceCount { get; set; }
        public Boolean? UserIsApproved { get; set; }
        public Boolean? UserIsDisabled { get; set; }
        public Guid UserIdentifier { get; set; }
        public String UserName { get; set; }
    }
}
