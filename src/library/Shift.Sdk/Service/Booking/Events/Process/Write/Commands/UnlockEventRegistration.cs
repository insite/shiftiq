using System;

using Shift.Common.Timeline.Commands;

namespace InSite.Application.Events.Write
{
    public class UnlockEventRegistration : Command
    {
        public UnlockEventRegistration(Guid eventId)
        {
            AggregateIdentifier = eventId;
        }
    }
}
