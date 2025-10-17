using System;

namespace Shift.Common
{
    public abstract class GroupNode<TChild> : GroupLeaf where TChild : GroupLeaf, IComparable<TChild>
    {
        #region Properties

        public GroupInfoCollection<TChild> Children { get; private set; }

        #endregion

        #region Methods

        internal override void Init(IGroupTable root, GroupLeaf parent, object[] key)
        {
            base.Init(root, parent, key);

            Children = new GroupInfoCollection<TChild>(root, this);
        }

        #endregion
    }

    public class DefaultGroupNode<TChild> : GroupNode<TChild>, IComparable<DefaultGroupNode<TChild>> where TChild : GroupLeaf, IComparable<TChild>
    {
        #region Properties

        public string Text { get; set; }

        #endregion

        #region Methods

        public int CompareTo(DefaultGroupNode<TChild> other) => String.Compare(Text, other.Text, StringComparison.Ordinal);

        #endregion
    }
}
