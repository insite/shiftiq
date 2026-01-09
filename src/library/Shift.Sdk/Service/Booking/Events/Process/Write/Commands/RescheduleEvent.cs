using System;

using Shift.Common.Timeline.Commands;

namespace InSite.Application.Events.Write
{
    public class RescheduleEvent : Command
    {
        public DateTimeOffset StartTime { get; set; }
        public DateTimeOffset EndTime { get; set; }

        public RescheduleEvent(Guid @event, DateTimeOffset start, DateTimeOffset end)
        {
            AggregateIdentifier = @event;
            StartTime = start;
            EndTime = end;
        }
    }
}