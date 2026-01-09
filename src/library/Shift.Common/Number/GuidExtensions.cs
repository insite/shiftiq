using System;

namespace Shift.Common
{
    public static class GuidExtensions
    {
        public static Guid? NullIfEmpty(this Guid value)
        {
            return value == Guid.Empty ? (Guid?)null : value;
        }
    }
}
