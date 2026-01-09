using System;

using Shift.Common.Timeline.Commands;

namespace InSite.Application.Events.Write
{
    public class CancelEvent : Command
    {
        public string Reason { get;  }
        public bool CancelRegistrations { get; }

        public CancelEvent(Guid aggregate, string reason, bool cancelRegistrations)
        {
            AggregateIdentifier = aggregate;
            Reason = reason;
            CancelRegistrations = cancelRegistrations;
        }
    }
}
