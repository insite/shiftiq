using System.Collections.Generic;
using System.Linq;

using Shift.Constant;

namespace Shift.Common
{
    public static class IHasVersionControlExtensions
    {
        public static bool IsSingleVersion<T>(this T obj) where T : IHasVersionControl<T>
        {
            return obj.NextVersion == null && obj.PreviousVersion == null;
        }

        public static bool IsFirstVersion<T>(this T obj) where T : IHasVersionControl<T>
        {
            return obj.PreviousVersion == null;
        }

        public static bool IsIntermediateVersion<T>(this T obj) where T : IHasVersionControl<T>
        {
            return obj.NextVersion != null && obj.PreviousVersion != null;
        }

        public static bool IsLastVersion<T>(this T obj) where T : IHasVersionControl<T>
        {
            return obj.NextVersion == null;
        }

        public static int GetVersionNumber<T>(this T obj) where T : IHasVersionControl<T>
        {
            return obj.PreviousVersion != null
                ? 1 + GetVersionNumber(obj.PreviousVersion)
                : obj.FirstPublished.HasValue
                    ? 1
                    : 0;
        }

        public static int GetSequence<T>(this IList<T> list, T obj) where T : IHasVersionControl<T>
        {
            return obj.NextVersion == null ? 1 + list.IndexOf(obj) : GetSequence(list, obj.NextVersion);
        }

        public static void RestoreNextVersionReferences<T>(this T obj) where T : IHasVersionControl<T>
        {
            var current = obj;

            while (current.PreviousVersion != null && current.PreviousVersion.NextVersion == null)
            {
                current.PreviousVersion.NextVersion = current;
                current = current.PreviousVersion;
            }
        }

        public static T GetFirstVersion<T>(this T obj) where T : IHasVersionControl<T>
        {
            var current = obj;

            while (current.PreviousVersion != null)
                current = current.PreviousVersion;

            return current;
        }

        public static T GetLastVersion<T>(this T obj) where T : IHasVersionControl<T>
        {
            var current = obj;

            while (current.NextVersion != null)
                current = current.NextVersion;

            return current;
        }

        public static IEnumerable<T> EnumeratePreviousVersions<T>(this T obj) where T : IHasVersionControl<T>
        {
            var current = obj.PreviousVersion;

            while (current != null)
            {
                yield return current;

                current = current.PreviousVersion;
            }
        }

        public static IEnumerable<T> EnumerateNextVersions<T>(this T obj) where T : IHasVersionControl<T>
        {
            var current = obj.NextVersion;

            while (current != null)
            {
                yield return current;

                current = current.NextVersion;
            }
        }

        public static IEnumerable<T> EnumerateAllVersions<T>(this T obj, SortOrder orderBy = SortOrder.Ascending) where T : IHasVersionControl<T>
        {
            if (orderBy == SortOrder.Ascending)
            {
                var first = obj.GetFirstVersion();

                return (new[] { first }).Concat(first.EnumerateNextVersions());
            }
            else
            {
                var last = obj.GetLastVersion();

                return (new[] { last }).Concat(last.EnumeratePreviousVersions());
            }
        }

        public static void RemoveVersion<T>(this IList<T> list, T obj) where T : IHasVersionControl<T>
        {
            var hasNext = obj.NextVersion != null;
            var hasPrev = obj.PreviousVersion != null;

            if (!hasNext && !hasPrev)
            {
                list.Remove(obj);
            }
            else
            {
                var next = obj.NextVersion;
                var prev = obj.PreviousVersion;

                obj.NextVersion = default;
                obj.PreviousVersion = default;

                if (hasNext)
                    next.PreviousVersion = prev;

                if (hasPrev)
                {
                    prev.NextVersion = next;

                    if (!hasNext)
                    {
                        var index = list.IndexOf(obj);

                        list[index] = prev;
                    }
                }
            }
        }

        public static void SetNewVersion<T>(this IList<T> list, T topObj, T newObj) where T : IHasVersionControl<T>
        {
            if (topObj.NextVersion != null)
                throw ApplicationError.Create("Invalid top version");

            var index = list.IndexOf(topObj);
            list[index] = newObj;
            newObj.PreviousVersion = topObj;
            topObj.NextVersion = newObj;
        }
    }
}
