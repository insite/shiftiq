using System;

namespace Shift.Common.Trees
{
    internal class DirectChildrenInReverseEnumerator<T> : BaseEnumerableCollectionPair<T>
    {
        public DirectChildrenInReverseEnumerator(NodeTree<T> root) : base(root)
        {
        }

        public override IEnumerableCollection<INode<T>> Nodes => new NodesEnumerableCollection(Root);

        protected class NodesEnumerableCollection : BaseNodesEnumerableCollection
        {
            public NodesEnumerableCollection(NodeTree<T> root) : base(root)
            {
            }

            protected NodesEnumerableCollection(NodesEnumerableCollection o) : base(o.Root)
            {
            }

            public override int Count => Root.DirectChildCount;

            protected override BaseNodesEnumerableCollection CreateCopy()
            {
                return new NodesEnumerableCollection(this);
            }

            public override bool MoveNext()
            {
                if (!base.MoveNext()) goto hell;

                if (CurrentNode == null) throw new InvalidOperationException("Current is null");

                CurrentNode = CurrentNode == Root ? Root.LastChild : CurrentNode.Previous;

                if (CurrentNode != null) return true;

                hell:

                AfterLast = true;
                return false;
            }
        }
    }
}