using System;
using System.Collections.Generic;

using Shift.Common;

namespace Shift.Contract
{
    public class SearchFileActivities : Query<IEnumerable<FileActivityMatch>>, IFileActivityCriteria
    {
        public Guid? FileIdentifier { get; set; }
        public Guid? UserIdentifier { get; set; }

        public string ActivityChanges { get; set; }

        public DateTimeOffset? ActivityTime { get; set; }
    }
}