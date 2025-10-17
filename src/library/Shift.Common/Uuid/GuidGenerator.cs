using System;

using Shift.Common;

using TimelineServices = Common.Timeline.Services;

namespace Shift.Common
{
    public class NewGuidGenerator : TimelineServices.IGuidGenerator
    {
        public Guid NewGuid(Type t)
            => GuidGenerator.NewGuid(t);

        public Guid NewGuid()
            => GuidGenerator.NewGuid();
    }
}
