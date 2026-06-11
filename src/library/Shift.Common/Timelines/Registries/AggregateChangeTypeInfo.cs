using System;

using Shift.Common.Timeline.Services;

namespace Shift.Common.Timeline.Registries
{
    public class AggregateChangeTypeInfo : IAggregateChangeTypeInfo
    {
        public Guid ID { get; }
        public Type Type { get; }
        public IAggregateTypeInfo Aggregate { get; }

        public AggregateChangeTypeInfo(Type t, IAggregateTypeInfo agg)
        {
            var generator = ServiceLocator.Instance.GetService<IGuidGenerator>();

            ID = generator.NewGuid(t);
            Type = t;
            Aggregate = agg;
        }

        public override string ToString() => Type.Name;
    }
}
