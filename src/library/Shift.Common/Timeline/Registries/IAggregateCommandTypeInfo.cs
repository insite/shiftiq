using System;

namespace Common.Timeline.Registries
{
    public interface IAggregateCommandTypeInfo
    {
        Guid ID { get; }
        Type Type { get; }
        IAggregateTypeInfo[] Aggregates { get; }
    }
}
