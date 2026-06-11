using Shift.Common.Timeline.Changes;

namespace InSite.Domain.Organizations
{
    public class OrganizationEventSettingsModified : Change
    {
        public EventSettings Events { get; set; }

        public OrganizationEventSettingsModified(EventSettings events)
        {
            Events = events;
        }
    }
}
