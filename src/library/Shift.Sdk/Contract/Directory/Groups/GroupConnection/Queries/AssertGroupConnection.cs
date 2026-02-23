using System;

using Shift.Common;

namespace Shift.Contract
{
    public class AssertGroupConnection : Query<bool>
    {
        public Guid ChildGroupId { get; set; }
        public Guid ParentGroupId { get; set; }
    }
}