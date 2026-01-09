using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace Shift.Common
{
    [SuppressMessage("NDepend", "ND3101:DontUseSystemRandomForSecurityPurposes", Scope = "method", Justification = "Random number generation is not security-sensitive here, therefore weak psuedo-random numbers are permitted.")]
    public static class ListExtensions
    {
        private static readonly Random rnd = new Random();

        public static void AddSorted<T>(this List<T> list, T item) where T : IComparable<T>
        {
            if (list.Count == 0)
                list.Add(item);
            else if (list[list.Count - 1].CompareTo(item) <= 0)
                list.Add(item);
            else if (list[0].CompareTo(item) >= 0)
                list.Insert(0, item);
            else
            {
                var index = list.BinarySearch(item);

                if (index < 0)
                    index = ~index;

                list.Insert(index, item);
            }
        }

        public static int BinaryIndexSearch<T>(this List<T> list, Func<T, int> compare)
        {
            if (list.Count == 0)
                return ~0;

            var start = 0;
            var end = list.Count - 1;

            while (start <= end)
            {
                var index = start + (end - start >> 1);
                var diff = compare(list[index]);

                if (diff == 0)
                    return index;

                if (diff < 0)
                    start = index + 1;
                else
                    end = index - 1;
            }

            return ~start;
        }

        public static T BinaryItemSearch<T>(this List<T> list, Func<T, int> compare)
        {
            var index = BinaryIndexSearch(list, compare);

            return index >= 0 ? list[index] : default;
        }

        public static void Shuffle<T>(this IList<T> list)
        {
            var n = list.Count;
            while (n > 1)
            {
                n--;
                var k = rnd.Next(n + 1);
                var value = list[k];
                list[k] = list[n];
                list[n] = value;
            }
        }

        public static void Shuffle<T>(this IList<T> list, int index, int count)
        {
            if (index < 0 || index >= list.Count)
                throw new ArgumentOutOfRangeException(nameof(index));

            if (count <= 0 || count > list.Count - index)
                throw new ArgumentOutOfRangeException(nameof(count));

            while (count > 1)
            {
                count--;

                var n = index + count;
                var k = index + rnd.Next(count + 1);

                var value = list[k];
                list[k] = list[n];
                list[n] = value;
            }
        }
    }
}
