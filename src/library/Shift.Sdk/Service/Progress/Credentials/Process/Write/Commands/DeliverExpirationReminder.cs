using System;

using Shift.Common.Timeline.Commands;

using Shift.Constant;

namespace InSite.Application.Credentials.Write
{
    public class DeliverExpirationReminder : Command
    {
        public DeliverExpirationReminder(Guid credential, ReminderType type, DateTimeOffset? delivered)
        {
            AggregateIdentifier = credential;
            Type = type;
            Delivered = delivered;
        }

        public ReminderType Type { get; set; }
        public DateTimeOffset? Delivered { get; set; }
    }
}