using System;

namespace Shift.Contract
{
    public partial class GroupConnectionMatch
    {
        public Guid ChildGroupId { get; set; }
        public Guid ParentGroupId { get; set; }
    }
}