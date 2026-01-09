using System;
using System.Linq;

using Shift.Common.Timeline.Changes;
using Shift.Common.Timeline.Exceptions;
using Shift.Common.Timeline.Services;

using Shift.Common;

namespace Shift.Common.Timeline.Snapshots
{
    /// <summary>
    /// Saves and gets snapshots to and from a snapshot store.
    /// </summary>
    public class SnapshotRepository : IChangeRepository
    {
        private readonly IGuidCache<AggregateRoot> _cache;
        private readonly IJsonSerializer _serializer;

        private readonly ISnapshotStore _snapshotStore;
        private readonly ISnapshotStrategy _snapshotStrategy;
        private readonly IChangeRepository _changeRepository;
        private readonly IChangeStore _changeStore;

        /// <summary>
        /// Constructs a new SnapshotRepository instance.
        /// </summary>
        /// <param name="changeStore">Store where changes are persisted</param>
        /// <param name="changeRepository">Repository to get aggregates from the change store</param>
        /// <param name="snapshotStore">Store where snapshots are persisted</param>
        /// <param name="snapshotStrategy">Strategy used to determine when to take a snapshot</param>
        public SnapshotRepository(IChangeStore changeStore, IChangeRepository changeRepository, ISnapshotStore snapshotStore, ISnapshotStrategy snapshotStrategy)
        {
            _changeStore = changeStore ?? throw new ArgumentNullException(nameof(changeStore));
            _changeRepository = changeRepository ?? throw new ArgumentNullException(nameof(changeRepository));
            _snapshotStore = snapshotStore ?? throw new ArgumentNullException(nameof(snapshotStore));
            _snapshotStrategy = snapshotStrategy ?? throw new ArgumentNullException(nameof(snapshotStrategy));

            _cache = ServiceLocator.Instance.GetService<IGuidCache<AggregateRoot>>();
            _serializer = ServiceLocator.Instance.GetService<IJsonSerializer>();
        }

        /// <summary>
        /// Saves the aggregate. Takes a snapshot if needed.
        /// </summary>
        public IChange[] Save<T>(T aggregate, int? version = null) where T : AggregateRoot
        {
            var concurrencyChange = false;

            var previous = (T)_cache.Get(aggregate.AggregateIdentifier);

            if (previous != null && aggregate != previous)
                throw new ConcurrencyException($"Aggregate {aggregate.AggregateIdentifier} version {aggregate.AggregateVersion} cannot be saved because another aggregate (version {aggregate.AggregateVersion}) already exists in the cache with the same identifier. Your code might be trying to create a new aggregate that is already created.");

            // Cache the aggregate for 5 minutes.
            lock (_cache)
                _cache.Add(aggregate.AggregateIdentifier, aggregate, 5 * 60, true);

            IChange[] changes = null;

            aggregate.LockAndRun(() =>
            {
                if (!concurrencyChange)
                {
                    // Take a snapshot if needed but only if no concurrency changes happened
                    TakeSnapshot(aggregate, false);
                }

                // Return the stream of saved changes to the caller so they can be published.
                changes = _changeRepository.Save(aggregate, version);
            });

            return changes;
        }

        /// <summary>
        /// Gets the aggregate.
        /// </summary>
        public T Get<T>(Guid aggregateId, int? version = -1) where T : AggregateRoot
        {
            T aggregate;

            lock (_cache)
            {
                aggregate = (T)_cache.Get(aggregateId);

                if (aggregate == null)
                    aggregate = CreateAggregate<T>(aggregateId, null);

                _cache.Add(aggregate.AggregateIdentifier, aggregate, 5 * 60, true);
            }

            return aggregate;
        }

