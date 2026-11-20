using System;

using Shift.Common.Timeline.Commands;

namespace InSite.Application.Events.Write
{
    public class ModifyFormAttachmentLimit : Command
    {
        public int? Value { get; set; }

        public ModifyFormAttachmentLimit(Guid @event, int? value)
        {
            AggregateIdentifier = @event;
            Value = value;
        }
    }
}
