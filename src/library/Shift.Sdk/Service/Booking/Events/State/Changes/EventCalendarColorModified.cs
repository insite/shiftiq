using Shift.Common.Timeline.Changes;

namespace InSite.Domain.Events
{
    public class EventCalendarColorModified : Change
    {
        public string CalendarColor { get; }

        public EventCalendarColorModified(string calendarColor)
        {
            CalendarColor = calendarColor;
        }
    }
}
