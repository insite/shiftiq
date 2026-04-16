using System;
using System.Threading.Tasks;

using Shift.Common.Timeline.Changes;

namespace Shift.Sdk.Service
{
    public interface ITimelineQuery
    {
        Task<StateType> GetAggregateStateAsync<AggregateType, StateType>(Guid aggregateId)
            where AggregateType : AggregateRoot
            where StateType : AggregateState
            ;
    }
}
