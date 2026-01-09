using Shift.Common.Timeline.Changes;

namespace Shift.Common.Timeline.Snapshots
{
    /// <summary>
    /// Defines the strategy to use for determining when a snapshot should be taken.
    /// </summary>
    public interface ISnapshotStrategy
    {
        /// <summary>
        /// Returns true if a snapshot should be taken for the aggregate.
        /// </summary>
        bool ShouldTakeSnapShot(AggregateRoot aggregate, int snapshotCount);
    }
}
