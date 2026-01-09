using System;

using Shift.Common.Timeline.Commands;

namespace InSite.Application.Events.Write
{
    public class TriggerEventNotification : Command
    {
        public string Name { get; set; }

        public TriggerEventNotification(Guid aggregate, string name)
        {
            AggregateIdentifier = aggregate;
            Name = name;
        }
    }
}
