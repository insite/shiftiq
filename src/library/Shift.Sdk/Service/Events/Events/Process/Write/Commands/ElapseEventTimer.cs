using System;

using Shift.Common.Timeline.Commands;

namespace InSite.Application.Events.Write
{
    public class ElapseEventTimer : Command
    {
        public Guid Timer { get; set; }

        public ElapseEventTimer(Guid aggregate, Guid timer)
        {
            AggregateIdentifier = aggregate;
            Timer = timer;
        }
    }
}
