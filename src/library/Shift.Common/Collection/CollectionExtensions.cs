using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;

namespace Shift.Common
{
    public static class CollectionExtensions
    {
        #region IsEmpty / IsNotEmpty

        public static bool IsNotEmpty<T>(this T[] array)
        {
            return array != null && array.Length > 0;
        }

        public static bool IsEmpty<T>(this T[] array)
        {
            return array == null || array.Length == 0;
        }

        public static bool IsNotEmpty<T>(this ICollection<T> collection)
        {
            return collection != null && collection.Count > 0;
        }

        public static bool IsEmpty<T>(this ICollection<T> collection)
        {
            return collection == null || collection.Count == 0;
        }

        public static bool IsNotEmpty<T>(this Collection<T> collection)
        {
            return collection != null && collection.Count > 0;
        }

        public static bool IsEmpty<T>(this Collection<T> collection)
        {
            return collection == null || collection.Count == 0;
        }

        public static bool IsNotEmpty<T>(this IReadOnlyList<T> list)
        {
            return list != null && list.Count > 0;
        }

        public static bool IsEmpty<T>(this IReadOnlyList<T> list)
        {
            return list == null || list.Count == 0;
        }

        public static bool IsNotEmpty<T>(this IList<T> list)
        {
            return list != null && list.Count > 0;
        }

        public static bool IsEmpty<T>(this IList<T> list)
        {
            return list == null || list.Count == 0;
        }

        public static bool IsNotEmpty(this IList list)
        {
            return list != null && list.Count > 0;
        }

        public static bool IsEmpty(this IList list)
        {
            return list == null || list.Count == 0;
        }

        public static bool IsNotEmpty(this ICollection list)
        {
            return list != null && list.Count > 0;
        }

        public static bool IsEmpty(this ICollection list)
        {
            return list == null || list.Count == 0;
        }

        public static bool IsNotEmpty(this NameValueCollection collection)
        {
            return collection != null && collection.Count > 0;
        }

        public static bool IsEmpty(this NameValueCollection collection)
        {
            return collection == null || collection.Count == 0;
        }

        public static bool IsNotEmpty(this PropertyDescriptorCollection collection)
        {
            return collection != null && collection.Count > 0;
        }

        public static bool IsEmpty(this PropertyDescriptorCollection collection)
        {
            return collection == null || collection.Count == 0;
        }

        public static bool IsNotEmpty(this SearchResultList list)
        {
            return list != null && list.GetList().Count > 0;
        }

        public static bool IsEmpty(this SearchResultList list)
        {
            return list == null || list.GetList().Count == 0;
        }

        public static bool IsNotEmpty<T>(this List<T> list)
        {
            return list != null && list.Count > 0;
        }

        public static bool IsEmpty<T>(this List<T> list)
        {
            return list == null || list.Count == 0;
        }

        public static bool IsNotEmpty<TKey, TValue>(this IDictionary<TKey, TValue> dict)
        {
            return dict != null && dict.Count > 0;
        }

        public static bool IsEmpty<TKey, TValue>(this IDictionary<TKey, TValue> dict)
        {
            return dict == null || dict.Count == 0;
        }

        public static bool IsNotEmpty<TKey, TValue>(this Dictionary<TKey, TValue> dict)
        {
            return dict != null && dict.Count > 0;
        }

        public static bool IsEmpty<TKey, TValue>(this Dictionary<TKey, TValue> dict)
        {
            return dict == null || dict.Count == 0;
        }

        public static bool IsNotEmpty<TKey, TValue>(this SortedDictionary<TKey, TValue> dict)
        {
            return dict != null && dict.Count > 0;
        }

        public static bool IsEmpty<TKey, TValue>(this SortedDictionary<TKey, TValue> dict)
        {
            return dict == null || dict.Count == 0;
        }

        public static bool IsNotEmpty(this StringDictionary dict)
        {
            return dict != null && dict.Count > 0;
        }

        public static bool IsEmpty(this StringDictionary dict)
        {
            return dict == null || dict.Count == 0;
        }

        #endregion

        #region NullIfEmpty / EmptyIfNull

        public static T[] NullIfEmpty<T>(this T[] array)
        {
            return array.IsEmpty() ? null : array;
        }

        public static T[] EmptyIfNull<T>(this T[] array)
        {
            return array == null ? new T[0] : array;
        }

        public static ICollection<T> NullIfEmpty<T>(this ICollection<T> collection)
        {
            return collection.IsEmpty() ? null : collection;
        }

