using System;

namespace Shift.Common
{
    [Serializable]
    public class DateTimeOffsetRange
    {
        public DateTimeOffset? Since { get; set; }
        public DateTimeOffset? Before { get; set; }
        public bool IsEmpty => !Since.HasValue && !Before.HasValue;

        public DateTimeOffsetRange()
            : this(null, null)
        {

        }

        public DateTimeOffsetRange(DateTimeOffset? since, DateTimeOffset? before)
        {
            Since = since;
            Before = before;
        }

        public DateTimeOffsetRange(DateTime? since, DateTime? before, bool isUtc)
        {
            Since = !since.HasValue ? null : isUtc ? new DateTimeOffset(since.Value, TimeSpan.Zero) : (DateTimeOffset?)since;
            Before = !before.HasValue ? null : isUtc ? new DateTimeOffset(before.Value, TimeSpan.Zero) : (DateTimeOffset?)before;
        }

        public DateTimeOffsetRange(DateTime? since, DateTime? before, TimeSpan offset)
        {
            Since = !since.HasValue
                ? (DateTimeOffset?)null
                : new DateTimeOffset(DateTime.SpecifyKind(since.Value, DateTimeKind.Unspecified), offset);
            Before = !before.HasValue
                ? (DateTimeOffset?)null
                : new DateTimeOffset(DateTime.SpecifyKind(before.Value, DateTimeKind.Unspecified), offset);
        }

        public DateTimeOffsetRange Clone()
        {
            return (DateTimeOffsetRange)MemberwiseClone();
        }
    }
}