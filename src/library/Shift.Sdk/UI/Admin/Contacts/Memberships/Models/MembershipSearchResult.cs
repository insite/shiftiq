using System;

namespace Shift.Sdk.UI
{
    public class MembershipSearchResult
    {
        public Guid GroupIdentifier { get; set; }
        public string GroupName { get; set; }
        public string GroupType { get; set; }
        public string GroupLabel { get; set; }

        public Guid UserIdentifier { get; set; }
        public string UserFullName { get; set; }
        public string PersonCode { get; set; }
        public string UserEmail { get; set; }

        public DateTimeOffset MembershipAssigned { get; set; }
        public DateTimeOffset? MembershipExpiry { get; set; }
        public string MembershipFunction { get; set; }
    }
}