using System;

namespace Shift.Common
{
    [Serializable]
    public abstract class GroupLeaf
    {
        #region Properties

        public IGroupTable Root => _root;

        public GroupLeaf Parent => _parent;

        protected object[] Key => _key;

        #endregion

        #region Fields

        private object[] _key;
        private IGroupTable _root;
        private GroupLeaf _parent;

        #endregion

        #region Methods

        internal virtual void Init(IGroupTable root, GroupLeaf parent, object[] key)
        {
            _root = root ?? throw new ArgumentNullException(nameof(root));
            _parent = parent;
            _key = key;
        }

        public MultiKey GetCellKey() => BuildCellKey(0);

        private MultiKey BuildCellKey(int length)
        {
            var key = _parent == null
                ? new MultiKey(length + _key.Length)
                : _parent.BuildCellKey(length + _key.Length);

            foreach (var value in _key)
                key.Add(value);

            return key;
        }

        #endregion
    }

    [Serializable]
    public class DefaultGroupLeaf : GroupLeaf, IComparable<DefaultGroupLeaf>
    {
        #region Properties

        public string Text { get; set; }

        #endregion

        #region Methods

        public int CompareTo(DefaultGroupLeaf other) => String.Compare(Text, other.Text, StringComparison.Ordinal);

        #endregion
    }
}
