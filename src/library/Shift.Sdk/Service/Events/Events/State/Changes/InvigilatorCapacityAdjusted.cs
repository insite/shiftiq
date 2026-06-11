
using Shift.Common.Timeline.Changes;

namespace InSite.Domain.Events
{
    public class InvigilatorCapacityAdjusted : Change
    {
        public int? Minimum { get; set; }
        public int? Maximum { get; set; }

        public InvigilatorCapacityAdjusted(int? min, int? max)
        {
            Minimum = min;
            Maximum = max;
        }
    }
}