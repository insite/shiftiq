using System;

using Shift.Common.Timeline.Commands;

namespace InSite.Application.Events.Write
{
    public class StartEventTimer : Command
    {
        public Guid Timer { get; set; }
        public DateTimeOffset At { get; set; }
        public string Description { get; set; }

        public StartEventTimer(Guid aggregate, Guid timer, DateTimeOffset at, string description)
        {
            AggregateIdentifier = aggregate;
            Timer = timer;
            At = at;
            Description = description;
        }
    }
}
