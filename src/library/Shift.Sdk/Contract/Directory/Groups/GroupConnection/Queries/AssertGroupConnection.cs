using System;

using Shift.Common;

namespace Shift.Contract
{
    public class AssertGroupConnection : Query<bool>
    {
        public Guid ChildGroupIdentifier { get; set; }
        public Guid ParentGroupIdentifier { get; set; }
    }
}