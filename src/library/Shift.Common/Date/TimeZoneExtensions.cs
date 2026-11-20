using System;
using System.Globalization;
using System.Text;

namespace Shift.Common
{
    public static class TimeZoneExtensions
    {
        public static string Format(this DateTime dateValue) =>
            TimeZones.Format(dateValue);

        public static string Format(this DateTimeOffset dateValue, string zone) =>
            TimeZones.Format(dateValue, zone);

        public static string Format(this DateTimeOffset? dateValue, string zone, string nullValue) =>
            dateValue.HasValue ? dateValue.Value.Format(zone) : nullValue;

        public static string Format(this DateTimeOffset dateValue, string zoneId = null, bool isHtml = false, bool wrap = false, bool showSeconds = false, CultureInfo culture = null) =>
            TimeZones.Format(dateValue, zoneId, isHtml, wrap, showSeconds, culture);

        public static string Format(this DateTimeOffset? dateValue, string zoneId = null, bool isHtml = false, bool wrap = false, bool showSeconds = false, CultureInfo culture = null, string nullValue = null) =>
            dateValue.HasValue ? dateValue.Value.Format(zoneId, isHtml, wrap, showSeconds, culture) : nullValue;

        public static string FormatDateOnly(this DateTimeOffset dateValue) =>
            TimeZones.FormatDateOnly(dateValue, TimeZones.FindTimeZoneId(dateValue));

        public static string FormatDateOnly(this DateTimeOffset? dateValue, string zone = null, string nullValue = null) =>
            dateValue.HasValue ? dateValue.Value.FormatDateOnly(zone, null, null) : nullValue;

        public static string FormatDateOnly(this DateTimeOffset dateValue, string zoneId, CultureInfo culture = null) =>
            TimeZones.FormatDateOnly(dateValue, zoneId, culture);

        public static string FormatDateOnly(this DateTimeOffset dateValue, string zone, CultureInfo culture, string format) =>
            TimeZones.FormatDateOnly(dateValue, zone, culture, format);

        public static string FormatTimeOnly(this DateTimeOffset dateValue) =>
            TimeZones.FormatTimeOnly(dateValue, TimeZones.FindTimeZoneId(dateValue));

        public static string FormatTimeOnly(this DateTimeOffset? dateValue, string nullValue = null) =>
            dateValue.HasValue ? dateValue.Value.FormatTimeOnly() : nullValue;

        public static string FormatTimeOnly(this DateTimeOffset dateValue, string zoneId) =>
            TimeZones.FormatTimeOnly(dateValue, zoneId);
    }
}
