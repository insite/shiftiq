using System;

namespace Shift.Common
{
    public static class DateTimeHelper
    {
        public static long ToJsTime(this DateTimeOffset? csTime)
        {
            if (csTime == null) return 0;
            return (csTime.Value.Ticks - 621355968000000000) / 10000;
        }
        public static string ToDateString(this DateTimeOffset? csTime)
        {
            if (csTime == null) return "";
            return $"{csTime.Value.Month}/{csTime.Value.Day}/{csTime.Value.Year}";
        }
    }
}
