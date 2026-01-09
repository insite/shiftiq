using System;

using Shift.Common.Timeline.Commands;

namespace InSite.Application.Events.Write
{
    public class ModifyPersonCodeIsRequired : Command
    {
        public bool Value { get; set; }

        public ModifyPersonCodeIsRequired(Guid @event, bool value)
        {
            AggregateIdentifier = @event;
            Value = value;
        }
    }
}
