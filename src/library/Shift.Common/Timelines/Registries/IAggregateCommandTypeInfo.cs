using System;

namespace Shift.Common.Timeline.Registries
{
    public interface IAggregateCommandTypeInfo
    {
        Guid ID { get; }
        Type Type { get; }
        IAggregateTypeInfo[] Aggregates { get; }
    }
}
