using System;
using System.Collections.Generic;

namespace Shift.Common.Trees
{
    public interface INode<T> : IEnumerableCollectionPair<T>, IDisposable
    {
        T Data { get; set; }

        string ToStringRecursive();

        int Depth { get; }
        int BranchIndex { get; }
        int BranchCount { get; }

        int Count { get; }
        int DirectChildCount { get; }

        INode<T> Parent { get; }
        INode<T> Previous { get; }
        INode<T> Next { get; }
        INode<T> Child { get; }

        ITree<T> Tree { get; }

        INode<T> Root { get; }
        INode<T> Top { get; }
        INode<T> First { get; }
        INode<T> Last { get; }

        INode<T> LastChild { get; }

        bool IsTree { get; }
        bool IsRoot { get; }
        bool IsTop { get; }
        bool HasParent { get; }
        bool HasPrevious { get; }
        bool HasNext { get; }
        bool HasChild { get; }
        bool IsFirst { get; }
        bool IsLast { get; }

        INode<T> this[T item] { get; }

        bool Contains(INode<T> item);
        bool Contains(T item);

        INode<T> InsertPrevious(T o);
        INode<T> InsertNext(T o);
        INode<T> InsertChild(T o);
        INode<T> Add(T o);
        INode<T> AddChild(T o);

        void InsertPrevious(ITree<T> tree);
        void InsertNext(ITree<T> tree);
        void InsertChild(ITree<T> tree);
        void Add(ITree<T> tree);
        void AddChild(ITree<T> tree);

        ITree<T> Cut(T o);
        ITree<T> Copy(T o);
        ITree<T> DeepCopy(T o);
        bool Remove(T o);

        ITree<T> Cut();
        ITree<T> Copy();
        ITree<T> DeepCopy();
        void Remove();

        bool CanMoveToParent { get; }
        bool CanMoveToPrevious { get; }
        bool CanMoveToNext { get; }
        bool CanMoveToChild { get; }
        bool CanMoveToFirst { get; }
        bool CanMoveToLast { get; }

        void MoveToParent();
        void MoveToPrevious();
        void MoveToNext();
        void MoveToChild();
        void MoveToFirst();
        void MoveToLast();

        IEnumerableCollectionPair<T> All { get; }
        IEnumerableCollectionPair<T> AllChildren { get; }
        IEnumerableCollectionPair<T> DirectChildren { get; }
        IEnumerableCollectionPair<T> DirectChildrenInReverse { get; }

        void SortAllChildren();
        void SortAllChildren(Comparison<T> comparison);
        void SortAllChildren(IComparer<T> comparer);

        void SortDirectChildren();
        void SortDirectChildren(Comparison<T> comparison);
        void SortDirectChildren(IComparer<T> comparer);

        event EventHandler<NodeTreeDataEventArgs<T>> Validate;
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