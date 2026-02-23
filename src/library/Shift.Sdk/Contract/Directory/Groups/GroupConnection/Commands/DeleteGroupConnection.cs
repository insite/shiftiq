using System;

namespace Shift.Contract
{
    public class DeleteGroupConnection
    {
        public Guid ChildGroupId { get; set; }
        public Guid ParentGroupId { get; set; }
    }
}