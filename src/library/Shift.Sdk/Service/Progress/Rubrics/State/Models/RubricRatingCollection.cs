using System;
using System.Collections;
using System.Collections.Generic;

using Shift.Common;

namespace InSite.Domain.Records
{
    public class RubricRatingCollection : ICollection<RubricRating>
    {
        public int Count => _items.Count;

        public bool IsReadOnly => false;

        public RubricRating this[int index] => _items[index];

        #region Fields

        [NonSerialized]
        private RubricCriterion _criterion;

        private List<RubricRating> _items = new List<RubricRating>();

        #endregion

        #region Construction

        private RubricRatingCollection()
        {

        }

        public RubricRatingCollection(RubricCriterion criterion)
        {
            _criterion = criterion;
        }

        #endregion

        #region Methods (ICollection)

        public void Add(RubricRating item)
        {
            item.SetContainer(_criterion);

            _items.Add(item);
        }

        public void Add(RubricRating item, int? sequence)
        {
            item.SetContainer(_criterion);

            var index = sequence.HasValue ? sequence.Value - 1 : -1;

            if (index >= 0 && index < _items.Count)
                _items.Insert(index, item);
            else
                _items.Add(item);
        }

        public void Clear()
        {
            foreach (var item in _items)
                item.RemoveContainer();
            _items.Clear();
        }

        public bool Contains(RubricRating item)
        {
            return _items.Contains(item);
        }

        public void CopyTo(RubricRating[] array, int arrayIndex)
        {
            throw new NotSupportedException();
        }

        public int IndexOf(RubricRating item)
        {
            return _items.IndexOf(item);
        }

        public IEnumerator<RubricRating> GetEnumerator()
        {
            return _items.GetEnumerator();
        }

        public bool Remove(RubricRating item)
        {
            var result = _items.Remove(item);

            if (result)
                item.RemoveContainer();

            return result;
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        #endregion

        #region Methods (helpers)

        public void SetContainer(RubricCriterion criterion)
        {
            if (_criterion == null)
            {
                _criterion = criterion;

                foreach (var item in _items)
                    item.SetContainer(_criterion);
            }
            else if (_criterion != criterion)
                throw ApplicationError.Create("Criterion is already assigned to this collection");
        }

        #endregion
    }
}
