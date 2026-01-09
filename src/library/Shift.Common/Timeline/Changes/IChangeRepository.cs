using System;

namespace Shift.Common.Timeline.Changes
{
    /// <summary>
    /// Provides functionality to get and save aggregates.
    /// </summary>
    public interface IChangeRepository
    {
        /// <summary>
        /// Returns a specific aggregate.
        /// </summary>
        T Get<T>(Guid id, int? expectedVersion = -1) where T : AggregateRoot;

        T GetClone<T>(Guid aggregateId, int? version = -1) where T : AggregateRoot;

        void LockAndRun<T>(Guid aggregateId, Action<T> action) where T : AggregateRoot;

        /// <summary>
        /// Returns a specific aggregate as at a specific version.
        /// </summary>
        T Peek<T>(Guid id, int asAtVersion) where T : AggregateRoot;

        /// <summary>
        /// Returns true if an aggregate exists.
        /// </summary>
        bool Exists<T>(Guid id);

        /// <summary>
        /// Saves an aggregate.
        /// </summary>
        /// <returns>
        /// Returns the changes that are now saved (and ready to be published).
        /// </returns>
        IChange[] Save<T>(T aggregate, int? expectedVersion = null) where T : AggregateRoot;

        /// <summary>
        /// Copies an aggregate to offline storage and removes it from online logs.
        /// </summary>
        void Box<T>(T aggregate) where T : AggregateRoot;

        /// <summary>
        /// Retrieves an aggregate from offline storage and returns only its most recent state.
        /// </summary>
        T Unbox<T>(Guid aggregate) where T : AggregateRoot;

        (AggregateState prev, AggregateState current) GetPrevAndCurrentStates(Guid aggregateId, int version);

        IChange[] GetStatesAndChanges(Guid aggregateId);
    }
}
