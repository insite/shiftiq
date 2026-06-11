using System;

using Shift.Common;

namespace Shift.Contract
{
    public class RetrieveGroupConnection : Query<GroupConnectionModel>
    {
        public Guid ChildGroupId { get; set; }
        public Guid ParentGroupId { get; set; }
    }
}