using System;
using System.Globalization;
using System.Text;

namespace Shift.Common
{
    public static class TimeZoneExtensions
    {
        public static TimeZoneAbbreviation GetAbbreviation(this TimeZoneInfo info) =>
            TimeZones.GetAbbreviation(info);

        public static TimeZoneInfo GetTimeZone(this DateTimeOffset dateValue) =>
            TimeZones.Find(dateValue);

        public static TimeZoneInfo GetTimeZone(this DateTimeOffset? dateValue) =>
            dateValue.HasValue ? TimeZones.Find(dateValue.Value) : null;

        public static DateTimeOffset SetTimeZone(this DateTimeOffset dateValue, TimeSpan offset) =>
            new DateTimeOffset(dateValue.DateTime, offset);

        public static string Format(this DateTime dateValue) =>
            TimeZones.Format(dateValue);

        public static string Format(this DateTime? dateValue, string nullValue = null) =>
            dateValue.HasValue ? dateValue.Value.Format() : nullValue;

        public static string Format(this DateTimeOffset dateValue, string zone) =>
            TimeZones.Format(dateValue, zone);

        public static string Format(this DateTimeOffset? dateValue, string zone, string nullValue = null) =>
            dateValue.HasValue ? dateValue.Value.Format(zone) : nullValue;

        public static string Format(this DateTimeOffset dateValue, TimeZoneInfo zone = null, bool isHtml = false, bool wrap = false, bool showSeconds = false, CultureInfo culture = null) =>
            TimeZones.Format(dateValue, zone, isHtml, wrap, showSeconds, culture);

        public static string Format(this DateTimeOffset? dateValue, TimeZoneInfo zone = null, bool isHtml = false, bool wrap = false, bool showSeconds = false, CultureInfo culture = null, string nullValue = null) =>
            dateValue.HasValue ? dateValue.Value.Format(zone, isHtml, wrap, showSeconds, culture) : nullValue;

        public static string FormatDateOnly(this DateTimeOffset dateValue) =>
            TimeZones.FormatDateOnly(dateValue, dateValue.GetTimeZone());

        public static string FormatDateOnly(this DateTimeOffset? dateValue, string nullValue = null) =>
            dateValue.HasValue ? dateValue.Value.FormatDateOnly() : nullValue;

        public static string FormatDateOnly(this DateTimeOffset? dateValue, TimeZoneInfo zone, string nullValue = null) =>
            dateValue.HasValue ? dateValue.Value.FormatDateOnly(zone) : nullValue;

        public static string FormatDateOnly(this DateTimeOffset dateValue, TimeZoneInfo zone, CultureInfo culture = null) =>
            TimeZones.FormatDateOnly(dateValue, zone, culture);

        public static string FormatDateOnly(this DateTimeOffset? dateValue, TimeZoneInfo zone, CultureInfo culture, string nullValue = null) =>
            dateValue.HasValue ? dateValue.Value.FormatDateOnly(zone, culture) : nullValue;

        public static string FormatDateOnly(this DateTimeOffset dateValue, TimeZoneInfo zone, CultureInfo culture, string format) =>
            TimeZones.FormatDateOnly(dateValue, zone, culture, format);

        public static string FormatDateOnly(this DateTimeOffset? dateValue, TimeZoneInfo zone, CultureInfo culture, string format, string nullValue = null) =>
            dateValue.HasValue ? dateValue.Value.FormatDateOnly(zone, culture, format) : nullValue;

        public static string FormatTimeOnly(this DateTimeOffset dateValue) =>
            TimeZones.FormatTimeOnly(dateValue, dateValue.GetTimeZone());

        public static string FormatTimeOnly(this DateTimeOffset? dateValue, string nullValue = null) =>
            dateValue.HasValue ? dateValue.Value.FormatTimeOnly() : nullValue;

        public static string FormatTimeOnly(this DateTimeOffset dateValue, TimeZoneInfo zone) =>
            TimeZones.FormatTimeOnly(dateValue, zone);

        public static string FormatTimeOnly(this DateTimeOffset? dateValue, TimeZoneInfo zone, string nullValue = null) =>
            dateValue.HasValue ? dateValue.Value.FormatTimeOnly(zone) : nullValue;

        public static StringBuilder AppendFormatted(this StringBuilder builder, DateTimeOffset? time, TimeZoneInfo zone = null, bool isHtml = false, bool wrap = false, CultureInfo culture = null, string nullValue = null)
        {
            if (time.HasValue)
                TimeZones.Append(builder, time.Value, zone, isHtml, wrap, false, culture);
            else if (!string.IsNullOrEmpty(nullValue))
                builder.Append(nullValue);

            return builder;
        }
    }
}
