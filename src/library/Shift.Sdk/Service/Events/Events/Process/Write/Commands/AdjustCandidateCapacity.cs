using System;

using Shift.Common.Timeline.Commands;

using Shift.Constant;

namespace InSite.Application.Events.Write
{
    public class AdjustCandidateCapacity : Command
    {
        public int? Minimum { get; set; }
        public int? Maximum { get; set; }
        public ToggleType Waitlist { get; set; }

        public AdjustCandidateCapacity(Guid @event, int? min, int? max, ToggleType waitlist)
        {
            AggregateIdentifier = @event;
            Minimum = min;
            Maximum = max;
            Waitlist = waitlist;
        }
    }
}
