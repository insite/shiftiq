using System;

using Shift.Common;

namespace InSite.Application.Contacts.Read
{
    [Serializable]
    public class QMembershipReasonFilter : Filter
    {
        public Guid? MembershipIdentifier { get; set; }
        public Guid? UserIdentifier { get; set; }
        public Guid? GroupIdentifier { get; set; }
        public Guid[] GroupOrganizationIdentifiers { get; set; }
        public Guid? ReasonCreatedBy { get; set; }
        public Guid? OrganizationIdentifier { get; set; }
        public string GroupName { get; set; }
        public string ReasonType { get; set; }
        public string PersonCode { get; set; }
        public string ReasonSubtype { get; set; }
        public DateTimeOffset? ReasonEffectiveSince { get; set; }
        public DateTimeOffset? ReasonEffectiveBefore { get; set; }
        public DateTimeOffset? ReasonExpirySince { get; set; }
        public DateTimeOffset? ReasonExpiryBefore { get; set; }
        public string PersonOccupation { get; set; }
    }
}
