using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;

namespace Shift.Common
{
    public static class DictionaryExtensions
    {
        public static TValue GetOrDefault<TKey, TValue>(this Dictionary<TKey, TValue> dict, TKey key) =>
            dict.TryGetValue(key, out var value) ? value : default;

        public static TValue GetOrDefault<TKey, TValue>(this Dictionary<TKey, TValue> dict, TKey key, TValue defaultValue) =>
            dict.TryGetValue(key, out var value) ? value : defaultValue;

        public static TValue GetOrDefault<TKey, TValue>(this Dictionary<TKey, TValue> dict, TKey key, Func<TValue> defaultFactory) =>
            dict.TryGetValue(key, out var value) ? value : defaultFactory();

        public static TValue GetOrDefault<TKey, TValue>(this IDictionary<TKey, TValue> dict, TKey key) =>
            dict.TryGetValue(key, out var value) ? value : default;

        public static TValue GetOrDefault<TKey, TValue>(this IDictionary<TKey, TValue> dict, TKey key, TValue defaultValue) =>
            dict.TryGetValue(key, out var value) ? value : defaultValue;

        public static TValue GetOrDefault<TKey, TValue>(this IDictionary<TKey, TValue> dict, TKey key, Func<TValue> defaultFactory) =>
            dict.TryGetValue(key, out var value) ? value : defaultFactory();

        public static TValue GetOrDefault<TKey, TValue>(this IReadOnlyDictionary<TKey, TValue> dict, TKey key) =>
            dict.TryGetValue(key, out var value) ? value : default;

        public static TValue GetOrDefault<TKey, TValue>(this IReadOnlyDictionary<TKey, TValue> dict, TKey key, TValue defaultValue) =>
            dict.TryGetValue(key, out var value) ? value : defaultValue;

        public static TValue GetOrDefault<TKey, TValue>(this IReadOnlyDictionary<TKey, TValue> dict, TKey key, Func<TValue> defaultFactory) =>
            dict.TryGetValue(key, out var value) ? value : defaultFactory();

        public static TValue GetOrDefault<TKey, TValue>(this ReadOnlyDictionary<TKey, TValue> dict, TKey key) =>
            dict.TryGetValue(key, out var value) ? value : default;

        public static TValue GetOrDefault<TKey, TValue>(this ReadOnlyDictionary<TKey, TValue> dict, TKey key, TValue defaultValue) =>
            dict.TryGetValue(key, out var value) ? value : defaultValue;

        public static TValue GetOrDefault<TKey, TValue>(this ReadOnlyDictionary<TKey, TValue> dict, TKey key, Func<TValue> defaultFactory) =>
            dict.TryGetValue(key, out var value) ? value : defaultFactory();

        public static TValue GetOrAdd<TKey, TValue>(this IDictionary<TKey, TValue> dict, TKey key, TValue value) =>
            dict.TryGetValue(key, out var existValue) ? existValue : (dict[key] = value);

        public static TValue GetOrAdd<TKey, TValue>(this IDictionary<TKey, TValue> dict, TKey key, Func<TValue> valueFactory) =>
            dict.TryGetValue(key, out var existValue) ? existValue : (dict[key] = valueFactory());

        public static Dictionary<string, string> ToDictionary(this NameValueCollection col)
        {
            var result = new Dictionary<string, string>();

            foreach (var k in col.AllKeys)
                result.Add(k, col[k]);

            return result;
        }
    }
}
