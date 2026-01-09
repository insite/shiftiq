using System;
using System.Collections;
using System.Collections.Generic;

using Shift.Common;

namespace InSite.Persistence
{
    [Serializable]
    public class QuestionDisplayFilter : IReadOnlyCollection<QuestionDisplayFilterItem>
    {
        #region Constants

        private static readonly StringComparer KeyStringComparer = StringComparer.OrdinalIgnoreCase;

        #endregion

        #region Properties

        public int Count => _items.Count;

        public QuestionDisplayFilterItem this[string tag]
        {
            get
            {
                if (string.IsNullOrEmpty(tag))
                    return null;

                if (_items.TryGetValue(tag, out var item))
                    return item;

                var parts = tag.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                foreach (var part in parts)
                {
                    var trimmedPart = part.Trim();
                    if (!string.IsNullOrEmpty(trimmedPart) && _items.TryGetValue(trimmedPart, out var partItem))
                        return partItem;
                }

                return null;
            }
        }

        #endregion

        #region Fields

        private Dictionary<string, QuestionDisplayFilterItem> _items;

        #endregion

        #region Construction

        private QuestionDisplayFilter()
        {
            _items = new Dictionary<string, QuestionDisplayFilterItem>(KeyStringComparer);
        }

        #endregion

        #region Methods

        public bool Contains(string tag) => _items.ContainsKey(tag);

        public static QuestionDisplayFilter Parse(string input)
        {
            var filter = new QuestionDisplayFilter();

            var items = ParseItems(input);
            var excludeTags = new HashSet<string>(KeyStringComparer);

            foreach (var item in items)
            {
                if (excludeTags.Contains(item.Tag))
                    continue;

                if (filter._items.ContainsKey(item.Tag))
                {
                    filter._items.Remove(item.Tag);
                    excludeTags.Add(item.Tag);
                    continue;
                }

                filter._items.Add(item.Tag, item);
            }

            return filter;
        }

        public static void Validate(string input)
        {
            if (string.IsNullOrEmpty(input))
                return;

            var items = ParseItems(input);
            var tags = new HashSet<string>(KeyStringComparer);

            foreach (var item in items)
            {
                if (tags.Contains(item.Tag))
                    throw new ApplicationError("Duplicate tag found: " + item.Tag);

                tags.Add(item.Tag);
            }
        }

        #endregion

        #region Methods (IEnumerable)

        public IEnumerator<QuestionDisplayFilterItem> GetEnumerator() => _items.Values.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        #endregion

        #region Methods (helpers)

        private static QuestionDisplayFilterItem[] ParseItems(string input)
        {
            var result = new List<QuestionDisplayFilterItem>();

            if (!string.IsNullOrEmpty(input))
            {
                foreach (var inputItem in input.Split(','))
                    if (QuestionDisplayFilterItem.TryParse(inputItem, out var item))
                        result.Add(item);
            }

            return result.ToArray();
        }

        #endregion
    }
}