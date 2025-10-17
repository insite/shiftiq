using System;

namespace Common.Timeline.Registries
{
    public interface IAggregateChangeTypeInfo
    {
        Guid ID { get; }
        Type Type { get; }
        IAggregateTypeInfo Aggregate { get; }
    }
}
