using System;

using Shift.Common.Timeline.Commands;

namespace InSite.Application.Events.Write
{
    public class ChangeEventDuration : Command
    {
        public int? Duration { get; set; }
        public string Unit { get; set; }

        public ChangeEventDuration(Guid @event, int? duration, string unit)
        {
            AggregateIdentifier = @event;
            Duration = duration;
            Unit = unit;
        }
    }
}
