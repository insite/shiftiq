using System;

namespace InSite.Domain
{
    internal static class StateFieldHelper
    {
        public static StateFieldType GetFieldType<T>()
        {
            return GetFieldType(typeof(T));
        }

        public static StateFieldType GetFieldType(Type type)
        {
            if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Nullable<>))
                type = Nullable.GetUnderlyingType(type);

            if (type == typeof(string))
                return StateFieldType.Text;

            if (type == typeof(bool))
                return StateFieldType.Bool;

            if (type == typeof(int))
                return StateFieldType.Int;

            if (type == typeof(decimal))
                return StateFieldType.Decimal;

            if (type == typeof(DateTimeOffset))
                return StateFieldType.DateOffset;

            if (type == typeof(DateTime))
                return StateFieldType.Date;

            if (type == typeof(Guid))
                return StateFieldType.Guid;

            throw new ArgumentException($"Unsupported type: {type}");
        }
    }
}
