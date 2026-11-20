using System;

namespace Shift.Common.Base
{
    public static class Clock
    {
        public static string DefaultTimeFormat = "h:mm tt";

        public static DateTimeOffset NextCentury = new DateTimeOffset(new DateTime(2100, 1, 1, 0, 0, 0), TimeSpan.Zero);

        public static string FormatTime(DateTimeOffset when, string zoneId)
        {
            var time = TimeZones.ConvertTime(when, zoneId);

            var abbrev = TimeZones.GetAbbreviation(zoneId);

            var name = abbrev.GetAbbreviation(time);

            var format = "{0:" + DefaultTimeFormat + "} {1}";

            return string.Format(format, time, name);
        }
    }
}
