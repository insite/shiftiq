using System;

namespace Shift.Common
{
    public class TimeZoneAlias
    {
        public string Generic { get; }

        public string Standard { get; }

        public string Daylight { get; }

        public string Moment { get; }

        private readonly TimeZoneInfo _zone;

        public TimeZoneAlias(TimeZoneInfo zone, string generic, string standard, string daylight, string moment)
        {
            _zone = zone;

            Generic = generic;
            Standard = standard;
            Daylight = daylight;
            Moment = moment;
        }

        public string GetName(DateTimeOffset when)
            => _zone.IsDaylightSavingTime(when) ? Daylight : Standard;
    }
}