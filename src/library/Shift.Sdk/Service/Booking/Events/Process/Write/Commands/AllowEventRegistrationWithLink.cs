using System;

using Common.Timeline.Commands;

namespace InSite.Application.Events.Write
{
    public class AllowEventRegistrationWithLink : Command
    {
        public AllowEventRegistrationWithLink(Guid aggregate)
        {
            AggregateIdentifier = aggregate;
        }
    }
}
