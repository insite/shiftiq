using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

using InSite.Domain.Banks;

using Shift.Common;

namespace InSite.UI.Admin.Assessments.Questions.Controls
{
    public partial class QuestionOrderingDetails
    {
        private delegate void ItemEventHandler<T>(object sender, ItemEventArgs<T> args) where T : BaseItem;

        private class ItemEventArgs<T> : EventArgs where T : BaseItem
        {
            public T Item { get; }

            public ItemEventArgs(T item)
            {
                Item = item;
            }
        }

        [Serializable]
        private class ControlData
        {
            public Guid BankId { get; }
            public int BankAsset { get; }
            public Guid? QuestionId { get; }

            public ControlData(BankState bank)
            {
                BankId = bank.Identifier;
                BankAsset = bank.Asset;
            }

            public ControlData(Question question)
                : this(question.Set.Bank)
            {
                QuestionId = question.Identifier;
            }
        }

        [Serializable]
        private class BaseItem
        {
            public int Key { get; }
            public Guid Identifier { get; }
            public int Index { get; set; }
            public int Sequence { get; set; }
            public bool IsReadOnly { get; set; }

            public BaseItem(int key)
            {
                Key = key;
            }

            public BaseItem(int key, Guid id) : this(key)
            {
                Identifier = id;
            }
        }

        [Serializable]
        private class ItemCollection<T> : IReadOnlyList<T> where T : BaseItem
        {
            [field: NonSerialized]
            public event ItemEventHandler<T> ItemAdded;
            private void OnItemAdded(T item) => ItemAdded?.Invoke(this, new ItemEventArgs<T>(item));

            [field: NonSerialized]
            public event ItemEventHandler<T> ItemRemoved;
            private void OnItemRemoved(T item) => ItemRemoved?.Invoke(this, new ItemEventArgs<T>(item));

            public T this[int index] => _items[index];

            public int Count => _items.Count;

            private int _key = 0;
            private List<T> _items = new List<T>();
            private List<T> _removed = new List<T>();

            public T Add()
            {
                var item = (T)Activator.CreateInstance(typeof(T), new object[] { ++_key });

                item.Sequence = item.Key;

                _items.Add(item);

                OnItemAdded(item);

                return item;
            }

            public T Add(Guid id)
            {
                var item = (T)Activator.CreateInstance(typeof(T), new object[] { ++_key, id });

                item.Sequence = item.Key;

                _items.Add(item);

                OnItemAdded(item);

                return item;
            }

            public bool Remove(T item)
            {
                var result = _items.Remove(item);

                if (result)
                {
                    _removed.Add(item);

                    OnItemRemoved(item);
                }

                return result;
            }

            public bool Unremove(T item)
            {
                var result = _removed.Remove(item);

                if (result)
                {
                    _items.Add(item);

                    OnItemAdded(item);
                }

                return result;
            }

            public T FindByKey(int key)
            {
                return _items.FirstOrDefault(x => x.Key == key);
            }

            public T FindById(Guid id)
            {
                return _items.FirstOrDefault(x => x.Identifier == id);
            }

            public void UpdateItemIndexes()
            {
                foreach (var data in _items.OrderBy(x => x.Key).Select((item, index) => (Item: item, Index: index)))
                    data.Item.Index = data.Index;
            }

            public IEnumerable<T> EnumerateRemoved() => _removed.AsEnumerable();

            public IEnumerable<T> EnumerateAll() => _items.Concat(_removed);

            public IEnumerator<T> GetEnumerator() => _items.GetEnumerator();

            IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
        }

        [Serializable]
        private class OptionItem : BaseItem
        {
            public int Number { get; set; }
            public MultilingualString Text { get; } = new MultilingualString();

            public OptionItem(int key) : base(key)
            {

            }

            public OptionItem(int key, Guid id) : base(key, id)
            {

            }
        }

        [Serializable]
        private class SolutionItem : BaseItem
        {
            public string Letter => Calculator.ToBase26(Index + 1);
            public decimal Points { get; set; }
            public decimal? CutScore { get; set; }

            public List<int> Options { get; set; } = new List<int>();

            public SolutionItem(int key) : base(key)
            {

            }

            public SolutionItem(int key, Guid id) : base(key, id)
            {

            }
        }
    }
}