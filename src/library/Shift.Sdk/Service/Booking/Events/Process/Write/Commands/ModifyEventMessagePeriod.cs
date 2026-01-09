using System;

using Shift.Common.Timeline.Commands;

namespace InSite.Application.Events.Write
{
    public class ModifyEventMessagePeriod : Command
    {
        public int? SendReminderBeforeDays { get; set; }

        public ModifyEventMessagePeriod(Guid eventId, int? sendReminderBeforeDays)
        {
            AggregateIdentifier = eventId;
            SendReminderBeforeDays = sendReminderBeforeDays;
        }
    }
}
