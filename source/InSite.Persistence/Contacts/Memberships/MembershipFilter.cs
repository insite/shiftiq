using System;

using Shift.Common;

namespace InSite.Persistence
{
    [Serializable]
    public class MembershipFilter : Filter
    {
        public Guid? OrganizationIdentifier { get; set; }

        public string GroupType { get; set; }
        public string GroupLabel { get; set; }
        public string GroupName { get; set; }

        public string UserFullName { get; set; }
        public string UserCode { get; set; }
        public string UserEmail { get; set; }

        public Guid[] MembershipStatuses { get; set; }

        public string[] MembershipFunctions { get; set; }
        public bool? HasMembershipFunction { get; set; }

        public DateTimeOffset? EffectiveSince { get; set; }
        public DateTimeOffset? EffectiveBefore { get; set; }

        public DateTimeOffset? ExpirySince { get; set; }
        public DateTimeOffset? ExpiryBefore { get; set; }
    }
}
