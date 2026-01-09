using System.Collections.Generic;

namespace Shift.Common.Timeline.Changes
{
    public interface IChangeBuffer
    {
        void Open();
        void Save(AggregateRoot aggregate, IEnumerable<IChange> changes);
        void Flush();
        void Close();
    }
}
