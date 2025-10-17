using System;

namespace Shift.Contract
{
    public partial class GroupConnectionModel
    {
        public Guid ChildGroupIdentifier { get; set; }
        public Guid? OrganizationIdentifier { get; set; }
        public Guid ParentGroupIdentifier { get; set; }
    }
}