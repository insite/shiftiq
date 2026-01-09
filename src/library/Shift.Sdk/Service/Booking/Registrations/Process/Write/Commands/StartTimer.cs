using System;

using Shift.Common.Timeline.Commands;

namespace InSite.Application.Registrations.Write
{
    public class StartTimer : Command
    {
        public Guid Timer { get; set; }
        public DateTimeOffset At { get; set; }
        public string Description { get; set; }

        public StartTimer(Guid aggregate, Guid timer, DateTimeOffset at, string description)
        {
            AggregateIdentifier = aggregate;

            Timer = timer;
            At = at;
            Description = description;
        }
    }
}
