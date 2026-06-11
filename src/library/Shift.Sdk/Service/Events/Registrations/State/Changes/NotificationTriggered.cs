
using Shift.Common.Timeline.Changes;

namespace InSite.Domain.Registrations
{
    public class NotificationTriggered : Change
    {
        public string Name { get; set; }

        public NotificationTriggered(string name)
        {
            Name = name;
        }
    }
}
