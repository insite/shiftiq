using System;
using System.Collections.Generic;

namespace Shift.Common.Trees
{
    public interface ITree<T> : IEnumerableCollectionPair<T>, IDisposable
    {
        Type DataType { get; }

        IEqualityComparer<T> DataComparer { get; set; }

        void Clear();
        int Count { get; }
        int DirectChildCount { get; }

        INode<T> Root { get; }

        INode<T> this[T o] { get; }

        string ToStringRecursive();

        bool Contains(T item);
        bool Contains(INode<T> item);

        INode<T> InsertChild(T o);
        INode<T> AddChild(T o);

        void InsertChild(ITree<T> tree);
        void AddChild(ITree<T> tree);

        ITree<T> Cut(T o);
        ITree<T> Copy(T o);
        ITree<T> DeepCopy(T o);
        bool Remove(T o);

        ITree<T> Copy();
        ITree<T> DeepCopy();

        IEnumerableCollectionPair<T> All { get; }
        IEnumerableCollectionPair<T> AllChildren { get; }
        IEnumerableCollectionPair<T> DirectChildren { get; }
        IEnumerableCollectionPair<T> DirectChildrenInReverse { get; }

        void SortAllChildren();
        void SortAllChildren(Comparison<T> comparison);
        void SortAllChildren(IComparer<T> comparer);

        event EventHandler<NodeTreeDataEventArgs<T>> Validate;
        event EventHandler Clearing;
        event EventHandler Cleared;
        event EventHandler<NodeTreeDataEventArgs<T>> Setting;
        event EventHandler<NodeTreeDataEventArgs<T>> SetDone;
        event EventHandler<NodeTreeInsertEventArgs<T>> Inserting;
        event EventHandler<NodeTreeInsertEventArgs<T>> Inserted;
        event EventHandler Cutting;
        event EventHandler CutDone;
        event EventHandler<NodeTreeNodeEventArgs<T>> Copying;
        event EventHandler<NodeTreeNodeEventArgs<T>> Copied;
        event EventHandler<NodeTreeNodeEventArgs<T>> DeepCopying;
        event EventHandler<NodeTreeNodeEventArgs<T>> DeepCopied;
    }
}