using System;

namespace Shift.Contract
{
    public partial class OrganizationMatch
    {
        public Guid OrganizationIdentifier { get; set; }
        public Guid? ParentOrganizationIdentifier { get; set; }
        public string CompanyName { get; set; }
    }
}