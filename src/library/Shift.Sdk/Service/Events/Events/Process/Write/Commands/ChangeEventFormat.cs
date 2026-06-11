using System;

using Shift.Common.Timeline.Commands;

namespace InSite.Application.Events.Write
{
    public class ChangeEventFormat : Command
    {
        public string EventFormat { get; set; }

        public ChangeEventFormat(Guid @event, string format)
        {
            AggregateIdentifier = @event;
            EventFormat = format;
        }
    }
}