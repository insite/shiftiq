using System.Collections.Generic;

namespace Shift.Common.Timeline.Changes
{
    public class AggregateImport
    {
        public AggregateRoot Aggregate { get; set; }
        public IEnumerable<IChange> Changes { get; set; }
    }
}