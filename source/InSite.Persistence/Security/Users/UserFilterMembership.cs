using System;

namespace InSite.Persistence
{
    [Serializable]
    public class UserFilterMembership
    {
        public Guid? MembershipGroupIdentifier { get; set; }
        public string MembershipGroupName { get; set; }
        public string MembershipType { get; set; }
        public bool MembershipTypeAnd { get; set; }
    }
}
