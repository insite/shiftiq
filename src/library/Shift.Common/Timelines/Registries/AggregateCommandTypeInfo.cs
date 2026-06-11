using System;
using System.Collections.Generic;
using System.Linq;

using Shift.Common.Timeline.Services;

namespace Shift.Common.Timeline.Registries
{
    public class AggregateCommandTypeInfo : IAggregateCommandTypeInfo
    {
        public Guid ID { get; }
        public Type Type { get; }
        public IAggregateTypeInfo[] Aggregates { get; private set; }

        private List<IAggregateTypeInfo> _aggregatesList;

        public AggregateCommandTypeInfo(Type t)
        {
            var generator = ServiceLocator.Instance.GetService<IGuidGenerator>();
            
            ID = generator.NewGuid(t);
            Type = t;
            _aggregatesList = new List<IAggregateTypeInfo>();
        }

        public void AddAggregate(IAggregateTypeInfo agg)
        {
            _aggregatesList.Add(agg);
        }

        public void Lock()
        {
            Aggregates = _aggregatesList.OrderBy(x => x.Name).ToArray();
            _aggregatesList = null;
        }

        public override string ToString() => Type.Name;
    }
}
