using System;

namespace InSite.Application.Contacts.Read
{
    public class GroupOutlineItem
    {
        public Guid GroupIdentifier { get; set; }
        public Guid? ParentGroupIdentifier { get; set; }
        public string GroupName { get; set; }
        public string GroupType { get; set; }
        public string GroupCode { get; set; }
        public int MemberCount { get; set; }
        public int GroupActionCount { get; set; }
    }
}
