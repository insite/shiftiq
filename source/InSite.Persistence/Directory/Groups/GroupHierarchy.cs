using System;

namespace InSite.Persistence
{
    public class GroupHierarchy
    {
        public Guid? GroupIdentifier { get; set; }
        public String GroupName { get; set; }
        public String GroupType { get; set; }
        public Guid? ParentGroupIdentifier { get; set; }
        public Int32? PathDepth { get; set; }
        public String PathIndent { get; set; }
        public String PathName { get; set; }
        public Guid? OrganizationIdentifier { get; set; }
    }
}
