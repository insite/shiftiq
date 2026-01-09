using System;

using Shift.Common.Timeline.Commands;

namespace InSite.Application.Registrations.Write
{
    public class ElapseRegistrationTimer : Command
    {
        public Guid Timer { get; set; }

        public ElapseRegistrationTimer(Guid aggregate, Guid timer)
        {
            AggregateIdentifier = aggregate;
            Timer = timer;
        }
    }
}
