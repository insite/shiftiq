using System;

using Shift.Common.Timeline.Commands;

using Shift.Constant;

namespace InSite.Application.Credentials.Write
{
    public class RequestExpirationReminder : Command
    {
        public RequestExpirationReminder(Guid credential, ReminderType type, DateTimeOffset requested)
        {
            AggregateIdentifier = credential;
            Type = type;
            Requested = requested;
        }

        public ReminderType Type { get; set; }
        public DateTimeOffset Requested { get; set; }
    }
}