using System.Collections.Generic;

namespace Shift.Common.Trees
{
    internal class NodeComparer<T> : IComparer<INode<T>>
    {
        private readonly IComparer<T> _dataComparer;

        public NodeComparer(IComparer<T> dataComparer)
        {
            _dataComparer = dataComparer;
        }

        public int Compare(INode<T> x, INode<T> y)
        {
            if (x == null && y == null) return 0;
            if (x == null) return -1;
            if (y == null) return 1;
            return _dataComparer.Compare(x.Data, y.Data);
        }
    }
}