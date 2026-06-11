using System;

using Shift.Common.Timeline.Commands;

namespace InSite.Application.Events.Write
{
    public class UnpublishEvent : Command
    {
        public UnpublishEvent(Guid aggregate)
        {
            AggregateIdentifier = aggregate;
        }
    }
}
