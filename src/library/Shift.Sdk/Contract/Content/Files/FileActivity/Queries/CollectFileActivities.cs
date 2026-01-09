using System;
using System.Collections.Generic;

using Shift.Common;

namespace Shift.Contract
{
    public class CollectFileActivities : Query<IEnumerable<FileActivityModel>>, IFileActivityCriteria
    {
        public Guid? FileIdentifier { get; set; }
        public Guid? UserIdentifier { get; set; }

        public string ActivityChanges { get; set; }

        public DateTimeOffset? ActivityTime { get; set; }
    }
}