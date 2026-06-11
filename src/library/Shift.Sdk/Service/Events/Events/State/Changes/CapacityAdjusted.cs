using Shift.Common.Timeline.Changes;

using Newtonsoft.Json;

using Shift.Constant;

namespace InSite.Domain.Events
{
    public class CapacityAdjusted : Change
    {
        public int? Minimum { get; set; }
        public int? Maximum { get; set; }

        [JsonProperty(DefaultValueHandling = DefaultValueHandling.Include)]
        public ToggleType Waitlist { get; set; }

        public CapacityAdjusted(int? min, int? max, ToggleType waitlist)
        {
            Minimum = min;
            Maximum = max;
            Waitlist = waitlist;
        }
    }
}