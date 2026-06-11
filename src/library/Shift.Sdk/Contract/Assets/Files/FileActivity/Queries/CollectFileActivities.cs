using System;
using System.Collections.Generic;

using Shift.Common;

namespace Shift.Contract
{
    public class CollectFileActivities : Query<IEnumerable<FileActivityModel>>, IFileActivityCriteria
    {
        public Guid? OrganizationId { get; set; }

        public DateTimeOffset? ActivityTimeSince { get; set; }
        public DateTimeOffset? ActivityTimeBefore { get; set; }
    }
}
