using System;

namespace Shift.Contract
{
    public class DeleteGroupConnection
    {
        public Guid ChildGroupIdentifier { get; set; }
        public Guid ParentGroupIdentifier { get; set; }
    }
}