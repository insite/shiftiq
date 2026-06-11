using Shift.Common.Timeline.Changes;

namespace InSite.Domain.Events
{
    public class EventPersonCodeIsRequiredModified : Change
    {
        public bool Value { get; set; }

        public EventPersonCodeIsRequiredModified(bool value)
        {
            Value = value;
        }
    }
}
