using System.Globalization;

namespace Shift.Api;

public static class DateTimeInspector
{
    public class DateTimeReport
    {
        public string Original { get; set; } = null!;

        public Dictionary<string, string> Formats { get; set; } = [];

        public DateTimeReportConversionList Conversions { get; set; } = new DateTimeReportConversionList();
    }

    public static DateTimeReport Inspect(string original, DateTimeOffset dto)
    {
        var report = new DateTimeReport();

        var zoneId = TimeZones.FindTimeZoneId(dto);

        var zoneName = zoneId != TimeZones.Ids.Utc
            ? zoneId
            : $"UTC{(dto.Offset.TotalHours >= 0 ? "+" : "")}{dto.Offset.TotalHours:0.##}";

        var result = new Dictionary<string, string>();

        report.Original = original;

        result["Value"] = dto.ToString("dddd - MMMM d, yyyy - h:mm:ss tt") + " " + zoneName;

        result["RoundTrip"] = dto.ToString("O");

        result["Offset"] = dto.Offset.ToString();

        result["TimeZone"] = zoneName;

        result["ISO8601"] = dto.ToString("yyyy-MM-ddTHH:mm:ss.fffffffK");

        result["RFC3339"] = dto.ToString("yyyy-MM-dd'T'HH:mm:ss.fffK");

        result["ShortDate"] = dto.ToString("d");

        result["LongDate"] = dto.ToString("D");

        result["ShortTime"] = dto.ToString("t");

        result["LongTime"] = dto.ToString("T");

        result["FullDateTime"] = dto.ToString("F");

        result["SortableDateTime"] = dto.ToString("s");

        result["UniversalSortable"] = dto.ToString("u");

        result["UnixTimestamp"] = dto.ToUnixTimeSeconds().ToString();

        var utc = dto.ToUniversalTime();

        result["UTC"] = utc.ToString("yyyy-MM-dd HH:mm:ss 'UTC'");

        result["UTC_ISO8601"] = utc.ToString("yyyy-MM-ddTHH:mm:ss.fffffffZ");

        result["Quarter"] = GetQuarter(dto.Month).ToString();

        result["Week"] = GetWeekOfYear(dto).ToString();

        result["Day"] = dto.DayOfYear.ToString();

        result["IsWeekend"] = (dto.DayOfWeek == DayOfWeek.Saturday || dto.DayOfWeek == DayOfWeek.Sunday).ToString();

        report.Formats = result;

        report.Conversions.Eastern = ConvertTimeZone(dto, TimeZones.Ids.Eastern);
        report.Conversions.Mountain = ConvertTimeZone(dto, TimeZones.Ids.Mountain);
        report.Conversions.Pacific = ConvertTimeZone(dto, TimeZones.Ids.Pacific);

        return report;
    }

    private static DateTimeReportConversionItem ConvertTimeZone(DateTimeOffset dto, string timeZoneId)
    {
        var item = new DateTimeReportConversionItem();

        var convertedTime = TimeZones.ConvertTime(dto, timeZoneId);

        var abbrev = TimeZones.GetAbbreviation(timeZoneId);

        var timeZoneAbbreviation = TimeZones.IsDaylightSavingTime(convertedTime, timeZoneId) ? abbrev.Daylight : abbrev.Standard;

        item.DateTime = convertedTime.ToString($"MMM d, yyyy h:mm:ss tt '{timeZoneAbbreviation}'");

        item.Offset = convertedTime.ToString("zzz");

        return item;
    }

    private static int GetWeekOfYear(DateTimeOffset date)
    {
        var calendar = CultureInfo.CurrentCulture.Calendar;
        return calendar.GetWeekOfYear(date.DateTime, CalendarWeekRule.FirstFourDayWeek, DayOfWeek.Monday);
    }

    private static int GetQuarter(int month)
    {
        return (month - 1) / 3 + 1;
    }
}
