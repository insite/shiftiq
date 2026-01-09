using System;
using System.Collections;
using System.Collections.Generic;

namespace Shift.Common.Trees
{
    internal abstract class BaseEnumerableCollectionPair<T> : IEnumerableCollectionPair<T>
    {
        protected BaseEnumerableCollectionPair(NodeTree<T> root)
        {
            Root = root;
        }

        protected NodeTree<T> Root { get; set; }

        // Nodes

        public abstract IEnumerableCollection<INode<T>> Nodes { get; }

        // Find

        public virtual INode<T> Find(T data)
        {
            var comparer = Root.DataComparer;

            INode<T> retval = null;

            foreach (var node in Nodes)
                if (comparer.Equals(node.Data, data))
                    if (retval != null)
                    {
                        throw new TreeException("Multiple matches");
                    }
                    else
                    {
                        retval = node;
                    }

            return retval;
        }

        public virtual INode<T> Find(Predicate<T> predicate)
        {
            INode<T> retval = null;

            foreach (var node in Nodes)
                if (predicate(node.Data))
                    if (retval != null)
                    {
                        throw new TreeException("Multiple matches");
                    }
                    else
                    {
                        retval = node;
                    }

            return retval;
        }

        // Values

        public virtual IEnumerableCollection<T> Values => new ValuesEnumerableCollection(Root.DataComparer, Nodes);

        protected abstract class BaseNodesEnumerableCollection : IEnumerableCollection<INode<T>>, IEnumerator<INode<T>>
        {
            private readonly int _version;

            protected BaseNodesEnumerableCollection(NodeTree<T> root)
            {
                Root = root;
                CurrentNode = root;

                _version = Root.Version;
            }

            protected NodeTree<T> Root { get; set; }

            protected INode<T> CurrentNode { get; set; }

            protected bool BeforeFirst { get; set; } = true;

            protected bool AfterLast { get; set; }

            protected virtual bool HasChanged => Root.HasChanged(_version);

            // IEnumerable
            IEnumerator IEnumerable.GetEnumerator()
            {
                return GetEnumerator();
            }

            // IEnumerable<INode<T>>
            public virtual IEnumerator<INode<T>> GetEnumerator()
            {
                Reset();

                return this;
            }

            // ICollection
            public virtual int Count
            {
                get
                {
                    var e = CreateCopy();

                    var i = 0;
                    foreach (var unused in e) i++;
                    return i;
                }
            }

            public virtual bool IsSynchronized => false;

            public virtual object SyncRoot { get; } = new object();

            void ICollection.CopyTo(Array array, int index)
            {
                if (array == null) throw new ArgumentNullException(nameof(array));
                if (array.Rank > 1) throw new ArgumentException(@"array is multidimensional", nameof(array));
                if (index < 0) throw new ArgumentOutOfRangeException(nameof(index));

                var count = Count;

                if (count > 0)
                    if (index >= array.Length) throw new ArgumentException(@"index is out of bounds", nameof(index));

                if (index + count > array.Length)
                    throw new ArgumentException(@"Not enough space in array", nameof(array));

                var e = CreateCopy();

                foreach (var n in e)
                    array.SetValue(n, index++);
            }

            // ICollectionEnumerable<INode<T>>
            public virtual bool Contains(INode<T> item)
            {
                var e = CreateCopy();

                IEqualityComparer<INode<T>> comparer = EqualityComparer<INode<T>>.Default;

                foreach (var n in e)
                    if (comparer.Equals(n, item))
                        return true;

                return false;
            }

            // IDisposable
            public void Dispose()
            {
                Dispose(true);

                GC.SuppressFinalize(this);
            }

            // IEnumerator
            object IEnumerator.Current => Current;

            // IEnumerator<INode<T>>
            public virtual void Reset()
            {
                if (HasChanged) throw new InvalidOperationException("Tree has been modified.");

                CurrentNode = Root;

                BeforeFirst = true;
                AfterLast = false;
            }

