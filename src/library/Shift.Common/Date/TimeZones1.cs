using System;
using System.Collections.Generic;
using System.Linq;

namespace Shift.Common.Base
{
    public static class TimeZones
    {
        public static TimeZoneInfo Alaskan { get; } = TimeZoneInfo.FindSystemTimeZoneById("Alaskan Standard Time");
        public static TimeZoneInfo Atlantic { get; } = TimeZoneInfo.FindSystemTimeZoneById("Atlantic Standard Time");
        public static TimeZoneInfo Central { get; } = TimeZoneInfo.FindSystemTimeZoneById("Central Standard Time");
        public static TimeZoneInfo Eastern { get; } = TimeZoneInfo.FindSystemTimeZoneById("Eastern Standard Time");
        public static TimeZoneInfo Hawaiian { get; } = TimeZoneInfo.FindSystemTimeZoneById("Hawaiian Standard Time");
        public static TimeZoneInfo Mountain { get; } = TimeZoneInfo.FindSystemTimeZoneById("Mountain Standard Time");
        public static TimeZoneInfo Newfoundland { get; } = TimeZoneInfo.FindSystemTimeZoneById("Newfoundland Standard Time");
        public static TimeZoneInfo Pacific { get; } = TimeZoneInfo.FindSystemTimeZoneById("Pacific Standard Time");
        public static TimeZoneInfo Utc { get; } = TimeZoneInfo.FindSystemTimeZoneById("UTC");

        public static TimeZoneInfo[] Supported { get; } =
        {
            Alaskan, Atlantic, Central, Eastern, Hawaiian, Mountain, Newfoundland, Pacific, Utc
        };

        private static readonly Dictionary<TimeZoneInfo, TimeZoneAlias> Aliases;

        private static readonly Dictionary<string, TimeZoneInfo> Zones;

        static TimeZones()
        {
            Aliases = new Dictionary<TimeZoneInfo, TimeZoneAlias>
            {
                { Alaskan, new TimeZoneAlias(Alaskan, "AKT", "AKST", "AKDT", "America/Anchorage") },
                { Atlantic, new TimeZoneAlias(Atlantic, "AT", "AST", "ADT", "America/Halifax") },
                { Central, new TimeZoneAlias(Central, "CT", "CST", "CDT", "America/Winnipeg") },
                { Eastern, new TimeZoneAlias(Eastern, "ET", "EST", "EDT", "America/Toronto") },
                { Hawaiian, new TimeZoneAlias(Hawaiian, "HST", "HST", "HDT", "Pacific/Honolulu") },
                { Mountain, new TimeZoneAlias(Mountain, "MT", "MST", "MDT", "America/Edmonton") },
                { Newfoundland, new TimeZoneAlias(Newfoundland, "NT", "NST", "NDT", "America/St_Johns") },
                { Pacific, new TimeZoneAlias(Pacific, "PT", "PST", "PDT", "America/Vancouver") },
                { Utc, new TimeZoneAlias(Utc, "UTC", "UTC", "UTC", "UTC") }
            };

            Zones = new Dictionary<string, TimeZoneInfo>(StringComparer.OrdinalIgnoreCase);

            foreach (var tz in Supported)
            {
                var alias = GetAlias(tz);

                var names = new[] { tz.DaylightName, tz.DisplayName, tz.Id, tz.StandardName, alias.Daylight, alias.Generic, alias.Moment, alias.Standard };

                foreach (var name in names)
                    if (!Zones.ContainsKey(name))
                        Zones.Add(name, tz);
            }
        }

        public static DateTimeOffset Convert(DateTimeOffset when, string zone)
        {
            var tz = GetZone(zone);

            return TimeZoneInfo.ConvertTime(when, tz);
        }

        public static TimeZoneAlias GetAlias(string zone)
            => GetAlias(GetZone(zone));

        public static TimeZoneAlias GetAlias(TimeZoneInfo zone)
            => Aliases.ContainsKey(zone) ? Aliases[zone] : null;

        public static TimeZoneInfo GetZone(string zone)
        {
            if (zone.IsNotEmpty() && Zones.ContainsKey(zone))
                return Zones[zone];

            if (IsValidZone(zone))
                return TimeZoneInfo.FindSystemTimeZoneById(zone);

            return Utc;
        }

        public static TimeZoneInfo GetZone(DateTimeOffset when)
        {
            var zones = GetZones(when);

            foreach (var zone in zones)
                if (Supported.Any(x => x.Id == zone.Id))
                    return zone;

            return Utc;
        }

        public static TimeZoneInfo[] GetZones(DateTimeOffset when)
        {
            var offset = when.Offset;

            var zones = TimeZoneInfo.GetSystemTimeZones()
                .Where(tz => tz.GetUtcOffset(when) == offset)
                .OrderBy(x => x.StandardName)
                .ToArray();

            return zones;
        }

        public static TimeSpan GetOffset(DateTime when, string zone)
        {
            return GetZone(zone).GetUtcOffset(when);
        }

        public static bool IsValidZone(string zone)
        {
            if (zone.IsEmpty())
                return false;

            return TimeZoneInfo.GetSystemTimeZones().Any(x => x.Id == zone);
        }
    }
}