using Shift.Common.Timeline.Changes;

namespace InSite.Domain.Events
{
    public class EventPublicationCompleted : Change
    {
        public string Status { get; set; }
        public string Errors { get; set; }

        public EventPublicationCompleted(string status, string errors)
        {
            Status = status;
            Errors = errors;
        }
    }
}