
using Shift.Common.Timeline.Changes;

namespace InSite.Domain.Events
{
    public class EventNotificationTriggered : Change
    {
        public string Name { get; set; }

        public EventNotificationTriggered(string name)
        {
            Name = name;
        }
    }
}