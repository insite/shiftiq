using System;

using Shift.Common.Timeline.Commands;

namespace InSite.Application.Events.Write
{
    public class CancelEventTimer : Command
    {
        public Guid Timer { get; set; }

        public CancelEventTimer(Guid aggregate, Guid timer)
        {
            AggregateIdentifier = aggregate;
            Timer = timer;
        }
    }
}
