using System;

using Shift.Common.Timeline.Commands;

namespace InSite.Application.Events.Write
{
    public class AdjustInvigilatorCapacity : Command
    {
        public int? Minimum { get; set; }
        public int? Maximum { get; set; }

        public AdjustInvigilatorCapacity(Guid @event, int? min, int? max)
        {
            AggregateIdentifier = @event;
            Minimum = min;
            Maximum = max;
        }
    }
}
