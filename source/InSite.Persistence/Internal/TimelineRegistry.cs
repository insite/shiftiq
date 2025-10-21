using Common.Timeline.Changes;

using Shift.Common;
using Shift.Toolbox;

using TimelineServices = Common.Timeline.Services;

namespace InSite
{
    internal class TimelineRegistry
    {
        static GuidCache<AggregateRoot> _aggregateCache;
        static TimelineServices.IGuidGenerator _guidGenerator;

        internal static void Initialize(Shift.Common.Json.Serializer serializer)
        {
            _aggregateCache = new GuidCache<AggregateRoot>();
            _guidGenerator = new NewGuidGenerator();

            TimelineServices.ServiceLocator.Instance.Register<TimelineServices.IJsonSerializer>(serializer);
            TimelineServices.ServiceLocator.Instance.Register<TimelineServices.IGuidGenerator>(_guidGenerator);
            TimelineServices.ServiceLocator.Instance.Register<TimelineServices.IGuidCache<AggregateRoot>>(_aggregateCache);
        }
    }
}