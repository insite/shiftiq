using Shift.Common.Timeline.Changes;

namespace InSite.Domain.Events
{
    public class EventCancelled : Change
    {
        public string Reason { get; }
        public bool CancelRegistrations { get; }

        public EventCancelled(string reason, bool cancelRegistrations)
        {
            Reason = reason;
            CancelRegistrations = cancelRegistrations;
        }
    }
}