using System;
using System.Collections.Generic;

using Shift.Common.Timeline.Changes;

namespace InSite.Application.Logs.Read
{
    public interface IAggregateSearch
    {
        SerializedAggregate Get(Guid aggregateId);
        SerializedAggregate[] Get(IEnumerable<Guid> aggregateIds);
        SerializedAggregate[] GetByClass(Guid organizationId, string[] classes);
        SerializedAggregate[] GetByType(string type, Guid? organization);
        SerializedAggregate[] GetByOrganization(Guid organization);
        SerializedAggregate[] GetGhosts();

        AggregateState GetState<T>(Guid id) where T : AggregateRoot;

        StateType GetState<AggregateType, StateType>(Guid id)
            where AggregateType : AggregateRoot
            where StateType : AggregateState;
    }
}
