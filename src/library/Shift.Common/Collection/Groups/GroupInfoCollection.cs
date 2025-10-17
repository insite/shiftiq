using System;
using System.Collections;
using System.Collections.Generic;

namespace Shift.Common
{
    [Serializable]
    public class GroupInfoCollection<TGroup> : IReadOnlyList<TGroup> where TGroup : GroupLeaf, IComparable<TGroup>
    {
        #region Properties

        public TGroup this[int index] => _list[index];

        public int Count => _list.Count;

        #endregion

        #region Fields

        private IGroupTable _root;
        private GroupLeaf _container;
        private List<TGroup> _list = new List<TGroup>();
        private Dictionary<MultiKey, TGroup> _index = new Dictionary<MultiKey, TGroup>();

        #endregion

        #region Construction

        public GroupInfoCollection(IGroupTable root, GroupLeaf container)
        {
            _root = root;
            _container = container;
        }

        #endregion

        #region Method

        public TGroup GetOrAdd(Func<TGroup> valueFactory, params object[] keys)
        {
            var key = new MultiKey(keys);

            if (_index.ContainsKey(key))
                return _index[key];

            var value = valueFactory();

            value.Init(_root, _container, keys);

            _index.Add(key, value);
            _list.AddSorted(value);

            return value;
        }

        public TGroup Get(params object[] keys)
        {
            var key = new MultiKey(keys);

            return _index.ContainsKey(key) ? _index[key] : null;
        }

        #endregion

        #region IEnumerable

        public IEnumerator<TGroup> GetEnumerator() => _list.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        #endregion
    }
}
