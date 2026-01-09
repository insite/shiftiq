using System;
using System.ComponentModel;

namespace Shift.Constant
{
    public static class EnumExtensions
    {
        public static string GetName(this Enum value)
        {
            var type = value.GetType();
            return Enum.GetName(type, value);
        }

        public static string GetNameLowerCase(this Enum value)
        {
            var type = value.GetType();
            return Enum.GetName(type, value).ToLower();
        }

        public static string GetNameUpperCase(this Enum value)
        {
            var type = value.GetType();
            return Enum.GetName(type, value).ToUpper();
        }

        public static string GetName<T>(this T value, T nullIf) where T : Enum
        {
            return value.Equals(nullIf) ? null : value.GetName();
        }

        public static string GetDescription(this Enum value)
        {
            var type = value.GetType();
            var name = Enum.GetName(type, value);

            if (name == null)
                return name;

            var field = type.GetField(name);
            if (field == null)
                return name;

            var attr = (DescriptionAttribute)Attribute.GetCustomAttribute(field, typeof(DescriptionAttribute));

            return attr == null ? name : attr.Description;
        }

        public static string GetIconClass(this Enum value)
        {
            var type = value.GetType();
            var name = Enum.GetName(type, value);

            if (name != null)
            {
                var field = type.GetField(name);
                if (field != null)
                {
                    var attr = (IconAttribute)Attribute.GetCustomAttribute(field, typeof(IconAttribute));
                    if (attr != null)
                        return attr.Name;
                }
            }

            return null;
        }

        public static string GetContextualClass(this Enum value)
        {
            var type = value.GetType();
            var name = Enum.GetName(type, value);

            if (name != null)
            {
                var field = type.GetField(name);
                if (field != null)
                {
                    var attr = (ContextualClassAttribute)Attribute.GetCustomAttribute(field, typeof(ContextualClassAttribute));
                    if (attr != null && !string.IsNullOrEmpty(attr.Name))
                        return attr.Name;
                }
            }

            return null;
        }
    }
}