        public T GetClone<T>(Guid aggregateId, int? expectedVersion = -1) where T : AggregateRoot
        {
            var aggregate = Get<T>(aggregateId);
            if (aggregate != null)
            {
                T clone = default;

                aggregate.LockAndRun(() =>
                {
                    var originalState = _serializer.Serialize(aggregate.State);

                    clone = AggregateFactory<T>.CreateAggregate();
                    clone.AggregateIdentifier = aggregate.AggregateIdentifier;
                    clone.RootAggregateIdentifier = aggregate.RootAggregateIdentifier;
                    clone.AggregateVersion = aggregate.AggregateVersion;
                    clone.State = _serializer.Deserialize<AggregateState>(originalState, aggregate.State.GetType(), false);
                });

                return clone;
            }

            return CreateAggregate<T>(aggregateId, expectedVersion);
        }

        public void LockAndRun<T>(Guid aggregateId, Action<T> action) where T : AggregateRoot
        {
            var aggregate = Get<T>(aggregateId, null);

            aggregate.LockAndRun(() =>
            {
                action?.Invoke(aggregate);
            });
        }

        private T CreateAggregate<T>(Guid aggregateId, int? version) where T : AggregateRoot
        {
            // If it is not in the cache then load the aggregate from the most recent snapshot.
            var aggregate = AggregateFactory<T>.CreateAggregate();
            var snapshotVersion = RestoreAggregateFromSnapshot(aggregateId, aggregate);

            // If there is no snapshot then load the aggregate directly from the change store.
            if (snapshotVersion == (version ?? -1))
                return _changeRepository.Get<T>(aggregateId);

            // Otherwise load the aggregate from the changes that occurred after the snapshot was taken.
            var changes = _changeStore.GetChanges(aggregateId, snapshotVersion)
                .Where(desc => desc.AggregateVersion > snapshotVersion);

            aggregate.Rehydrate(changes);

            return aggregate;
        }

        public (AggregateState prev, AggregateState current) GetPrevAndCurrentStates(Guid aggregateId, int version)
        {
            var changes = _changeStore.GetChanges(aggregateId, -1, version);
            if (changes.Length == 0)
                return (null, null);

            _changeStore.GetClassAndOrganization(aggregateId, out var aggregateClass, out var _);

            var aggregateType = Type.GetType(aggregateClass);
            var aggregate = (AggregateRoot)aggregateType.GetConstructor(Type.EmptyTypes).Invoke(null);

            var prevState = aggregate.CreateState();
            for (int i = 0; i < changes.Length - 1; i++)
                prevState.Apply(changes[i]);

            var prevStateJson = _serializer.Serialize(prevState);

            var currentState = _serializer.Deserialize<AggregateState>(prevStateJson, prevState.GetType(), false);
            currentState.Apply(changes[changes.Length - 1]);

            if (changes.Length == 1)
                prevState = null;

            return (prevState, currentState);
        }

        public IChange[] GetStatesAndChanges(Guid aggregateId)
        {
            var changes = _changeStore.GetChanges(aggregateId, -1);
            if (changes.Length == 0)
                return changes;

            _changeStore.GetClassAndOrganization(aggregateId, out var aggregateClass, out var _);

            var aggregateType = Type.GetType(aggregateClass);
            var aggregate = (AggregateRoot)aggregateType.GetConstructor(Type.EmptyTypes).Invoke(null);

            var state = aggregate.CreateState();
            var stateType = state.GetType();

            for (int i = 0; i < changes.Length; i++)
            {
                var change = changes[i];

                state.Apply(change);

                var stateJson = _serializer.Serialize(state);

                change.AggregateState = _serializer.Deserialize<AggregateState>(stateJson, stateType, false);
            }

            return changes;
        }

        /// <summary>
        /// Returns a specific aggregate as at a specific version.
        /// </summary>
        public T Peek<T>(Guid _, int __) where T : AggregateRoot
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Returns true if an aggregate exists.
        /// </summary>
        public bool Exists<T>(Guid aggregate)
        {
            return _changeRepository.Exists<T>(aggregate);
        }

