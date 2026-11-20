using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;

namespace Shift.Common
{
    public sealed class QuerySort : IEnumerable<QuerySort.Item>
    {
        public sealed class Item
        {
            #region Properties

            public string Field { get; set; }

            public bool IsDescending { get; set; }

            private static readonly char[] Separators = { ' ', '\t' };

            #endregion

            #region Construction

            public Item(string field, bool descending)
            {
                Field = field.Trim().NullIfEmpty()
                    ?? throw new ArgumentException("Field is required", nameof(field));
                IsDescending = descending;
            }

            public static Item Parse(string expression)
            {
                return TryParse(expression, out var item)
                    ? item
                    : throw ApplicationError.Create("Invalid sort item: '{0}'", expression);
            }

            public static bool TryParse(string expression, out Item item)
            {
                item = null;

                if (string.IsNullOrWhiteSpace(expression))
                    return false;

                var parts = expression.Split(Separators, StringSplitOptions.RemoveEmptyEntries);
                if (parts.Length != 1 && parts.Length != 2)
                    return false;

                var field = parts[0].Trim();
                if (field.Length == 0)
                    return false;

                var isDescending = false;

                if (parts.Length == 2)
                {
                    if (IsDirection(parts[1], "desc", "descending"))
                        isDescending = true;
                    else if (!IsDirection(parts[1], "asc", "ascending"))
                        return false;
                }

                item = new Item(field, isDescending);

                return true;

                bool IsDirection(string value, string match1, string match2)
                {
                    return string.Equals(value, match1, StringComparison.OrdinalIgnoreCase)
                        || string.Equals(value, match1, StringComparison.OrdinalIgnoreCase);
                }
            }

            #endregion

            #region Other

            public string ToString(bool explicitAsc)
            {
                return IsDescending
                    ? Field + " DESC"
                    : explicitAsc
                        ? Field + " ASC"
                        : Field;
            }

            public override string ToString()
            {
                return ToString(false);
            }

            #endregion
        }

        #region Properties

        public int Count => _items.Count;

        public bool IsEmpty => _items.Count == 0;

        [IndexerName("SortIndexer")]
        public Item this[int index]
        {
            get => _items[index];
            set => _items[index] = value ?? throw new ArgumentNullException(nameof(value));
        }

        #endregion

        #region Fields

        private static readonly char[] Separators = { ',', ';' };

        private readonly List<Item> _items;

        #endregion

        #region Construction

        public QuerySort()
        {
            _items = new List<Item>();
        }

        #endregion

        #region Methods (parse)

        public static QuerySort Parse(string expression)
        {
            return TryParse(expression, out var sort)
                ? sort
                : throw ApplicationError.Create("Invalid sort expression: '{0}'", expression);
        }

        public static bool TryParse(string expression, out QuerySort sort)
        {
            sort = null;

            if (string.IsNullOrWhiteSpace(expression))
            {
                sort = new QuerySort();
                return true;
            }

            var result = new QuerySort();
            var parts = expression.Split(Separators, StringSplitOptions.RemoveEmptyEntries);

            foreach (var part in parts)
            {
                if (string.IsNullOrWhiteSpace(part))
                    continue;

                if (!Item.TryParse(part, out var item))
                    return false;

                result._items.Add(item);
            }

            sort = result;

            return true;
        }

        #endregion

        #region Methods (item management)

        public Item Add(string field) => Add(field, false);

        public Item Add(string field, bool isDescending)
        {
            var item = new Item(field, isDescending);
            _items.Add(item);
            return item;
        }

        public void Add(Item item) =>
            _items.Add(item ?? throw new ArgumentNullException(nameof(item)));

        public void RemoveAt(int index) => _items.RemoveAt(index);

        public bool Remove(string field) =>
            _items.RemoveAll(x => string.Equals(x.Field, field, StringComparison.OrdinalIgnoreCase)) > 0;

        public void Clear() => _items.Clear();

        #endregion

        #region Methods (search)

        public int IndexOf(string field) =>
            _items.FindIndex(x => string.Equals(x.Field, field, StringComparison.OrdinalIgnoreCase));

        public int IndexOf(Item item) => _items.IndexOf(item);

        public bool Contains(string field) => IndexOf(field) >= 0;

        public bool Contains(Item item) => _items.Contains(item);

        public Item Find(string field)
        {
            var index = IndexOf(field);
            return index < 0 ? null : _items[index];
        }

        #endregion

        #region Methods (other)

        public string ToString(bool explicitAsc) =>
            string.Join(",", _items.Select(x => x.ToString(explicitAsc)));

        public override string ToString() => ToString(false);

        public IEnumerator<Item> GetEnumerator() => _items.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        #endregion
    }
}
