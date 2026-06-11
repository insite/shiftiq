using System;

using Shift.Common;

namespace Shift.Contract
{
    public class CountBanks : Query<int>, IBankCriteria
    {
        public Guid? OrganizationId { get; set; }

        public DateTimeOffset? LastChangeTimeSince { get; set; }
        public DateTimeOffset? LastChangeTimeBefore { get; set; }
        public bool? IsActive { get; set; }
    }
}