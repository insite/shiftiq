using System;

using Shift.Common.Timeline.Commands;

namespace InSite.Application.Messages.Write
{
    public class EnableMessage : Command
    {
        public EnableMessage(Guid message)
        {
            AggregateIdentifier = message;
        }
    }

    public class EnableAutoBccSubscribers : Command
    {
        public EnableAutoBccSubscribers(Guid message)
        {
            AggregateIdentifier = message;
        }
    }
}