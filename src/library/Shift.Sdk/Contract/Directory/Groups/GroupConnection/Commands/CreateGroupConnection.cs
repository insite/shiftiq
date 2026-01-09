using System;

namespace Shift.Contract
{
    public class CreateGroupConnection
    {
        public Guid ChildGroupIdentifier { get; set; }
        public Guid? OrganizationIdentifier { get; set; }
        public Guid ParentGroupIdentifier { get; set; }
    }
}