using Shift.Common.Timeline.Changes;

namespace InSite.Domain.Events
{
    public class EventRequestStatusChanged : Change
    {
        public string Status { get; set; }

        public EventRequestStatusChanged(string status)
        {
            Status = status;
        }
    }
}