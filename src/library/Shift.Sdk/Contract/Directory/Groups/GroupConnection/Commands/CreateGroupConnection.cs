using System;

namespace Shift.Contract
{
    public class CreateGroupConnection
    {
        public Guid ChildGroupId { get; set; }
        public Guid? OrganizationId { get; set; }
        public Guid ParentGroupId { get; set; }
    }
}