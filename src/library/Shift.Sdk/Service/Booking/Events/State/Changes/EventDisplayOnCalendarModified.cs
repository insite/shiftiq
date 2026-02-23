using Shift.Common.Timeline.Changes;

namespace InSite.Domain.Events
{
    public class EventDisplayOnCalendarModified : Change
    {
        public bool Display { get; set; }

        public EventDisplayOnCalendarModified(bool display)
        {
            Display = display;
        }
    }
}
