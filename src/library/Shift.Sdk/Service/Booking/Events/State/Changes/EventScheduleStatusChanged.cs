using Shift.Common.Timeline.Changes;

namespace InSite.Domain.Events
{
    public class EventScheduleStatusChanged : Change
    {
        public string Status { get; set; }

        public EventScheduleStatusChanged(string status)
        {
            Status = status;
        }
    }
}