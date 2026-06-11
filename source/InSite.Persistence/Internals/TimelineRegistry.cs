using Shift.Common.Timeline.Changes;

using Shift.Common;

using TimelineServices = Shift.Common.Timeline.Services;

namespace InSite
{
    internal class TimelineRegistry
    {
        static GuidCache<AggregateRoot> _aggregateCache;
        static TimelineServices.IGuidGenerator _guidGenerator;

        internal static void Initialize(Shift.Common.IJsonSerializer serializer)
        {
            _aggregateCache = new GuidCache<AggregateRoot>();
            _guidGenerator = new UuidFactory();

            TimelineServices.ServiceLocator.Instance.Register<Shift.Common.IJsonSerializer>(serializer);
            TimelineServices.ServiceLocator.Instance.Register<TimelineServices.IGuidGenerator>(_guidGenerator);
            TimelineServices.ServiceLocator.Instance.Register<TimelineServices.IGuidCache<AggregateRoot>>(_aggregateCache);
        }
    }
}