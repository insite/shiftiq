using System;

namespace Shift.Common.Base
{
    public static class Clock
    {
        public static string DefaultTimeFormat = "h:mm tt";

        public static DateTimeOffset NextCentury = new DateTimeOffset(new DateTime(2100, 1, 1, 0, 0, 0), TimeSpan.Zero);

        public static string FormatTime(DateTimeOffset when, TimeZoneInfo zone)
        {
            var time = TimeZoneInfo.ConvertTime(when, zone);

            var alias = Shift.Common.Base.TimeZones.GetAlias(zone);

            var name = alias.GetName(time);

            var format = "{0:" + DefaultTimeFormat + "} {1}";

            return string.Format(format, time, name);
        }
    }
}