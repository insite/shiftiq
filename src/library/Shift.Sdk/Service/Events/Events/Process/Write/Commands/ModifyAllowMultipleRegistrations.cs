using System;

using Shift.Common.Timeline.Commands;

namespace InSite.Application.Events.Write
{
    public class ModifyAllowMultipleRegistrations : Command
    {
        public bool Value { get; set; }

        public ModifyAllowMultipleRegistrations(Guid @event, bool value)
        {
            AggregateIdentifier = @event;
            Value = value;
        }
    }
}
