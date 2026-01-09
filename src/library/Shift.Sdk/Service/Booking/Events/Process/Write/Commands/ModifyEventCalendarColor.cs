using System;

using Shift.Common.Timeline.Commands;

namespace InSite.Application.Events.Write
{
    public class ModifyEventCalendarColor : Command
    {
        public string CalendarColor { get; }

        public ModifyEventCalendarColor(Guid @event, string calendarColor)
        {
            AggregateIdentifier = @event;
            CalendarColor = calendarColor;
        }
    }
}
