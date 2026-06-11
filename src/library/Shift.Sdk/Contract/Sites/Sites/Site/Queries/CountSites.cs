using System;

using Shift.Common;

namespace Shift.Contract
{
    public class CountSites : Query<int>, ISiteCriteria
    {
        public Guid? OrganizationId { get; set; }

        public DateTimeOffset? LastChangeTimeSince { get; set; }
        public DateTimeOffset? LastChangeTimeBefore { get; set; }
    }
}
