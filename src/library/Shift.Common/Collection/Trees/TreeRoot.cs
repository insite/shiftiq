using System;
using System.Collections.Generic;

namespace Shift.Common.Trees
{
    [Serializable]
    internal class TreeRoot<T> : NodeTree<T>
    {
        private IEqualityComparer<T> _comparer;
        private int _version;

        public TreeRoot()
        {
        }

        public TreeRoot(IEqualityComparer<T> comparer)
        {
            _comparer = comparer;
        }

        public override int Version
        {
            get => _version;
            set => _version = value;
        }

        public override IEqualityComparer<T> DataComparer
        {
            get => _comparer ?? (_comparer = EqualityComparer<T>.Default);
            set => _comparer = value;
        }
        
        public override string ToString()
        {
            return "ROOT: " + DataType.Name;
        }
    }
}