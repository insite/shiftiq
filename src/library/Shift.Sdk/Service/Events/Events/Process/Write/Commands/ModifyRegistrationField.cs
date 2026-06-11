using System;

using Shift.Common.Timeline.Commands;

using InSite.Domain.Events;

namespace InSite.Application.Events.Write
{
    public class ModifyRegistrationField : Command
    {
        public RegistrationField Field { get; set; }

        public ModifyRegistrationField(Guid @event, RegistrationField field)
        {
            AggregateIdentifier = @event;
            Field = field ?? throw new ArgumentNullException(nameof(field));
        }
    }
}
