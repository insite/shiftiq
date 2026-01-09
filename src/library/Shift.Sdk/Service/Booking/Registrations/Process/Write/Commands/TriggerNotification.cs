using System;

using Shift.Common.Timeline.Commands;

namespace InSite.Application.Registrations.Write
{
    public class TriggerNotification : Command
    {
        public string Name { get; set; }

        public TriggerNotification(Guid aggregate, string name)
        {
            AggregateIdentifier = aggregate;
            Name = name;
        }
    }
}
