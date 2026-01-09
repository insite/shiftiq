using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;

namespace Shift.Common
{
    public static class ObjectComparer
    {
        public static Dictionary<PropertyInfo, object> GetSnapshot<T>(T obj, ICollection<string> exclusions = null)
        {
            var state = new Dictionary<PropertyInfo, object>();
            var props = obj.GetType().GetProperties();

            foreach (var prop in props)
            {
                if (exclusions != null && exclusions.Contains(prop.Name) || !prop.CanRead || !prop.CanWrite)
                    continue;

                var type = prop.PropertyType;
                if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Nullable<>))
                    type = Nullable.GetUnderlyingType(type);

                if (type != null && (type.IsPrimitive || type == typeof(decimal) || type == typeof(string) || type == typeof(DateTime) || type == typeof(DateTimeOffset)))
                    state.Add(prop, prop.GetValue(obj));
            }

            return state;
        }

        public static DifferenceCollection Compare<T>(Dictionary<PropertyInfo, object> before, T after, ICollection<string> exclusions = null)
        {
            var differences = new DifferenceCollection();

            foreach (var prop in before.Keys)
            {
                if (exclusions != null && exclusions.Contains(prop.Name))
                    continue;

                if (GetDifference(prop.PropertyType, prop.Name, before[prop], prop.GetValue(after), out var diff))
                    differences.Add(diff);
            }

            return differences;
        }

        public static DifferenceCollection Compare<T>(T before, T after, ICollection<string> exclusions = null, ICollection<Type> additionalIncludedTypes = null)
        {
            var differences = new DifferenceCollection();
            var fields = before.GetType().GetProperties();

            foreach (var field in fields)
            {
                if (exclusions != null && exclusions.Contains(field.Name)
                    || !field.PropertyType.IsValueType && field.PropertyType != typeof(string))
                {
                    continue;
                }

                var beforeValue = TrimSeconds(field.GetValue(before));
                var afterValue = TrimSeconds(field.GetValue(after));

                if (GetDifference(field.PropertyType, field.Name, beforeValue, afterValue,
                    out var diff, additionalIncludedTypes))
                    differences.Add(diff);
            }

            return differences;
        }

        public static bool IsChanged<T>(T before, T after, ICollection<string> exclusions = null)
        {
            var fields = typeof(T).GetProperties();

            foreach (var field in fields)
            {
                if (exclusions != null && exclusions.Contains(field.Name)
                    || !field.PropertyType.IsValueType && field.PropertyType != typeof(string))
                {
                    continue;
                }

                var beforeValue = TrimSeconds(field.GetValue(before));
                var afterValue = TrimSeconds(field.GetValue(after));

                if (IsChanged(field.PropertyType, beforeValue, afterValue))
                    return true;
            }

            return false;
        }

        private static object TrimSeconds(object value)
        {
            if (value is DateTime valueDateTime)
                return new DateTime(valueDateTime.Ticks - valueDateTime.Ticks % TimeSpan.TicksPerMinute, valueDateTime.Kind);

            if (value is DateTimeOffset valueDateTimeOffset)
                return new DateTimeOffset(valueDateTimeOffset.Ticks - valueDateTimeOffset.Ticks % TimeSpan.TicksPerMinute, valueDateTimeOffset.Offset);

            return value;
        }

        public static DifferenceCollection CompareDataRow(DataRow before, DataRow after,
            ICollection<string> inclusions = null, ICollection<string> exclusions = null)
        {
            var differences = new DifferenceCollection();

            foreach (DataColumn column in before.Table.Columns)
            {
                if (inclusions != null && !inclusions.Contains(column.ColumnName))
                    continue;

                if (exclusions != null && exclusions.Contains(column.ColumnName))
                    continue;

                var beforeValue = before[column.ColumnName];
                if (beforeValue == DBNull.Value)
                    beforeValue = null;

                var afterValue = after[column.ColumnName];
                if (afterValue == DBNull.Value)
                    afterValue = null;

                if (GetDifference(column.DataType, column.ColumnName, beforeValue, afterValue, out var diff))
                    differences.Add(diff);
            }

            return differences;
        }

        private static bool GetDifference(Type type, string name, object before, object after, out Difference diff, ICollection<Type> additionalIncludedTypes = null)
        {
            var isChanged = IsChanged(type, before, after, additionalIncludedTypes);

            diff = isChanged ? new Difference(name, before, after) : null;

            return isChanged;
        }

        private static bool IsChanged(Type type, object before, object after, ICollection<Type> additionalIncludedTypes = null)
        {
            if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Nullable<>))
                type = Nullable.GetUnderlyingType(type);

            var isString = type == typeof(string);
            var isAdditionalType = additionalIncludedTypes?.FirstOrDefault(x=>x == type);

            if (type == null || !type.IsPrimitive && type != typeof(decimal) && !isString 
                && type != typeof(DateTime) 
                && type != typeof(DateTimeOffset) 
                && isAdditionalType == null)
                return false;

            var isBeforeNull = before == null || isString && string.IsNullOrEmpty((string)before);
            var isAfterNull = after == null || isString && string.IsNullOrEmpty((string)after);

            return isBeforeNull != isAfterNull
                || !isBeforeNull && !isAfterNull && (
                    !isString && !before.Equals(after)
                    || isString && !StringEquals((string)before, (string)after));
        }

        // Returns true if the text equals the value. This function is not case-sensitive. Two null
        // references compare equal to each other.
        private static bool StringEquals(string text, string value)
        {
            var culture = new System.Globalization.CultureInfo("en-US");
            return string.Compare(text, value, true, culture) == 0;
        }
    }
}