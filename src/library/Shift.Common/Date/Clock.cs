using System;

namespace Shift.Common
{
    public static class Clock
    {
        private static readonly DateTimeOffset _appStartTime = DateTimeOffset.UtcNow;

        public static TimeSpan GetApplicationUptime()
        {
            return DateTimeOffset.UtcNow - _appStartTime;
        }

        public static DateTimeOffset GetApplicationStartTime()
        {
            return _appStartTime;
        }

        public static TimeSpan GetProcessUptime()
        {
            var startTime = GetProcessStartTime();
            return DateTimeOffset.UtcNow - startTime;
        }

        public static DateTimeOffset GetProcessStartTime()
        {
            return System.Diagnostics.Process.GetCurrentProcess().StartTime;
        }

        public static TimeSpan TimeEstimated(long totalCount, long currentCount, DateTime started)
        {
            var ticksEstimated = 0L;

            if (currentCount > 0)
            {
                double tickDelta = DateTime.UtcNow.Ticks - started.Ticks;
                ticksEstimated = (long)(tickDelta / currentCount * totalCount);
            }

            return new TimeSpan(ticksEstimated);
        }

        public static TimeSpan TimeRemaining(long totalCount, long currentCount, DateTime started)
        {
            var ticksRemaining = 0L;

            if (currentCount > 0)
            {
                double tickDelta = DateTime.UtcNow.Ticks - started.Ticks;
                ticksRemaining = (long)(tickDelta / currentCount * (totalCount - currentCount));
            }

            return new TimeSpan(ticksRemaining);
        }

        public static DateTime Trim(DateTime value, long ticks = TimeSpan.TicksPerSecond) =>
            value.AddTicks(-(value.Ticks % ticks));

        public static DateTimeOffset Trim(DateTimeOffset value, long ticks = TimeSpan.TicksPerSecond) =>
            value.AddTicks(-(value.Ticks % ticks));

        public static int GetAge(DateTime birthdate) => GetAge(birthdate, DateTime.UtcNow);

        public static int GetAge(DateTime birthdate, DateTime now)
        {
            if (now <= birthdate)
                return 0;

            var age = now.Year - birthdate.Year;
            return now.Month < birthdate.Month || now.Month == birthdate.Month && now.Day < birthdate.Day
                ? age - 1
                : age;
        }

        public static DateTimeOffset ToDateTimeOffset(this DateTime value, TimeZoneInfo tz) =>
            new DateTimeOffset(value, tz.GetUtcOffset(value));

        public static DateTimeOffset? ToDateTimeOffset(this DateTime? value, TimeZoneInfo tz) =>
            value.HasValue ? value.Value.ToDateTimeOffset(tz) : (DateTimeOffset?)null;

        public static DateTimeOffset ToDateTimeOffset(this DateTime value, TimeSpan ts) =>
            new DateTimeOffset(value, ts);

        public static DateTimeOffset? ToDateTimeOffset(this DateTime? value, TimeSpan ts) =>
            value.HasValue ? value.Value.ToDateTimeOffset(ts) : (DateTimeOffset?)null;

        public static DateTime From(this DateTime value, long ticks) => Trim(value, ticks);

        public static DateTime? From(this DateTime? value, long ticks) => value.HasValue ? value.Value.From(ticks) : (DateTime?)null;

        public static DateTime Thru(this DateTime value, long ticks) => Trim(value, ticks).AddTicks(ticks - 1);

        public static DateTime? Thru(this DateTime? value, long ticks) => value.HasValue ? value.Value.Thru(ticks) : (DateTime?)null;

        public static DateTime FromMinute(this DateTime value) => From(value, TimeSpan.TicksPerMinute);

        public static DateTime? FromMinute(this DateTime? value) => From(value, TimeSpan.TicksPerMinute);

        public static DateTime ThruMinute(this DateTime value) => Thru(value, TimeSpan.TicksPerMinute);

        public static DateTime? ThruMinute(this DateTime? value) => Thru(value, TimeSpan.TicksPerMinute);

        public static DateTime FromDay(this DateTime value) => From(value, TimeSpan.TicksPerDay);

        public static DateTime? FromDay(this DateTime? value) => From(value, TimeSpan.TicksPerDay);

        public static DateTime ThruDay(this DateTime value) => Thru(value, TimeSpan.TicksPerDay);

        public static DateTime? ThruDay(this DateTime? value) => Thru(value, TimeSpan.TicksPerDay);

        public static DateTimeOffset From(this DateTimeOffset value, long ticks) => Trim(value, ticks);

        public static DateTimeOffset? From(this DateTimeOffset? value, long ticks) => value.HasValue ? value.Value.From(ticks) : (DateTimeOffset?)null;

        public static DateTimeOffset Thru(this DateTimeOffset value, long ticks) => Trim(value, ticks).AddTicks(ticks - 1);

        public static DateTimeOffset? Thru(this DateTimeOffset? value, long ticks) => value.HasValue ? value.Value.Thru(ticks) : (DateTimeOffset?)null;

        public static DateTimeOffset FromMinute(this DateTimeOffset value) => From(value, TimeSpan.TicksPerMinute);

        public static DateTimeOffset? FromMinute(this DateTimeOffset? value) => From(value, TimeSpan.TicksPerMinute);

        public static DateTimeOffset ThruMinute(this DateTimeOffset value) => Thru(value, TimeSpan.TicksPerMinute);

        public static DateTimeOffset? ThruMinute(this DateTimeOffset? value) => Thru(value, TimeSpan.TicksPerMinute);

        public static DateTimeOffset FromDay(this DateTimeOffset value) => From(value, TimeSpan.TicksPerDay);

        public static DateTimeOffset? FromDay(this DateTimeOffset? value) => From(value, TimeSpan.TicksPerDay);

        public static DateTimeOffset ThruDay(this DateTimeOffset value) => Thru(value, TimeSpan.TicksPerDay);

        public static DateTimeOffset? ThruDay(this DateTimeOffset? value) => Thru(value, TimeSpan.TicksPerDay);

        #region Unix

        private static readonly DateTime _unixEpoch = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);

        public static long ToUnixTimestamp(DateTime value)
        {
            return (long)(value.ToUniversalTime() - _unixEpoch).TotalSeconds;
        }

        public static long ToUnixMilliseconds(DateTime value)
        {
            return (long)(value.ToUniversalTime() - _unixEpoch).TotalMilliseconds;
        }

        public static DateTime FromUnixTimestamp(long value)
        {
            return _unixEpoch.AddSeconds(value);
        }

        #endregion
    }
}
