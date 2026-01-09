using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Shift.Common
{
    [Serializable]
    public class ListItemArray : IReadOnlyListItemArray
    {
        private readonly List<ListItem> _items;
        public List<ListItem> Items => _items;

        public int? TotalCount { get; set; }

        public ListItemArray()
        {
            _items = new List<ListItem>();
        }

        public ListItemArray(IEnumerable<ListItem> items)
        {
            _items = items == null ? new List<ListItem>() : new List<ListItem>(items);
        }

        public ListItemArray(IEnumerable<string> items)
        {
            _items = new List<ListItem>();
            foreach (var item in items)
                Add(item);
        }

        public void Add(ListItem item)
        {
            _items.Add(item);
        }

        public ListItem Add(string value, string text, string description = null)
        {
            var item = new ListItem { Text = text, Value = value, Description = description };

            _items.Add(item);

            return item;
        }

        public ListItem FindByValue(string value)
        {
            return _items.FirstOrDefault(x => x.Value == value);
        }

        public bool ContainsValue(string value)
        {
            return _items.Any(x => x.Value == value);
        }

        public ListItem Add(string value)
        {
            var result = new ListItem { Text = value, Value = value };

            _items.Add(result);

            return result;
        }

        public IEnumerator GetEnumerator()
        {
            return _items.GetEnumerator();
        }

        IEnumerator<ListItem> IEnumerable<ListItem>.GetEnumerator()
        {
            return _items.GetEnumerator();
        }
    }

    public interface IReadOnlyListItemArray : IEnumerable<ListItem>
    {
        int? TotalCount { get; }

        ListItem FindByValue(string value);

        bool ContainsValue(string value);
    }
}
