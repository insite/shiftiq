using System;

namespace Shift.Common.Timeline.Registries
{
    public interface IAggregateChangeTypeInfo
    {
        Guid ID { get; }
        Type Type { get; }
        IAggregateTypeInfo Aggregate { get; }
    }
}
