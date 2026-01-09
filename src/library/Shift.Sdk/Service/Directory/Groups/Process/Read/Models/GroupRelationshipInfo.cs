using System;

namespace InSite.Application.Contacts.Read
{
    public class GroupRelationshipInfo
    {
        public Guid? ParentGroupIdentifier { get; set; }
        public Guid ChildGroupIdentifier { get; set; }
        public bool? IsHierarchy { get; set; }
        public int Depth { get; set; }
    }
}
