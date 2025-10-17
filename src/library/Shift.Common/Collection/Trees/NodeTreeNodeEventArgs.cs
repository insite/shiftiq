using System;

namespace Shift.Common.Trees
{
    public class NodeTreeNodeEventArgs<T> : EventArgs
    {
        public INode<T> Node { get; }

        public NodeTreeNodeEventArgs(INode<T> node)
        {
            Node = node;
        }
    }
}