using System;

namespace InSite.Application.Contacts.Read
{
    public class GroupDetail
    {
        public Guid GroupIdentifier { get; set; }
        public string GroupName { get; set; }
        public string GroupCode { get; set; }
        public int MembershipCount { get; set; }
    }
}
