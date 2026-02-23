using System.Globalization;

namespace Shift.Api;

public static class DateTimeInspector
{
    public class DateTimeReport
    {
        private Dictionary<string, string> _preferredZones = new()
        {
            { "Eastern", "Eastern Standard Time" },
            { "Mountain", "Mountain Standard Time" },
            { "Pacific", "Pacific Standard Time" }
        };

        public Dictionary<string, string> GetPreferredZones() => _preferredZones;

        public string Original { get; set; } = null!;

        public Dictionary<string, string> Formats { get; set; } = [];

        public DateTimeReportConversionList Conversions { get; set; } = new DateTimeReportConversionList();
    }

    public static DateTimeReport Inspect(string original, DateTimeOffset dto)
    {
        var report = new DateTimeReport();

        var zones = report.GetPreferredZones();

        var zoneName = GetTimeZoneNameFromOffset(dto.Offset, dto.DateTime, [.. zones.Values]);

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

        report.Conversions.Eastern = ConvertTimeZone(dto, "Eastern Standard Time", "Eastern");
        report.Conversions.Mountain = ConvertTimeZone(dto, "Mountain Standard Time", "Mountain");
        report.Conversions.Pacific = ConvertTimeZone(dto, "Pacific Standard Time", "Pacific");

        return report;
    }

    private static DateTimeReportConversionItem ConvertTimeZone(DateTimeOffset dto, string timeZoneId, string timeZoneSlug)
    {
        var item = new DateTimeReportConversionItem();

        var timeZoneInfo = TimeZoneInfo.FindSystemTimeZoneById(timeZoneId);

        var convertedTime = TimeZoneInfo.ConvertTime(dto, timeZoneInfo);

        var timeZoneAbbreviation = GetTimeZoneAbbreviation(timeZoneSlug, timeZoneInfo.IsDaylightSavingTime(convertedTime));

        item.DateTime = convertedTime.ToString($"MMM d, yyyy h:mm:ss tt '{timeZoneAbbreviation}'");

        item.Offset = convertedTime.ToString("zzz");

        return item;
    }

    private static string GetTimeZoneAbbreviation(string timeZoneSlug, bool isDaylightSaving)
    {
        return timeZoneSlug switch
        {
            "Eastern" => isDaylightSaving ? "EDT" : "EST",
            "Central" => isDaylightSaving ? "CDT" : "CST",
            "Mountain" => isDaylightSaving ? "MDT" : "MST",
            "Pacific" => isDaylightSaving ? "PDT" : "PST",
            "Alaska" => isDaylightSaving ? "AKDT" : "AKST",
            "Hawaii" => "HST", // Hawaii doesn't observe DST
            "Atlantic" => isDaylightSaving ? "ADT" : "AST",
            "Newfoundland" => isDaylightSaving ? "NDT" : "NST",
            _ => ""
        };
    }

    private static string GetTimeZoneNameFromOffset(TimeSpan offset, DateTime dateTime, string[] zones)
    {
        var tz = TimeZoneInfo.GetSystemTimeZones().Where(x => zones.Contains(x.Id)).ToArray();

        var name = TryGetTimeZoneNameFromOffset(offset, dateTime, tz);

        if (name == null)
        {
            tz = [.. TimeZoneInfo.GetSystemTimeZones()];

            name = TryGetTimeZoneNameFromOffset(offset, dateTime, tz);
        }

        if (name == null)
        {
            name = $"UTC{(offset.TotalHours >= 0 ? "+" : "")}{offset.TotalHours:0.##}";
        }

        return name;
    }

    private static string? TryGetTimeZoneNameFromOffset(TimeSpan offset, DateTime dateTime, TimeZoneInfo[] zones)
    {
        // Try to find a matching time zone based on the offset

        foreach (var timeZone in zones)
        {
            var offsetAtDateTime = timeZone.GetUtcOffset(dateTime);
            if (offsetAtDateTime == offset)
            {
                // Check if it's a North American time zone we recognize
                var isDst = timeZone.IsDaylightSavingTime(dateTime);

                return timeZone.Id switch
                {
                    "Eastern Standard Time" => isDst ? "Eastern Daylight Time" : "Eastern Standard Time",
                    "Central Standard Time" => isDst ? "Central Daylight Time" : "Central Standard Time",
                    "Mountain Standard Time" => isDst ? "Mountain Daylight Time" : "Mountain Standard Time",
                    "Pacific Standard Time" => isDst ? "Pacific Daylight Time" : "Pacific Standard Time",
                    "Alaskan Standard Time" => isDst ? "Alaska Daylight Time" : "Alaska Standard Time",
                    "Hawaiian Standard Time" => "Hawaii Standard Time",
                    "Atlantic Standard Time" => isDst ? "Atlantic Daylight Time" : "Atlantic Standard Time",
                    "Newfoundland Standard Time" => isDst ? "Newfoundland Daylight Time" : "Newfoundland Standard Time",
                    _ => timeZone.DisplayName
                };
            }
        }

        return null;
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
