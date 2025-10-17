using System;

using Shift.Common;

namespace Shift.Contract
{
    public class RetrieveGroupConnection : Query<GroupConnectionModel>
    {
        public Guid ChildGroupIdentifier { get; set; }
        public Guid ParentGroupIdentifier { get; set; }
    }
}