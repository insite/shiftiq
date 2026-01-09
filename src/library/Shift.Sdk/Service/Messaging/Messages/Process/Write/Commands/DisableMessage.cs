using System;

using Shift.Common.Timeline.Commands;

namespace InSite.Application.Messages.Write
{
    public class DisableMessage : Command
    {
        public DisableMessage(Guid message)
        {
            AggregateIdentifier = message;
        }
    }

    public class DisableAutoBccSubscribers : Command
    {
        public DisableAutoBccSubscribers(Guid message)
        {
            AggregateIdentifier = message;
        }
    }
}