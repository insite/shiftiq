using System;

using Shift.Common.Timeline.Commands;

namespace InSite.Application.Events.Write
{
    public class EnableEventBillingCode : Command
    {
        public bool Enabled { get; set; }

        public EnableEventBillingCode(Guid aggregate, bool enabled)
        {
            AggregateIdentifier = aggregate;
            Enabled = enabled;
        }
    }
}
