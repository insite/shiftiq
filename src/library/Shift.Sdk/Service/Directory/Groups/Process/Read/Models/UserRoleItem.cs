using System;

namespace InSite.Application.Contacts.Read
{
    public class UserRoleItem
    {
        public Guid GroupIdentifier { get; set; }
        public string GroupName { get; set; }
        public bool OnlyOperatorCanAddUser { get; set; }
        public bool Selected { get; set; }
    }
}
