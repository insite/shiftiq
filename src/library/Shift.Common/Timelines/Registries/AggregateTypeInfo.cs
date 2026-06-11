using System;

using Shift.Common.Timeline.Services;

namespace Shift.Common.Timeline.Registries
{
    public class AggregateTypeInfo : IAggregateTypeInfo
    {
        public Guid ID { get; }
        public Type Type { get; }
        public string Name { get; }

        public IAggregateChangeTypeInfo[] Changes { get; set; }
        public IAggregateCommandTypeInfo[] Commands { get; set; }

        public AggregateTypeInfo(Type t)
        {
            const string postfix = "Aggregate";

            var generator = ServiceLocator.Instance.GetService<IGuidGenerator>();

            ID = generator.NewGuid(t);
            Type = t;
            Name = t.Name.EndsWith(postfix)
                ? t.Name.Substring(0, t.Name.Length - postfix.Length)
                : t.Name;
        }

        public override string ToString() => Type.Name;
    }
}
