using System;

using Shift.Common;

namespace Shift.Contract
{
    public class CountFileActivities : Query<int>, IFileActivityCriteria
    {
        public Guid? OrganizationId { get; set; }

        public DateTimeOffset? ActivityTimeSince { get; set; }
        public DateTimeOffset? ActivityTimeBefore { get; set; }
    }
}
