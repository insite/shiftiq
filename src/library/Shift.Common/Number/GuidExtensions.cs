using System;

namespace Shift.Common
{
    public static class GuidExtensions
    {
        public static bool IsEmpty(this Guid value) =>
            value == Guid.Empty;

        public static bool IsEmpty(this Guid? value) =>
            !value.HasValue || value == Guid.Empty;

        public static bool IsNotEmpty(this Guid value) =>
            value != Guid.Empty;

        public static bool IsNotEmpty(this Guid? value) =>
            value.HasValue && value != Guid.Empty;

        public static Guid IfNullOrEmpty(this Guid value, Guid nullValue) =>
            IsEmpty(value) ? nullValue : value;

        public static Guid? IfNullOrEmpty(this Guid? value, Guid? nullValue) =>
            IsEmpty(value) ? nullValue : value;

        public static Guid IfNullOrEmpty(this Guid value, Func<Guid> nullValueFactory) =>
            IsEmpty(value) ? nullValueFactory.Invoke() : value;

        public static Guid? IfNullOrEmpty(this Guid? value, Func<Guid?> nullValueFactory) =>
            IsEmpty(value) ? nullValueFactory.Invoke() : value;

        public static Guid? NullIfEmpty(this Guid value) =>
            IsEmpty(value) ? null : (Guid?)value;

        public static Guid? NullIfEmpty(this Guid? value) =>
            IsEmpty(value) ? null : value;

        public static Guid EmptyIfNull(this Guid? value) =>
            IsEmpty(value) ? Guid.Empty : value.Value;

        public static Guid? NullIf(this Guid value, Guid nullValue) =>
            IsEmpty(value) || value == nullValue ? null : (Guid?)value;

        public static Guid? NullIf(this Guid? value, Guid nullValue) =>
            IsEmpty(value) || value == nullValue ? null : value;
    }
}
