using System;

namespace Shift.Common.Timeline.Registries
{
    public interface IAggregateTypeInfo
    {
        Guid ID { get; }
        Type Type { get; }
        string Name { get; }

        IAggregateChangeTypeInfo[] Changes { get; }
        IAggregateCommandTypeInfo[] Commands { get; }
    }
}
