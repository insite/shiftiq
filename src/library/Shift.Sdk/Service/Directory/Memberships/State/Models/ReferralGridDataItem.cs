using System;

namespace InSite.Domain.Contacts
{
    public class ReferralGridDataItem
    {
        public Guid ReasonIdentifier { get; set; }
        public string ReasonType { get; set; }
        public string ReasonSubtype { get; set; }
        public string PersonOccupation { get; set; }
        public DateTimeOffset ReasonEffective { get; set; }
        public DateTimeOffset? ReasonExpiry { get; set; }
        public Guid MembershipIdentifier { get; set; }
        public Guid UserIdentifier { get; set; }
        public Guid GroupIdentifier { get; set; }
        public string GroupName { get; set; }
        public Guid? GroupParentIdentifier { get; set; }
        public string GroupParentType { get; set; }
        public string GroupParentName { get; set; }
        public Guid GroupOrganizationIdentifier { get; set; }
        public string GroupOrganizationCode { get; set; }
    }
}
