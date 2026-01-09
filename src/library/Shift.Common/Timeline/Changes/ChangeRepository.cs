using System;

using Shift.Common.Timeline.Exceptions;

namespace Shift.Common.Timeline.Changes
{
    /// <summary>
    /// Saves and gets aggregates to and from an change store.
    /// </summary>
    public class ChangeRepository : IChangeRepository
    {
        private readonly IChangeStore _store;

        public ChangeRepository(IChangeStore store)
        {
            _store = store ?? throw new ArgumentNullException(nameof(store));
        }

        /// <summary>
        /// Gets an aggregate from the change store.
        /// </summary>
        public T Get<T>(Guid aggregate, int? expectedVersion = -1) where T : AggregateRoot
        {
            return Rehydrate<T>(aggregate, expectedVersion);
        }

        public T GetClone<T>(Guid aggregateId, int? version = -1) where T : AggregateRoot
        {
            return Rehydrate<T>(aggregateId, version);
        }

        public void LockAndRun<T>(Guid aggregateId, Action<T> action) where T : AggregateRoot
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Returns a specific aggregate as at a specific version.
        /// </summary>
        public T Peek<T>(Guid aggregate, int asAtVersion) where T : AggregateRoot
        {
            return RehydrateAsAt<T>(aggregate, asAtVersion);
        }

        /// <summary>
        /// Returns true if an aggregate exists.
        /// </summary>
        public bool Exists<T>(Guid aggregate)
        {
            return _store.Exists<T>(aggregate);
        }

        /// <summary>
        /// Saves all uncommitted changes to the change store.
        /// </summary>
        public IChange[] Save<T>(T aggregate, int? expectedVersion) where T : AggregateRoot
        {
            if (expectedVersion != null && _store.Exists(aggregate.AggregateIdentifier, expectedVersion.Value))
                throw new ConcurrencyException(aggregate.AggregateIdentifier);

            // Get the list of changes that are not yet saved. 
            var changes = aggregate.FlushUncommittedChanges();

            if (changes.Length > 0) // Save the uncommitted changes.
                _store.Save(aggregate, changes);

            // The change repository is not responsible for publishing these changes. Instead they are returned to the 
            // caller for that purpose.
            return changes;
        }

        /// <summary>
        /// Loads an aggregate instance from the full history of changes for that aggreate.
        /// </summary>
        private T Rehydrate<T>(Guid id, int? expectedVersion = -1) where T : AggregateRoot
        {
            // Get all the changes for the aggregate.
            var changes = _store.GetChanges(id, expectedVersion ?? -1);

            // Disallow empty change streams.
            if (changes.Length == 0)
                throw new AggregateNotFoundException(typeof(T), id);

            // Create and load the aggregate.
            var aggregate = AggregateFactory<T>.CreateAggregate();
            aggregate.Rehydrate(changes);
            return aggregate;
        }

        /// <summary>
        /// Loads an aggregate instance from the full history of changes for that aggreate.
        /// </summary>
        private T RehydrateAsAt<T>(Guid id, int version) where T : AggregateRoot
        {
            // Get all the changes for the aggregate.
            var changes = _store.GetChanges(id, -1, version);

            // Disallow empty change streams.
            if (changes.Length == 0)
                throw new AggregateNotFoundException(typeof(T), id);

            // Create and load the aggregate.
            var aggregate = AggregateFactory<T>.CreateAggregate();
            aggregate.Rehydrate(changes);
            return aggregate;
        }

        (AggregateState prev, AggregateState current) IChangeRepository.GetPrevAndCurrentStates(Guid aggregateId, int version)
            => throw new NotImplementedException();

        IChange[] IChangeRepository.GetStatesAndChanges(Guid aggregateId)
            => throw new NotImplementedException();

        #region Methods (boxing and unboxing)

        /// <summary>
        /// Copies an aggregate to offline storage and removes it from online logs.
        /// </summary>
        /// <remarks>
        /// Aggregate boxing/unboxing is not implemented by default for all aggregates. It must be explicitly 
        /// implemented per aggregate for those aggregates that require this functionality, and snapshots are required. 
        /// Therefore this function in this class throws a NotImplementedException; refer to SnapshotRepository for the
        /// implementation.
        /// </remarks>
        public void Box<T>(T aggregate) where T : AggregateRoot
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Retrieves an aggregate from offline storage and returns only its most recent state.
        /// </summary>
        /// <remarks>
        /// Aggregate boxing/unboxing is not implemented by default for all aggregates. It must be explicitly 
        /// implemented per aggregate for those aggregates that require this functionality, and snapshots are required. 
        /// Therefore this function in this class throws a NotImplementedException; refer to SnapshotRepository for the
        /// implementation.
        /// </remarks>
        public T Unbox<T>(Guid aggregateId) where T : AggregateRoot
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}
