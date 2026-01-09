using System;

namespace Shift.Common.Timeline.Snapshots
{
    /// <summary>
    /// Defines the methods needed from the snapshot store.
    /// </summary>
    public interface ISnapshotStore
    {
        /// <summary>
        /// Counts the number of snapshots in the store for an aggregate.
        /// </summary>
        int Count(Guid id);

        /// <summary>
        /// Gets a snapshot from the store.
        /// </summary>
        Snapshot Get(Guid id);

        /// <summary>
        /// Saves a snapshot to the store.
        /// </summary>
        void Save(Snapshot snapshot);

        /// <summary>
        /// Copies a snapshot to offline storage and removes it from online logs.
        /// </summary>
        void Box(Guid id);

        /// <summary>
        /// Retrieves an aggregate from offline storage and returns only its most recent state.
        /// </summary>
        Snapshot Unbox(Guid id);

        /// <summary>
        /// Deletes a snapshot from the store.
        /// </summary>
        void Delete(Guid id, string type);
    }
}
