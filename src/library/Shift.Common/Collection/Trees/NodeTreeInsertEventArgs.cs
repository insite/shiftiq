using System;

using Shift.Constant;

namespace Shift.Common.Trees
{
    public class NodeTreeInsertEventArgs<T> : EventArgs
    {
        public NodeTreeInsertOperation Operation { get; }

        public INode<T> Node { get; }

        public NodeTreeInsertEventArgs(NodeTreeInsertOperation operation, INode<T> node)
        {
            Operation = operation;
            Node = node;
        }
    }
}