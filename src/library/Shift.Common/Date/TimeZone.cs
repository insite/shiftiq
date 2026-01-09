using System;

namespace Shift.Common
{
    public static class TimeZone
    {
        public static DateTime ConvertTime(DateTime sourceTime, TimeZoneInfo sourceZone, TimeZoneInfo targetZone)
        {
            return TimeZoneInfo.ConvertTime(sourceTime, sourceZone, targetZone);
        }
    }
}