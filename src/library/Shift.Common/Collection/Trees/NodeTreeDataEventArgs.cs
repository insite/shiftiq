using System;

namespace Shift.Common.Trees
{
    public class NodeTreeDataEventArgs<T> : EventArgs
    {
        public T Data { get; }

        public NodeTreeDataEventArgs(T data)
        {
            Data = data;
        }
    }
}