        public static ICollection<T> EmptyIfNull<T>(this ICollection<T> collection)
        {
            return collection == null ? new T[0] : collection;
        }

        public static IEnumerable<T> NullIfEmpty<T>(this IEnumerable<T> enumerable)
        {
            return enumerable == null || !enumerable.Any() ? null : enumerable;
        }

        public static IEnumerable<T> EmptyIfNull<T>(this IEnumerable<T> enumerable)
        {
            return enumerable == null ? Enumerable.Empty<T>() : enumerable;
        }

        public static Collection<T> NullIfEmpty<T>(this Collection<T> collection)
        {
            return collection.IsEmpty() ? null : collection;
        }

        public static Collection<T> EmptyIfNull<T>(this Collection<T> collection)
        {
            return collection == null ? new Collection<T>() : collection;
        }

        public static IReadOnlyList<T> NullIfEmpty<T>(this IReadOnlyList<T> list)
        {
            return list.IsEmpty() ? null : list;
        }

        public static IReadOnlyList<T> EmptyIfNull<T>(this IReadOnlyList<T> list)
        {
            return list == null ? new T[0] : list;
        }

        public static IList<T> NullIfEmpty<T>(this IList<T> list)
        {
            return list.IsEmpty() ? null : list;
        }

        public static IList<T> EmptyIfNull<T>(this IList<T> list)
        {
            return list == null ? new T[0] : list;
        }

        public static IList NullIfEmpty(this IList list)
        {
            return list.IsEmpty() ? null : list;
        }

        public static IList EmptyIfNull(this IList list)
        {
            return list == null ? new object[0] : list;
        }

        public static ICollection NullIfEmpty(this ICollection list)
        {
            return list.IsEmpty() ? null : list;
        }

        public static ICollection EmptyIfNull(this ICollection list)
        {
            return list == null ? new object[0] : list;
        }

        public static NameValueCollection NullIfEmpty(this NameValueCollection collection)
        {
            return collection.IsEmpty() ? null : collection;
        }

        public static NameValueCollection EmptyIfNull(this NameValueCollection collection)
        {
            return collection == null ? new NameValueCollection() : collection;
        }

        public static PropertyDescriptorCollection NullIfEmpty(this PropertyDescriptorCollection collection)
        {
            return collection.IsEmpty() ? null : collection;
        }

        public static PropertyDescriptorCollection EmptyIfNull(this PropertyDescriptorCollection collection)
        {
            return collection == null ? PropertyDescriptorCollection.Empty : collection;
        }

        public static SearchResultList NullIfEmpty(this SearchResultList list)
        {
            return list.IsEmpty() ? null : list;
        }

        public static SearchResultList EmptyIfNull(this SearchResultList list)
        {
            return list == null ? new SearchResultList(new object[0]) : list;
        }

        public static List<T> NullIfEmpty<T>(this List<T> list)
        {
            return list.IsEmpty() ? null : list;
        }

        public static List<T> EmptyIfNull<T>(this List<T> list)
        {
            return list == null ? new List<T>() : list;
        }

        public static IDictionary<TKey, TValue> NullIfEmpty<TKey, TValue>(this IDictionary<TKey, TValue> dict)
        {
            return dict.IsEmpty() ? null : dict;
        }

        public static IDictionary<TKey, TValue> EmptyIfNull<TKey, TValue>(this IDictionary<TKey, TValue> dict)
        {
            return dict == null ? new Dictionary<TKey, TValue>() : dict;
        }

        public static Dictionary<TKey, TValue> NullIfEmpty<TKey, TValue>(this Dictionary<TKey, TValue> dict)
        {
            return dict.IsEmpty() ? null : dict;
        }

        public static Dictionary<TKey, TValue> EmptyIfNull<TKey, TValue>(this Dictionary<TKey, TValue> dict)
        {
            return dict == null ? new Dictionary<TKey, TValue>() : dict;
        }

        public static SortedDictionary<TKey, TValue> NullIfEmpty<TKey, TValue>(this SortedDictionary<TKey, TValue> dict)
        {
            return dict.IsEmpty() ? null : dict;
        }

        public static SortedDictionary<TKey, TValue> EmptyIfNull<TKey, TValue>(this SortedDictionary<TKey, TValue> dict)
        {
            return dict == null ? new SortedDictionary<TKey, TValue>() : dict;
        }

        #endregion

        #region Other

        public static T TryGetItem<T>(this IList<T> list, int index)
        {
            return index >= 0 && list.Count > 0 && index < list.Count ? list[index] : default;
        }

        #endregion
    }
}
