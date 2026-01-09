using System;

namespace Shift.Contract
{
    public class ModifyGroupConnection
    {
        public Guid ChildGroupIdentifier { get; set; }
        public Guid? OrganizationIdentifier { get; set; }
        public Guid ParentGroupIdentifier { get; set; }
    }
}