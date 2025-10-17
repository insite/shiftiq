using System;
using System.Collections.Generic;
using System.Linq;

using Shift.Constant;

namespace Shift.Common
{
    public static class Calendar
    {
        public static DateTime Unknown = new DateTime(1, 1, 1);
        public static DateTimeOffset UnknownUtc = new DateTimeOffset(Unknown, TimeSpan.Zero);

        /// <summary>
        /// Returns the current date plus/minus a specific number of business days. For example, if today is Friday, 
        /// then today plus one business day is the following Monday.
        /// </summary>
        public static DateTimeOffset AddBusinessDays(DateTimeOffset current, int days, IEnumerable<DateTime> holidays = null)
        {
            var sign = Math.Sign(days);
            var unsignedDays = Math.Abs(days);
            for (var i = 0; i < unsignedDays; i++)
            {
                do
                {
                    current = current.AddDays(sign);
                }
                while (current.DayOfWeek == DayOfWeek.Saturday
                    || current.DayOfWeek == DayOfWeek.Sunday
                    || (holidays != null && holidays.Contains(current.Date))
                    );
            }
            return current;
        }

        public static DateTimeOffset CalculateNextInterval(DateTimeOffset start, DateTimeOffset end, string unit, int quantity)
        {
            if (start > end)
                return start;

            var last = CalculateLastInterval(start, end, unit, quantity);

            switch (unit)
            {
                case "minute": return last.AddMinutes(quantity);
                case "hour": return last.AddHours(quantity);
                case "day": return last.AddDays(quantity);
                case "week": return last.AddDays(7 * quantity);
                case "month": return last.AddMonths(quantity);
            }

            return last;
        }

        public static DateTimeOffset CalculateLastInterval(DateTimeOffset start, DateTimeOffset end, string unit, int quantity)
        {
            if (start < end)
            {
                var elapsed = (int)(Math.Floor(CountElapsedIntervals(start, end, unit) / quantity) * quantity);

                switch (unit)
                {
                    case "minute": return start.AddMinutes(elapsed);
                    case "hour": return start.AddHours(elapsed);
                    case "day": return start.AddDays(elapsed);
                    case "week": return start.AddDays(elapsed * 7);
                    case "month": return start.AddMonths(elapsed);
                }
            }

            return start;
        }

        public static double CountElapsedIntervals(DateTimeOffset start, DateTimeOffset end, string unit)
        {
            var total = 0d;

            if (start < end)
            {
                var span = end.Subtract(start);

                switch (unit)
                {
                    case "minute": total = span.TotalMinutes; break;
                    case "hour": total = span.TotalHours; break;
                    case "day": total = span.TotalDays; break;
                    case "week": total = span.TotalDays / 7; break;
                    case "month": total = CountElapsedMonths(start, end); break;
                }
            }

            return total;
        }

        public static bool IsEmpty(DateTimeOffset? dto)
        {
            return dto == null || dto == DateTimeOffset.MinValue;
        }

        public static bool WeekdaysContain(string weekdays, DayOfWeek day)
        {
            if (!string.IsNullOrWhiteSpace(weekdays))
                return WeekdaysContain(CleanSplit(weekdays), day);
            return false;
        }

        private static string[] CleanSplit(string text)
        {
            var delimiters = new[] { ',', ';', '|', '\r', '\n' };
            var items = text?.Split(delimiters, StringSplitOptions.RemoveEmptyEntries);
            return items?.Where(x => !string.IsNullOrWhiteSpace(x)).ToArray();
        }

        public static bool WeekdaysContain(string[] weekdays, DayOfWeek day)
        {
            if (weekdays == null)
                return false;
            foreach (var weekday in weekdays)
                if (day.ToString().StartsWith(weekday, StringComparison.CurrentCultureIgnoreCase))
                    return true;
            return false;
        }

        public static int CountElapsedMonths(DateTimeOffset a, DateTimeOffset b)
        {
            DateTime before = (b < a) ? b.Date : a.Date;
            DateTime after = (a > b) ? a.Date : b.Date;
            int diff = 1;
            while (before.AddMonths(diff) <= after)
                diff++;
            return diff - 1;
        }

        public static DateTime FirstDateInMonth(DateTime date)
        {
            return new DateTime(date.Year, date.Month, 1);
        }

        public static DateTime FirstDateInWeek(DateTime date)
        {
            return date.AddDays(-(int)date.DayOfWeek);
        }

