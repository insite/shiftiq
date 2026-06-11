using System;
using System.Collections.Generic;

using Shift.Common;

namespace Shift.Contract
{
    public class SearchFileActivities : Query<IEnumerable<FileActivityMatch>>, IFileActivityCriteria
    {
        public Guid? OrganizationId { get; set; }

        public DateTimeOffset? ActivityTimeSince { get; set; }
        public DateTimeOffset? ActivityTimeBefore { get; set; }
    }
}
