using System;

namespace Shift.Common.Trees
{
    internal class AllEnumerator<T> : BaseEnumerableCollectionPair<T>
    {
        public AllEnumerator(NodeTree<T> root) : base(root)
        {
        }

        public override IEnumerableCollection<INode<T>> Nodes => new NodesEnumerableCollection(Root);

        protected class NodesEnumerableCollection : BaseNodesEnumerableCollection
        {
            private bool _first = true;

            public NodesEnumerableCollection(NodeTree<T> root) : base(root)
            {
            }

            protected NodesEnumerableCollection(NodesEnumerableCollection o) : base(o.Root)
            {
            }

            protected override BaseNodesEnumerableCollection CreateCopy()
            {
                return new NodesEnumerableCollection(this);
            }

            public override void Reset()
            {
                base.Reset();

                _first = true;
            }

            public override bool MoveNext()
            {
                if (!base.MoveNext()) goto hell;

                if (CurrentNode == null) throw new InvalidOperationException("Current is null");

                if (CurrentNode.IsRoot)
                {
                    CurrentNode = CurrentNode.Child;
                    if (CurrentNode == null) goto hell;
                }

                if (_first)
                {
                    _first = false;
                    return true;
                }

                if (CurrentNode.Child != null)
                {
                    CurrentNode = CurrentNode.Child;
                    return true;
                }

                for (; CurrentNode.Parent != null; CurrentNode = CurrentNode.Parent)
                {
                    if (CurrentNode == Root) goto hell;
                    if (CurrentNode.Next != null)
                    {
                        CurrentNode = CurrentNode.Next;
                        return true;
                    }
                }

                hell:

                AfterLast = true;
                return false;
            }
        }
    }
}