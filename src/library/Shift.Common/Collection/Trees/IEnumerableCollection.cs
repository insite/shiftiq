using System.Collections;
using System.Collections.Generic;

namespace Shift.Common.Trees
{
    public interface IEnumerableCollection<T> : IEnumerable<T>, ICollection
    {
        bool Contains(T item);
    }
}