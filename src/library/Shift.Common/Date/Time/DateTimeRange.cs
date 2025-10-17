using System;

namespace Shift.Common
{
    [Serializable]
    public class DateTimeRange
    {
        public DateTime? Since { get; set; }
        public DateTime? Before { get; set; }
        public bool TruncateTime { get; set; }
        public bool IsEmpty => !Since.HasValue && !Before.HasValue;

        public DateTimeRange(bool truncateTime = true)
            : this(null, null, truncateTime)
        {

        }

        public DateTimeRange(DateTime? since, DateTime? before, bool truncateTime = true)
        {
            Since = since;
            Before = before;
            TruncateTime = truncateTime;
        }
    }
}
