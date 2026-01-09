using Shift.Common.Timeline.Changes;

namespace Shift.Common.Timeline.Snapshots
{
    /// <summary>
    /// Implements the default snapshot strategy. A snapshot of an aggregate is taken after every Interval events.
    /// </summary>
    public class SnapshotStrategy : ISnapshotStrategy
    {
        private readonly int _interval;

        /// <summary>
        /// Constructs a new strategy.
        /// </summary>
        public SnapshotStrategy(int interval)
        {
            _interval = interval;
        }

        /// <summary>
        /// Returns true if a snapshot should be taken for the aggregate.
        /// </summary>
        public bool ShouldTakeSnapShot(AggregateRoot aggregate, int snapshotCount)
        {
            var i = aggregate.AggregateVersion;
            
            for (var j = 0; j < aggregate.GetUncommittedChanges().Length; j++)
                if (++i % _interval == 0 && i != 0)
                    return true;

            if (snapshotCount == 0 && aggregate.AggregateVersion > _interval)
                return true;

            return false;
        }
    }
}
