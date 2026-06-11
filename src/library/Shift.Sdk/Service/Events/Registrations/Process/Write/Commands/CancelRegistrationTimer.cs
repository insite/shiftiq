using System;

using Shift.Common.Timeline.Commands;

namespace InSite.Application.Registrations.Write
{
    public class CancelRegistrationTimer : Command
    {
        public Guid Timer { get; set; }

        public CancelRegistrationTimer(Guid aggregate, Guid timer)
        {
            AggregateIdentifier = aggregate;
            Timer = timer;
        }
    }
}