            public virtual bool MoveNext()
            {
                if (HasChanged) throw new InvalidOperationException("Tree has been modified.");

                BeforeFirst = false;

                return true;
            }

            public virtual INode<T> Current
            {
                get
                {
                    if (BeforeFirst) throw new InvalidOperationException("Enumeration has not started.");
                    if (AfterLast) throw new InvalidOperationException("Enumeration has finished.");

                    return CurrentNode;
                }
            }

            ~BaseNodesEnumerableCollection()
            {
                Dispose(false);
            }

            protected abstract BaseNodesEnumerableCollection CreateCopy();

            protected virtual void Dispose(bool disposing)
            {
            }

            public virtual void CopyTo(T[] array, int index)
            {
                ((ICollection) this).CopyTo(array, index);
            }
        }

        private sealed class ValuesEnumerableCollection : IEnumerableCollection<T>, IEnumerator<T>
        {
            private readonly IEqualityComparer<T> _DataComparer;
            private readonly IEnumerator<INode<T>> _Enumerator;
            private readonly IEnumerableCollection<INode<T>> _Nodes;

            public ValuesEnumerableCollection(IEqualityComparer<T> dataComparer, IEnumerableCollection<INode<T>> nodes)
            {
                _DataComparer = dataComparer;
                _Nodes = nodes;
                _Enumerator = _Nodes.GetEnumerator();
            }

            private ValuesEnumerableCollection(ValuesEnumerableCollection o)
            {
                _Nodes = o._Nodes;
                _Enumerator = _Nodes.GetEnumerator();
            }

            // IEnumerable
            IEnumerator IEnumerable.GetEnumerator()
            {
                return GetEnumerator();
            }

            // IEnumerable<T>
            public IEnumerator<T> GetEnumerator()
            {
                Reset();

                return this;
            }

            // ICollection
            public int Count => _Nodes.Count;

            public bool IsSynchronized => false;

            public object SyncRoot => _Nodes.SyncRoot;

            public void CopyTo(Array array, int index)
            {
                if (array == null) throw new ArgumentNullException(nameof(array));
                if (array.Rank > 1) throw new ArgumentException(@"array is multidimensional", nameof(array));
                if (index < 0) throw new ArgumentOutOfRangeException(nameof(index));

                var count = Count;

                if (count > 0)
                    if (index >= array.Length) throw new ArgumentException(@"index is out of bounds", nameof(index));

                if (index + count > array.Length)
                    throw new ArgumentException(@"Not enough space in array", nameof(array));

                var e = CreateCopy();

                foreach (var n in e)
                    array.SetValue(n, index++);
            }

            // IEnumerableCollection<T>
            public bool Contains(T item)
            {
                var e = CreateCopy();

                foreach (var n in e)
                    if (_DataComparer.Equals(n, item))
                        return true;

                return false;
            }

            public void Dispose()
            {
                Dispose(true);

                GC.SuppressFinalize(this);
            }

            // IEnumerator
            object IEnumerator.Current => Current;

            // IEnumerator<T> Members
            public void Reset()
            {
                _Enumerator.Reset();
            }

            public bool MoveNext()
            {
                return _Enumerator.MoveNext();
            }

            public T Current
            {
                get
                {
                    if (_Enumerator == null)
                    {
                        throw new TreeException("Missing enumerator");
                    }
                    if (_Enumerator.Current == null)
                    {
                        throw new TreeException("Missing enumerator");
                    }

                    return _Enumerator.Current.Data;
                }
            }

            private ValuesEnumerableCollection CreateCopy()
            {
                return new ValuesEnumerableCollection(this);
            }

            // IDisposable
            ~ValuesEnumerableCollection()
            {
                Dispose(false);
            }

            private void Dispose(bool _)
            {
            }
        }
    }

    internal class TreeException : Exception
    {
        public TreeException(string message) : base(message)
        {
            
        }
    }
}