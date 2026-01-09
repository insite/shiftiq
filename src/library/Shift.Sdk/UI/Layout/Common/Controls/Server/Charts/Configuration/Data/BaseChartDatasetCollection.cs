using System;
using System.Collections;
using System.Collections.Generic;

namespace Shift.Sdk.UI
{
    [Serializable]
    public abstract class BaseChartDatasetCollection<T> : IReadOnlyList<T> where T : BaseChartDataset
    {
        #region Properties

        public int Count => _list.Count;

        public T this[int index] => _list[index];

        public T this[string id] => _dictionary[id];

        #endregion

        #region Fields

        private List<T> _list = new List<T>();
        private Dictionary<string, T> _dictionary = new Dictionary<string, T>(StringComparer.OrdinalIgnoreCase);

        #endregion

        #region Methods

        public T Create(string id)
        {
            if (string.IsNullOrEmpty(id))
                throw new ArgumentNullException(nameof(id));

            if (_dictionary.ContainsKey(id))
                throw new ApplicationException($"The chart already contains a dataset with id {id}");

            var dataset = CreateDatasetInstance(id);

            _dictionary.Add(id, dataset);
            _list.Add(dataset);

            return dataset;
        }

        protected abstract T CreateDatasetInstance(string id);

        public bool Remove(string id)
        {
            if (!_dictionary.ContainsKey(id))
                return false;

            var dataset = _dictionary[id];

            _dictionary.Remove(id);
            _list.Remove(dataset);

            return true;
        }

        public void Clear()
        {
            _list.Clear();
            _dictionary.Clear();
        }

        #endregion

        #region IEnumerable

        public IEnumerator<T> GetEnumerator() => _list.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        #endregion
    }
}
