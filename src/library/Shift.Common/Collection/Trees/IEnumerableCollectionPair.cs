using System;

namespace Shift.Common.Trees
{
    public interface IEnumerableCollectionPair<T>
    {
        IEnumerableCollection<INode<T>> Nodes { get; }
        IEnumerableCollection<T> Values { get; }
        INode<T> Find(T item);
        INode<T> Find(Predicate<T> predicate);
    }
}