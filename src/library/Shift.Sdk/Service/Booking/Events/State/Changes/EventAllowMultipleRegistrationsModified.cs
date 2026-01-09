using Shift.Common.Timeline.Changes;

namespace InSite.Domain.Events
{
    public class EventAllowMultipleRegistrationsModified : Change
    {
        public bool Value { get; set; }

        public EventAllowMultipleRegistrationsModified(bool value)
        {
            Value = value;
        }
    }
}
