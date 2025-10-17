using System;

namespace Shift.Contract
{
    public partial class GroupConnectionMatch
    {
        public Guid ChildGroupIdentifier { get; set; }
        public Guid ParentGroupIdentifier { get; set; }
    }
}