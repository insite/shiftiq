using System;

using Shift.Common;

namespace Shift.Contract
{
    public class CountFileActivities : Query<int>, IFileActivityCriteria
    {
        public Guid? FileIdentifier { get; set; }
        public Guid? UserIdentifier { get; set; }

        public string ActivityChanges { get; set; }

        public DateTimeOffset? ActivityTime { get; set; }
    }
}