        public static string GetMonthName(int month)
        {
            string name;

            switch (month)
            {
                case 1:
                    name = "Jan";
                    break;
                case 2:
                    name = "Feb";
                    break;
                case 3:
                    name = "Mar";
                    break;
                case 4:
                    name = "Apr";
                    break;
                case 5:
                    name = "May";
                    break;
                case 6:
                    name = "Jun";
                    break;
                case 7:
                    name = "Jul";
                    break;
                case 8:
                    name = "Aug";
                    break;
                case 9:
                    name = "Sep";
                    break;
                case 10:
                    name = "Oct";
                    break;
                case 11:
                    name = "Nov";
                    break;
                case 12:
                    name = "Dec";
                    break;
                default:
                    throw new ArgumentOutOfRangeException($"Unexpected month: {month} (use a value between 1 and 12)");
            }

            return name;
        }

        public static int GetMonthNumber(string name)
        {
            if (name == null)
                throw new Exception($"Unexpected null parameter value: {nameof(name)}");

            name = name.ToLower();

            if (name.StartsWith("jan", StringComparison.OrdinalIgnoreCase))
                return 1;

            if (name.StartsWith("feb", StringComparison.OrdinalIgnoreCase))
                return 2;

            if (name.StartsWith("mar", StringComparison.OrdinalIgnoreCase))
                return 3;

            if (name.StartsWith("apr", StringComparison.OrdinalIgnoreCase))
                return 4;

            if (name.StartsWith("may", StringComparison.OrdinalIgnoreCase))
                return 5;

            if (name.StartsWith("jun", StringComparison.OrdinalIgnoreCase))
                return 6;

            if (name.StartsWith("jul", StringComparison.OrdinalIgnoreCase))
                return 7;

            if (name.StartsWith("aug", StringComparison.OrdinalIgnoreCase))
                return 8;

            if (name.StartsWith("sep", StringComparison.OrdinalIgnoreCase))
                return 9;

            if (name.StartsWith("oct", StringComparison.OrdinalIgnoreCase))
                return 10;

            if (name.StartsWith("nov", StringComparison.OrdinalIgnoreCase))
                return 11;

            if (name.StartsWith("dec", StringComparison.OrdinalIgnoreCase))
                return 12;

            throw new ArgumentOutOfRangeException($"Unexpected value: {name}");
        }

        public static CalendarSeason GetSeason()
        {
            var now = DateTime.Now;
            var today = $"{now.Month:D2}/{now.Day:D2}";

            if (today.CompareTo("03/21") >= 0 && today.CompareTo("06/20") <= 0)
                return CalendarSeason.Spring;
            else if (today.CompareTo("06/21") >= 0 && today.CompareTo("09/20") <= 0)
                return CalendarSeason.Summer;
            else if (today.CompareTo("09/21") >= 0 && today.CompareTo("12/20") <= 0)
                return CalendarSeason.Autumn;
            else
                return CalendarSeason.Winter;
        }

        public static DateTime LastDateInMonth(DateTime date)
        {
            return new DateTime(date.Year, date.Month, 1).AddMonths(1).AddDays(-1);
        }

        public static DateTime LastDateInQuarter(DateTime date)
        {
            return date.Date
                .AddDays(1 - date.Day)
                .AddMonths(3 - (date.Month - 1) % 3)
                .AddDays(-1);
        }

        public static DateTime LastDateInWeek(DateTime date)
        {
            return FirstDateInWeek(date).AddDays(6);
        }

        public static DateTimeRange GetDateTimeRange(DateRangeShortcut value)
        {
            var today = DateTime.Today.Date;

            DateTime sinceDate, beforeDate;

            if (value == DateRangeShortcut.Today)
            {
                sinceDate = today;
                beforeDate = today.AddDays(1);
            }
            else if (value == DateRangeShortcut.Yesterday)
            {
                sinceDate = today.AddDays(-1);
                beforeDate = today;
            }
            else if (value == DateRangeShortcut.ThisWeek)
            {
                sinceDate = today.AddDays(-(int)today.DayOfWeek);
                beforeDate = sinceDate.AddDays(7);
            }
            else if (value == DateRangeShortcut.LastWeek)
            {
                sinceDate = today.AddDays(-(int)today.DayOfWeek - 7);
                beforeDate = sinceDate.AddDays(7);
            }
            else if (value == DateRangeShortcut.ThisMonth)
            {
                sinceDate = today.AddDays(1 - today.Day);
                beforeDate = sinceDate.AddMonths(1);
            }
            else if (value == DateRangeShortcut.LastMonth)
            {
                sinceDate = today.AddDays(1 - today.Day).AddMonths(-1);
                beforeDate = sinceDate.AddMonths(1);
            }
            else if (value == DateRangeShortcut.ThisYear)
            {
                sinceDate = new DateTime(today.Year, 1, 1);
                beforeDate = sinceDate.AddMonths(12);
            }
            else if (value == DateRangeShortcut.LastYear)
            {
                sinceDate = new DateTime(today.Year - 1, 1, 1);
                beforeDate = sinceDate.AddMonths(12);
            }
            else
            {
                return null;
            }

            return new DateTimeRange(sinceDate, beforeDate);
        }
    }
}