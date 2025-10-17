using System;

namespace Shift.Common
{
    public static class DateTimeOffsetExtensions
    {
        public static bool IsEmpty(DateTimeOffset? dto)
            => dto == null || dto == DateTimeOffset.MinValue;
    }
}