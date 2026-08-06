using System;

using Shift.Common;

namespace Shift.Contract
{
    public class CountGroups : Query<int>, IGroupCriteria
    {
        public Guid? OrganizationId { get; set; }

        public string GroupCode { get; set; }
        public DateTimeOffset? GroupCreatedSince { get; set; }
        public DateTimeOffset? GroupCreatedBefore { get; set; }
        public DateTimeOffset? GroupExpirySince { get; set; }
        public DateTimeOffset? GroupExpiryBefore { get; set; }
        public DateTimeOffset? LastChangeTimeSince { get; set; }
        public DateTimeOffset? LastChangeTimeBefore { get; set; }
    }
}
