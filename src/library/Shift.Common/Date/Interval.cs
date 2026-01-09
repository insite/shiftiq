using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace Shift.Common
{
    /// <summary>
    /// Implements a half-open interval of date/time values.
    /// </summary>
    /// <remarks>
    /// An interval contains a date/time value x if x occurs in this range:
    ///    (Effective) &#8804; x &#60; (Effective + Duration)
    /// Please note this class uses DateTimeOffset values rather than DateTime values to guard 
    /// against ambiguities that arise when the time zone offset is unspecified. In addition, it 
    /// uses the MST/MDT time zone to ensure daylight savings is taken into account. For reference,
    /// [Calgary Time Zone](https://www.timeanddate.com/time/zone/canada/calgary).
    /// </remarks>
    /// <example>
    /// A 10-minute interval starting at 9:00 contains values in the range [9:00 AM .. 9:10 PM). In
    /// other words, between 9:00 AM (inclusive) and 9:10 AM (exclusive). 
    /// </example>
    public class Interval
    {
        private const string MST = "Mountain Standard Time";

        private const string RequiredDateFormat = "yyyy-MM-dd";

        private const string RequiredTimeFormat = "HH:mm";

        /// <summary>
        /// Date the interval opens for the first time. (Year/Month/Date Format = yyyy-MM-dd)
        /// </summary>
        public string Date { get; set; }

        /// <summary>
        /// Time of day the interval opens. (24-Hour Format = HH:mm)
        /// </summary>
        public string Time { get; set; }

        /// <summary>
        /// Time zone identifier.
        /// </summary>
        public string Zone { get; set; }

        /// <summary>
        /// Length of time the interval is open. (e.g. 1h, 30m, 2d)
        /// </summary>
        public string Length { get; set; }

        /// <summary>
        /// Days of the week when the interval is open. (e.g. Sun, Mon, Tue)
        /// </summary>
        public List<string> Recurrences { get; set; }

        /// <summary>
        /// Constructs a default interval.
        /// </summary>
        /// <remarks>
        /// The default is a non-recurring 60-minute interval that opens Jan 1, 2100 at 00:00 UTC.
        /// This ensures it is in the distant future and is therefore closed by default for all 
        /// date/time values in the current century.
        /// </remarks>
        public Interval()
            : this("2100-01-01", "00:00", "UTC", "1h")
        {

        }

        public Interval(DateTime effective, string zone, string length, string weekdays = null)
            : this($"{effective:yyyy-MM-dd}", $"{effective:HH:mm}", zone, length, weekdays)
        {

        }

        public Interval(DateTimeOffset effective, string length, string weekdays = null)
            : this(effective.DateTime, Shift.Common.Base.TimeZones.GetZone(effective).Id, length, weekdays)
        {

        }

        /// <summary>
        /// Constructs an interval that opens on a specific date, at a specific time, within a 
        /// specific time zone, for a specific period of time.
        /// </summary>
        public Interval(string date, string time, string zone, string length, string weekdays = null)
        {
            Date = date;
            Time = time;
            Zone = zone;
            Length = length;
            Recurrences = new List<string>();

            if (weekdays != null)
            {
                var days = weekdays.Split(new char[] { ',', ' ' })
                    .Where(day => IsValidWeekday(day));

                foreach (var day in days)
                    Recurrences.Add(day.Trim().ToLower());
            }
        }

        /// <summary>
        /// Parses the interval length into a TimeSpan.
        /// </summary>
        public TimeSpan GetDuration()
        {
            if (string.IsNullOrWhiteSpace(Length) || Length.Length < 2)
                throw new ArgumentException("Invalid interval format");

            var pattern = @"(\d+)\s*([dhm])";

            var match = System.Text.RegularExpressions.Regex.Match(Length, pattern);

            if (!match.Success)
                throw new ArgumentException("Invalid interval format");

            string numberPart = match.Groups[1].Value;

            if (!int.TryParse(numberPart, out int number))
                throw new ArgumentException("Invalid number in length");

            string unit = match.Groups[2].Value.ToLower();

            switch (unit)
            {
                case "m":
                    return TimeSpan.FromMinutes(number);
                case "h":
                    return TimeSpan.FromHours(number);
                case "d":
                    return TimeSpan.FromDays(number);
                default:
                    throw new ArgumentException($"{unit} is not a supported unit for interval length. Please use m (minutes), h (hours), or d (days).");
            }
        }

        /// <summary>
        /// Date and time the interval opens for the first time.
        /// </summary>
        public DateTimeOffset GetEffective()
        {
            var d = DateTime.ParseExact(Date, "yyyy-MM-dd", CultureInfo.CurrentCulture);
            var t = DateTime.ParseExact(Time, "HH:mm", CultureInfo.CurrentCulture);
            var dt = new DateTime(d.Year, d.Month, d.Day, t.Hour, t.Minute, 0);
            var offset = Shift.Common.Base.TimeZones.GetOffset(dt, Zone);
            var dto = new DateTimeOffset(dt, offset);
            return dto;
        }

        /// <summary>
        /// Calculates when the interval potentially starts on a specific date.
        /// </summary>
        public DateTimeOffset GetStart(DateTimeOffset current)
        {
            // Ensure we are working in the same time zone. If the effective date is MST and the 
            // current date is MDT (or vice versa) then adjust the offset by one hour.

            var tz = Shift.Common.Base.TimeZones.GetZone(Zone);

            var effective = GetEffective();

            var offset = effective.Offset;

            if (tz.IsDaylightSavingTime(current) && !tz.IsDaylightSavingTime(effective))
            {
                offset = offset.Add(new TimeSpan(1, 0, 0));
            }
            else if (!tz.IsDaylightSavingTime(current) && tz.IsDaylightSavingTime(effective))
            {
                offset = offset.Add(new TimeSpan(-1, 0, 0));
            }

            current = current.ToOffset(offset);

            // Merge the current date with the effective time of day to determine exactly when the
            // interval opens on the current date.

            var start = new DateTimeOffset(
                current.Year, current.Month, current.Day,
                effective.Hour, effective.Minute, effective.Second,
                offset);

            return start;
        }

        /// <summary>
        /// Calculates when the interval potentially ends on a specific date.
        /// </summary>
        public DateTimeOffset GetEnd(DateTimeOffset current)
        {
            var end = GetStart(current) + GetDuration();

            return end;
        }

        /// <summary>
        /// Determines if the interval contains a specific point in time.
        /// </summary>
        public bool Contains(DateTimeOffset current)
        {
            var effective = GetEffective();

            if (current < effective)
                return false;

            var start = GetStart(current);

            var end = GetEnd(current);

            if (IsRecurring() && !RecurrencesAsDays().Contains(start.DayOfWeek))
                return false;

            return start <= current && current < end;
        }

        /// <summary>
        /// Calculates exactly when the interval opens next, following a specific date and time.
        /// </summary>
        public DateTimeOffset? NextOpenTime(DateTimeOffset current)
        {
            var effective = GetEffective();

            // The simplest case is when the interval opens for the first time in the future.

            if (current < effective)
                return effective;

            // If the interval opened in the past, and there are no recurrences, then the interval
            // never opens again in the future.

            if (!IsRecurring())
                return null;

            // Otherwise, the interval opens next at the effective time on each day within the
            // recurrence pattern. Determine the first time this occurs in the future.

            var start = GetStart(current);

            var recurrences = RecurrencesAsDays();

            var openings = Enumerable.Range(0, 8)
                .Select(i => start.AddDays(i))
                .Where(day => recurrences.Contains(day.DayOfWeek))
                .ToList();

            return openings.FirstOrDefault(opening => current < opening);
        }

        /// <summary>
        /// Calculates exactly when the interval opens next, following a specific date and time.
        /// </summary>
        public int? MinutesBeforeOpenTime(DateTimeOffset current)
        {
            var open = NextOpenTime(current);

            if (open == null)
                return null;

            return (int)Math.Ceiling((open.Value - current).TotalMinutes);
        }

        /// <summary>
        /// Determines if the interval recurs throughout the week.
        /// </summary>
        public bool IsRecurring()
            => Recurrences.Any();

        /// <summary>
        /// Parses the list of recurrences into an enumeration of weekdays.
        /// </summary>
        public List<DayOfWeek> RecurrencesAsDays()
        {
            return Recurrences.Select(r =>
            {
                switch (r.ToLower())
                {
                    case "sun":
                        return DayOfWeek.Sunday;
                    case "mon":
                        return DayOfWeek.Monday;
                    case "tue":
                        return DayOfWeek.Tuesday;
                    case "wed":
                        return DayOfWeek.Wednesday;
                    case "thu":
                        return DayOfWeek.Thursday;
                    case "fri":
                        return DayOfWeek.Friday;
                    case "sat":
                        return DayOfWeek.Saturday;
                    default:
                        throw new ArgumentException($"{r} is not a valid day of the week.");
                }
            }).ToList();
        }

        /// <summary>
        /// Determines if the interval includes a recurrence for a specific day of the week.
        /// </summary>
        public bool IncludesRecurrence(string day)
            => Recurrences.Any(r => AreEqual(r, day));

        /// <summary>
        /// Determines validation errors in the interval.
        /// </summary>
        public List<ValidationError> Validate()
        {
            var errors = new List<ValidationError>();

            if (!DateTime.TryParseExact(Date, RequiredDateFormat, CultureInfo.InvariantCulture, DateTimeStyles.None, out var date))
                errors.Add(new ValidationError { Property = nameof(Date), Summary = $"{Date} is not an expected date format. Please use {RequiredDateFormat}." });

            if (!DateTime.TryParseExact(Time, RequiredTimeFormat, CultureInfo.InvariantCulture, DateTimeStyles.None, out var time))
                errors.Add(new ValidationError { Property = nameof(Time), Summary = $"{Time} is not an expected time format. Please use {RequiredTimeFormat}." });

            if (!Shift.Common.Base.TimeZones.IsValidZone(Zone))
                errors.Add(new ValidationError { Property = nameof(Zone), Summary = $"{Zone} is not a valid time zone." });

            try
            {
                GetDuration();
            }
            catch (ArgumentException ex)
            {
                errors.Add(new ValidationError { Property = nameof(Length), Summary = ex.Message });
            }

            if (Recurrences == null)
            {
                errors.Add(new ValidationError { Property = nameof(Recurrences), Summary = "Recurrences cannot be null. Use an empty list for a non-recurring interval." });
            }
            else if (Recurrences.Any())
            {
                var effective = GetEffective();
                if (!RecurrencesAsDays().Contains(effective.DayOfWeek))
                    errors.Add(new ValidationError { Property = nameof(Recurrences), Summary = $"Recurrences for this interval must include {effective.DayOfWeek} because {Date} occurs on this day of the week." });
            }

            return errors;
        }

        /// <summary>
        /// Determines if the interval is valid.
        /// </summary>
        public bool IsValid()
            => !Validate().Any();

        private bool IsValidWeekday(string day)
        {
            switch (day.ToLower())
            {
                case "sun":
                case "mon":
                case "tue":
                case "wed":
                case "thu":
                case "fri":
                case "sat":
                    return true;
                default:
                    return false;
            }
        }

        /// <summary>
        /// Determines if two strings are equal, ignoring case.
        /// </summary>
        private bool AreEqual(string x, string y)
            => string.Compare(x, y, StringComparison.OrdinalIgnoreCase) == 0;
    }
}