using System;
using System.Collections;
using System.Collections.Generic;

namespace Shift.Sdk.UI
{
    public class HistoryCollection : IReadOnlyCollection<HistoryEntity>
    {
        #region Classes

        private class HistoryComparer : IComparer<HistoryEntity>
        {
            private readonly int _maxTimeInterval;

            public HistoryComparer(int maxTimeInterval)
            {
                _maxTimeInterval = maxTimeInterval;
            }

            public int Compare(HistoryEntity x, HistoryEntity y)
            {
                // if the interval between two events is less than _maxTimeInterval then these are the same events

                if (string.Equals(x.UserName, x.UserName, StringComparison.OrdinalIgnoreCase))
                {
                    return Math.Abs((x.Date - y.Date).TotalMilliseconds) <= _maxTimeInterval
                        ? 0
                        : y.Date.CompareTo(x.Date);
                }
                else
                {
                    var result = y.Date.CompareTo(x.Date);

                    return result == 0
                        ? x.UserName.CompareTo(y.UserName)
                        : result;
                }
            }
        }

        #endregion

        #region Properties

        public int Count => _entities.Count;

        public HistoryEntity First => _entities.Count > 0 ? _entities[0] : null;

        #endregion

        #region Fields

        private readonly HistoryComparer _comparer;
        private readonly List<HistoryEntity> _entities;

        #endregion

        #region Construction

        public HistoryCollection(int maxTimeInterval = 1000)
        {
            _comparer = new HistoryComparer(maxTimeInterval);
            _entities = new List<HistoryEntity>();
        }

        #endregion

        #region Methods

        public HistoryEntity Add(DateTime when, string who, string what) =>
            Add(new HistoryEntity(when, who, what));

        public HistoryEntity Add(HistoryEntity entity)
        {
            if (_entities.Count == 0)
            {
                _entities.Add(entity);

                return entity;
            }

            var index = _entities.BinarySearch(entity, _comparer);
            if (index >= 0)
            {
                var target = _entities[index];
                target.MergeWith(entity);
                entity = target;
            }
            else
            {
                _entities.Insert(~index, entity);
            }

            return entity;
        }

        public void AddRange(IEnumerable<HistoryEntity> enumerable)
        {
            foreach (var e in enumerable)
                Add(e);
        }

        #endregion

        #region IEnumerator

        public IEnumerator<HistoryEntity> GetEnumerator() => _entities.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        #endregion
    }
}