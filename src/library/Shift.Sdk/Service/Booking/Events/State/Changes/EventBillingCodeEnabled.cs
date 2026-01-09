using Shift.Common.Timeline.Changes;

namespace InSite.Domain.Events
{
    public class EventBillingCodeEnabled : Change
    {
        public bool Enabled { get; set; }

        public EventBillingCodeEnabled(bool enabled)
        {
            Enabled = enabled;
        }
    }
}