        /// <summary>
        /// Loads the aggregate from the most recent snapshot.
        /// </summary>
        /// <returns>
        /// Returns the version number for the aggregate when the snapshot was taken.
        /// </returns>
        private int RestoreAggregateFromSnapshot<T>(Guid id, T aggregate) where T : AggregateRoot
        {
            var snapshot = _snapshotStore.Get(id);

            if (snapshot == null)
                return -1;

            aggregate.AggregateIdentifier = snapshot.AggregateIdentifier;
            aggregate.AggregateVersion = snapshot.AggregateVersion;
            aggregate.State = _serializer.Deserialize<AggregateState>(snapshot.AggregateState, aggregate.CreateState().GetType(), false);

            return snapshot.AggregateVersion;
        }

        /// <summary>
        /// Saves a snapshot of the aggregate if the strategy indicates a snapshot should now be taken.
        /// </summary>
        private void TakeSnapshot(AggregateRoot aggregate, bool force)
        {
            var count = _snapshotStore.Count(aggregate.AggregateIdentifier);

            if (!force && !_snapshotStrategy.ShouldTakeSnapShot(aggregate, count))
                return;

            var snapshot = new Snapshot
            {
                AggregateIdentifier = aggregate.AggregateIdentifier,
                AggregateVersion = aggregate.AggregateVersion
            };

            SerializeAggregateState(aggregate, snapshot);

            snapshot.AggregateVersion = aggregate.AggregateVersion + aggregate.GetUncommittedChanges().Length;

            _snapshotStore.Save(snapshot);
        }

        private void SerializeAggregateState(AggregateRoot aggregate, Snapshot snapshot)
        {
            try
            {
                snapshot.AggregateState = _serializer.Serialize(aggregate.State);
            }
            catch (InvalidOperationException ex)
            {
                // Jan 14, 2022 - Daniel: Newtonsoft.Json.Serialization.JsonSerializerInternalWriter.SerializeDictionary
                // threw this exception today when it failed to serialize a MessageAggregate instance. We don't know
                // what specific aggregate failed to serialize, therefore I'm rethrowing the exception here with more
                // information for future troubleshooting, in case the exception recurs.

                var type = aggregate.GetType().Name;
                var id = aggregate.AggregateIdentifier;
                var version = aggregate.AggregateVersion;
                var message = $"Serialization failed for {type} {id} version {version}.";
                throw new SerializationFailedException(message, ex);
            }
        }

        #region Methods (boxing and unboxing)

        /// <summary>
        /// Checks for expired aggregates. Automatically boxes all aggregates for which the timer is now elapsed.
        /// </summary>
        public void Ping()
        {
            var aggregates = _changeStore.GetExpired(DateTimeOffset.UtcNow);
            foreach (var aggregate in aggregates)
                Box(Get<AggregateRoot>(aggregate));
        }

        /// <summary>
        /// Copies an aggregate to offline storage and removes it from online logs.
        /// </summary>
        public void Box<T>(T aggregate) where T : AggregateRoot
        {
            TakeSnapshot(aggregate, true);

            _snapshotStore.Box(aggregate.AggregateIdentifier);
            _changeStore.Box(aggregate.AggregateIdentifier, true);

            _cache.Remove(aggregate.AggregateIdentifier);
        }

        /// <summary>
        /// Retrieves an aggregate from offline storage and returns only its most recent state.
        /// </summary>
        public T Unbox<T>(Guid aggregateId) where T : AggregateRoot
        {
            var snapshot = _snapshotStore.Unbox(aggregateId);
            var aggregate = AggregateFactory<T>.CreateAggregate();
            aggregate.AggregateIdentifier = aggregateId;
            aggregate.AggregateVersion = 1;
            aggregate.State = _serializer.Deserialize<AggregateState>(snapshot.AggregateState, aggregate.CreateState().GetType(), false);
            return aggregate;
        }

        #endregion
    }
}