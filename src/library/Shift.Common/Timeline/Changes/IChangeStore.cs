using System;
using System.Collections.Generic;

namespace Shift.Common.Timeline.Changes
{
    /// <summary>
    /// Defines the methods needed from the change store.
    /// </summary>
    public interface IChangeStore
    {
        /// <summary>
        /// Gets count of changes for an aggregate.
        /// </summary>
        int Count(Guid aggregate, int fromVersion = -1);

        /// <summary>
        /// Used by ChangeGrid
        /// </summary>
        int Count(Guid aggregate, string keyword, bool includeChildren);

        /// <summary>
        /// Gets count of changes for an aggregate.
        /// </summary>
        int Count(string aggregateType, IEnumerable<Guid> aggregateIdentifiers);

        /// <summary>
        /// Returns true if an aggregate exists.
        /// </summary>
        bool Exists<T>(Guid aggregate);

        /// <summary>
        /// Returns true if an aggregate with a specific version exists.
        /// </summary>
        bool Exists(Guid aggregate, int version);

        /// <summary>
        /// Gets all the identifiers for a specific aggregate type.
        /// </summary>
        List<Guid> GetAggregates(string aggregateType);

        /// <summary>
        /// Gets changes for an aggregate starting at a specific version. To get all changes use version = -1.
        /// </summary>
        IChange[] GetChanges(Guid aggregate, int fromVersion);

        /// <summary>
        /// Gets changes for an aggregate within a specific range of versions.
        /// </summary>
        IChange[] GetChanges(Guid aggregate, int fromVersion, int toVersion);

        /// <summary>
        /// Gets changes for all aggregates of a specific type.
        /// </summary>
        IChange[] GetChanges(string aggregateType, Guid? id, IEnumerable<string> changeTypes);

        /// <summary>
        /// Gets serialized changes for all aggregates of a specific type.
        /// </summary>
        /// <returns></returns>
        List<SerializedChange> GetSerializedChangesPaged(Guid aggregate, string keyword, bool includeChildren, int skip, int pageSize);

        /// <summary>
        /// Enumerate changes for all aggregates of a specific type.
        /// </summary>
        List<IChange> GetChanges(string aggregateType, IEnumerable<Guid> aggregateIdentifiers);

        /// <summary>
        /// Used in ChangeGrid
        /// </summary>
        IChange[] GetChangesPaged(Guid aggregate, string keyword, bool includeChildren, int skip, int pageSize);

        /// <summary>
        /// Gets a specific version of change for an aggregate.
        /// </summary>
        IChange GetChange(Guid aggregate, int version);

        /// <summary>
        /// Gets all aggregates that are scheduled to expire at (or before) a specific time on a specific date.
        /// </summary>
        Guid[] GetExpired(DateTimeOffset at);

        /// <summary>
        /// Save changes.
        /// </summary>
        void Save(AggregateRoot aggregate, IEnumerable<IChange> changes);
        void Save(IEnumerable<AggregateImport> import);

        /// <summary>
        /// Save change.
        /// </summary>
        void Save(IChange change);

        /// <summary>
        /// Insert change into the stream at a specific position.
        /// </summary>
        /// <remarks>
        /// Aggregate change streams index from starting position 1 (not 0).
        /// </remarks>
        void Insert(IChange change, int index);

        /// <summary>
        /// Copies an aggregate to offline storage and removes it from online logs.
        /// </summary>
        /// <remarks>
        /// Someone who is a purist with regard to change sourcing will red-flag this function and say the change stream 
        /// for an aggregate should never be altered or removed. However, we have two scenarios in which this is a non-
        /// negotiable business requirement. First, when a customer does not renew their contract with our business, we
        /// have a contractual obligation to remove all the customer's data from our systems. Second, we frequently run
        /// test-cases to confirm system functions are operating correctly; this data is temporary by definition, and 
        /// we do not want to permanently store the change streams for test aggregates.
        /// </remarks>
        void Box(Guid aggregate, bool archive);
        void Unbox(Guid aggregate, Func<Guid, AggregateRoot> creator);
        
        /// <summary>
        /// Performs a rollback on a specific aggregate to a specific version number. In simplest terms, this method deletes
        /// all the changes in an aggregate where the version number is greater than or equal to the input parameter.
        /// </summary>
        int Rollback(Guid id, int version);

        /// <summary>
        /// Allows to mark a change as an obsolete
        /// ChangeStore doesn't try to deserialize an obsolete change and returns the change as SerializedChange
        /// This is useful when a specific change is obsolete and we need either ignore it or transform to a new change
        /// </summary>
        void RegisterObsoleteChangeTypes(IEnumerable<string> changeTypes);

        void GetClassAndOrganization(Guid aggregate, out string @class, out Guid organization);
    }
}
