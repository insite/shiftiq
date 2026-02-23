using System;

using Shift.Common.Timeline.Commands;

namespace InSite.Application.Events.Write
{
    public class ModifyEventDisplayOnCalendar : Command
    {
        public bool Display { get; set; }

        public ModifyEventDisplayOnCalendar(Guid aggregate, bool display)
        {
            AggregateIdentifier = aggregate;
            Display = display;
        }
    }
}
