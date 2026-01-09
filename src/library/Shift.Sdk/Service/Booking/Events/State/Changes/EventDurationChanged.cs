using Shift.Common.Timeline.Changes;

namespace InSite.Domain.Events
{
    public class EventDurationChanged : Change
    {
        public int? Duration { get; set; }
        public string Unit { get; set; }

        public EventDurationChanged(int? duration, string unit)
        {
            Duration = duration;
            Unit = unit;
        }
    }
}
