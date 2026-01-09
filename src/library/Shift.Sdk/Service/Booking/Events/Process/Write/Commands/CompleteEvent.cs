using System;

using Shift.Common.Timeline.Commands;

namespace InSite.Application.Events.Write
{
    public class CompleteEvent : Command
    {
        public CompleteEvent(Guid aggregate)
        {
            AggregateIdentifier = aggregate;
        }
    }
}
