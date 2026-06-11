using System;

namespace Shift.Contract
{
    public class ModifyGroupConnection
    {
        public Guid ChildGroupId { get; set; }
        public Guid? OrganizationId { get; set; }
        public Guid ParentGroupId { get; set; }
    }
}