using System;
using System.Collections.Generic;
using System.Linq;

namespace Shift.Common.Base
{
    public class Calendar
    {
        /// <remarks>
        /// Calculates the current date plus-or-minus a specific number of business days. For 
        /// example, if today is Friday, then today plus one business day is the following Monday.
        /// </remarks>
        public DateTimeOffset AddBusinessDays(DateTimeOffset when, int days, IEnumerable<DateTimeOffset> holidays = null)
        {
            var sign = Math.Sign(days);
            var unsignedDays = Math.Abs(days);
            for (var i = 0; i < unsignedDays; i++)
            {
                do
                {
                    when = when.AddDays(sign);
                }
                while (when.DayOfWeek == DayOfWeek.Saturday
                    || when.DayOfWeek == DayOfWeek.Sunday
                    || (holidays != null && holidays.Any(holiday => holiday == when.Date)));
            }
            return when;
        }

        public DateTimeOffset GetStartOfMonth(DateTimeOffset when)
        {
            return new DateTimeOffset(when.Year, when.Month, 1, 0, 0, 0, TimeSpan.Zero);
        }

        public DateTimeOffset GetStartOfWeek(DateTimeOffset when)
        {
            var sunday = when.AddDays(-(int)when.DayOfWeek);
            return new DateTimeOffset(sunday.Year, sunday.Month, sunday.Day, 0, 0, 0, TimeSpan.Zero);
        }

        public DateTimeOffset GetStartOfYear(DateTimeOffset when)
        {
            return new DateTimeOffset(when.Year, 1, 1, 0, 0, 0, TimeSpan.Zero);
        }

        public Season GetSeason(DateTimeOffset when)
        {
            var today = $"{when.Month:D2}/{when.Day:D2}";

            if (0 <= today.CompareTo("03/21") && today.CompareTo("06/20") <= 0)
                return Season.Spring;

            else if (0 <= today.CompareTo("06/21") && today.CompareTo("09/20") <= 0)
                return Season.Summer;

            else if (0 <= today.CompareTo("09/21") && today.CompareTo("12/20") <= 0)
                return Season.Autumn;

            else
                return Season.Winter;
        }
    }
}